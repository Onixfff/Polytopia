using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AstarTest : Singleton<AstarTest>
{
    [SerializeField] private int mapSize = 16;
    [SerializeField] private int obstacle = 16;
    [SerializeField] private Transform tilePrefab;
    public Dictionary<Vector2Int ,Gameplay.Tile> GeneratedTiles;

    void Start()
    {
        GenerateBoard();
    }
    private void GenerateBoard()
    {
        GeneratedTiles ??= new Dictionary<Vector2Int, Gameplay.Tile>();
        var width = (int)(mapSize / Mathf.Sqrt(mapSize));
        for (var i = width; i > 0; i--)
        {
            for (var j = 0; j < width; j++)
            {
                var tileRectTransform = Instantiate(tilePrefab, transform);
                
                tileRectTransform.position = new Vector2(((i + j)) - Mathf.Sqrt(mapSize), (i - j));
                var ja = j + 1;
                tileRectTransform.name = i + ", " + ja;
                var tile = tileRectTransform.GetComponent<Gameplay.Tile>();
                
                tile.pos = new Vector2Int(i, ja);
                GeneratedTiles.Add(tile.pos, tile);
            }
        }

        for (var i = 0; i < obstacle; i++)
        {
            var allTiles = GeneratedTiles.Keys.ToList();
            var randPos = Random.Range(0, allTiles.Count);
            var tile = GeneratedTiles[allTiles[randPos]];
            tile.obstacle = true;
            tile.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }
    }

}
