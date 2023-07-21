using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBoardWindow : BaseWindow
{
    [SerializeField] private List<GameObject> boardPrefabs;

    [SerializeField] private List<GameObject> generatedTiles;
    [SerializeField] private RectTransform tilePrefab;
    [SerializeField] private RectTransform tileParent;

    [SerializeField] private int mapSize = 16;
    [SerializeField] private float heightMultiplier = 0.01f;
    [SerializeField] private GameObject homePrefab;

    private void Awake()
    {
        LevelManager.Instance.gameBoardWindow = this;
        LevelManager.Instance.OnObjectSelect += (pastO, currO) =>
        {
            if (currO != null && currO.TryGetComponent(out UnitController unit) && pastO != currO)
            {
                var closeTiles = GetCloseTile(unit.occupiedTile, unit.rad);
                foreach (var tile in generatedTiles.Select(tile => tile.GetComponent<Tile>()))
                {
                    tile.HideTargets();
                }
                foreach (var closeTile in closeTiles)
                {
                    closeTile.ShowBlueTarget();
                }

            }
        };
        
        GenerateBoard();
        UnlockAllTile();
        CreateHome();
    }

    public List<GameObject> GetAllTile()
    {
        return generatedTiles;
    }
    
    public List<Tile> GetCloseTile(Tile tile, int radius)
    {
        var closeTiles = new List<Tile>();
        var allTiles = generatedTiles.Select(til => til.GetComponent<Tile>()).ToList();

        var tilePos = tile.pos;

        var minX = tilePos.x - radius;
        var minY = tilePos.y - radius;
        var maxX = tilePos.x + radius;
        var maxY = tilePos.y + radius;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                var closeTile = allTiles.Find(til => til.pos == new Vector2Int(x, y));
                if(closeTile == null || closeTile.pos == tile.pos)
                    continue;
                closeTiles.Add(closeTile);
                Debug.Log(closeTile.pos);
            }
        }
        
        return closeTiles;
    }

    [Button()]
    private void GenerateBoard()
    {
        if(generatedTiles != null) RemoveBoard();
        generatedTiles ??= new List<GameObject>();
        var width = (int)(mapSize / Mathf.Sqrt(mapSize));
        for (var i = width; i > 0; i--)
        {
            for (var j = 0; j < width; j++)
            {
                var tileRectTransform = Instantiate(tilePrefab, tileParent);
                
                tileRectTransform.anchoredPosition = new Vector2((i + j) * 50f, (i - j) * 31.5f);
                var ja = j + 1;
                tileRectTransform.name = i + ", " + ja;
                var tile = tileRectTransform.GetComponent<Tile>();
                tile.pos = new Vector2Int(i, ja);
                generatedTiles.Add(tileRectTransform.gameObject);
                
                SubscribeOnTile(tile);
            }
        }
    }
    
    [Button()]
    private void RemoveBoard()
    {
        for (var i = 0; i < generatedTiles.Count; i++)
        {
            DestroyImmediate(generatedTiles[i]);
        }
        
        generatedTiles.Clear();
    }
    
    [Button()]
    private void UnlockAllTile()
    {
        foreach (var tile in generatedTiles.Select(tile => tile.GetComponent<Tile>()))
        {
            tile.UnlockTile();
        }
    }
    [Button()]
    private void CreateHome()
    {
        var randomTile = generatedTiles[Random.Range(0, generatedTiles.Count)];
        var home = Instantiate(homePrefab, randomTile.transform);
        var tile = randomTile.GetComponent<Tile>();
        home.transform.position = tile.GetEnvironmentRectTransform().position;
        tile.BuildHome();
    }

    private void SubscribeOnTile(Tile tile)
    {
        tile.OnClickOnTile += ShowTileInfo;
    }

    private void ShowTileInfo(Tile tile)
    {
        //LevelManager.Instance.gameplayWindow.SetTileName(tile.GetTileName());
    }
}