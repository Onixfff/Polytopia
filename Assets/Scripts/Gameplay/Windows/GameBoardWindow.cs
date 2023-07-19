using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class GameBoardWindow : BaseWindow
{
    [SerializeField] private List<GameObject> boardPrefabs;

    [SerializeField] private List<GameObject> generatedTiles;
    [SerializeField] private RectTransform tilePrefab;
    [SerializeField] private RectTransform tileParent;

    [SerializeField] private int mapSize = 16;
    [SerializeField] private float heightMultiplier = 0.01f;
    [SerializeField] private GameObject homePrefab;
    public List<GameObject> GetAllTile()
    {
        return generatedTiles;
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

    private void ShowTileInfo(string tileName)
    {
        Debug.Log(tileName);        
    }
}