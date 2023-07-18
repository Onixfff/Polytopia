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
    
    [Button()]
    private void GenerateBoard()
    {
        generatedTiles ??= new List<GameObject>();
        var width = mapSize / Mathf.Sqrt(mapSize);
        for (var i = width; i > 0; i--)
        {
            for (var j = 0; j < width; j++)
            {
                var tile = Instantiate(tilePrefab, tileParent);
                
                tile.anchoredPosition = new Vector2((i + j) * 50f, (i - j) * 31.5f);
                tile.name = i + ", " + j;
                generatedTiles.Add(tile.gameObject);
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
    private void GenerateDecoration()
    {
        foreach (var tile in generatedTiles.Select(tile => tile.GetComponent<Tile>()))
        {
            tile.GetDecoration();
        }
    }
}