using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class Scout : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;
    private Sequence _moveSeq;
    public RectTransform rectTransform;
    
    public void StartExploring(Home home, Tile t)
    {
        rectTransform.anchoredPosition = t.GetRectTransform().anchoredPosition;
        Circle(t, 0);

        void Circle(Tile startTile, int index)
        {
            if (index >= 20)
            {
                Destroy(gameObject);
                return;
            }
            
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(startTile, 4);
            closeTile.RemoveAll(tile1 => tile1.tileType == Tile.TileType.Water && !home.owner.technologies.Contains(TechInfo.Technology.Sailing));
            closeTile.RemoveAll(tile1 => tile1.tileType is Tile.TileType.DeepWater && !home.owner.technologies.Contains(TechInfo.Technology.Navigation));
            closeTile.RemoveAll(tile1 =>
                tile1.isHasMountain && !home.owner.technologies.Contains(TechInfo.Technology.Mountain));
            var allClosedTile = closeTile.FindAll(tile1 => !tile1.isOpened);
            if (allClosedTile.Count == 0)
            {
                Destroy(gameObject);
                return;
            }
            var rand = Random.Range(0, allClosedTile.Count);
            var randomTile = allClosedTile[rand];
            if (randomTile != null)
            {
                for (var j = 0; j < 20; j++)
                {
                    rand = Random.Range(0, allClosedTile.Count);
                    randomTile = allClosedTile[rand];
                    var vector2Int = AStarAlgorithm.FindPath(startTile.pos, randomTile.pos);
                    if (vector2Int != null)
                    {
                        randomTile = LevelManager.Instance.gameBoardWindow.GetTile(vector2Int[0]);
                        break;
                    }
                }
            }
            else
            {
                var close = LevelManager.Instance.gameBoardWindow.GetCloseTile(startTile, 1);
                close.RemoveAll(tile1 => tile1.tileType == Tile.TileType.Water);
                close.RemoveAll(tile1 => tile1.tileType is Tile.TileType.DeepWater);
                close.RemoveAll(tile1 => !tile1.IsTileFree());
                close.RemoveAll(tile1 =>
                    tile1.isHasMountain && !home.owner.technologies.Contains(TechInfo.Technology.Mountain));

                randomTile = close[Random.Range(0, close.Count)];
                if (randomTile == null)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            MoveToTile(randomTile).OnComplete((() =>
            {
                var playerCiv = LevelManager.Instance.gameBoardWindow.playerCiv;
                var addedRad = 0;
                if (randomTile.isHasMountain)
                    addedRad = 1;
                var closeTiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(randomTile, 1 + addedRad);
                foreach (var tile in closeTiles)
                { 
                    tile.UnlockTile(playerCiv);
                }
                Circle(randomTile, index + 1);
            }));
        }
    }
    
    private Tween MoveToTile(Tile to)
    {
        _moveSeq = DOTween.Sequence();
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetSiblingIndex(transform.parent.childCount);
        var inValX = rectTransform.anchoredPosition.x;
        _moveSeq.Append(DOTween.To(() => inValX, x => inValX = x, anchorPos.x, duration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(inValX, rectTransform.anchoredPosition.y);
        })));
        var inValY = rectTransform.anchoredPosition.y;
        _moveSeq.Join(DOTween.To(() => inValY, x => inValY = x, anchorPos.y + 30, duration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, inValY);
        })));
        var inVal1 = 0;
        _moveSeq.Join(DOTween.To(() => inVal1, x => inVal1 = x, 1, duration));

        return _moveSeq;
    }
}
