using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public Tile occupiedTile;
    public int rad = 1;
    [SerializeField] private Image unitImage;
    [SerializeField] private Button selectUnit;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float moveDuration = 2f;
    private bool _isSelected;

    public void OccupyTile(Tile tile)
    {
        tile.unitOnTile = gameObject;
        occupiedTile = tile;
    }
    
    private void Start()
    {
        unitImage.SetNativeSize();
        selectUnit.onClick.AddListener(SelectUnit);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (pastO == gameObject)
        {
            if (currO != null && currO.TryGetComponent(out Tile tile))
            {
                if(tile.IsTileFree() && tile.IsCanMoveTo())
                    MoveToTile(tile);
                if(!tile.IsTileFree() && tile.IsCanAttackTo())
                    AttackUnitOnTile();
            }
            DeselectUnit();
        }
    }

    private void MoveToTile(Tile to)
    {
        if(occupiedTile != null)
        {
            occupiedTile.unitOnTile = null;
        }
        OccupyTile(to);
        
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetParent(to.transform.parent);
        transform.SetSiblingIndex(transform.parent.childCount);
        var inValX = rectTransform.anchoredPosition.x;
        DOTween.To(() => inValX, x => inValX = x, anchorPos.x, moveDuration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(inValX, rectTransform.anchoredPosition.y);
        }));
        var inValY = rectTransform.anchoredPosition.y;
        DOTween.To(() => inValY, x => inValY = x, anchorPos.y + 30, moveDuration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, inValY);
        }));
    }

    private void AttackUnitOnTile()
    {
        
    }
    
    private void SelectUnit()
    {
        if (_isSelected)
        {
            occupiedTile.SelectTile();
            return;
        }
        
        _isSelected = true;
        LevelManager.Instance.currentName = "Warior";
        LevelManager.Instance.SelectObject(gameObject);

    }

    private void DeselectUnit()
    {
        _isSelected = false;
    }
}
