using System;
using System.Collections.Generic;
using UnityEngine;

public class Roads : MonoBehaviour
{
    [SerializeField] private List<GameObject> roads;
    [SerializeField] private Tile tile;
    public bool isRoad;

    public void CheckCloseTile()
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var closeTiles = board.GetCloseTile(tile, 1);
        var tiles = new List<Tile>();
        foreach(var closeTile in closeTiles)
        {
            if (closeTile.roads.isRoad)
            {
                tiles.Add(closeTile);
                var deltaPos = closeTile.pos - tile.pos;
                closeTile.roads.ShowRoad(-deltaPos);
            }
            else if (closeTile.GetHomeOnTile() != null)
            {
                tiles.Add(closeTile);
            }
        }

        if (tiles.Count != 0)
        {
            foreach (var til in tiles)
            {
                var deltaPos = til.pos - tile.pos;
                ShowRoad(deltaPos);
            }
        }
        else
        {
            ShowRoad(Vector2Int.left);
        }
        
        if(tile.GetOwner() != null)
            tile.GetOwner().owner.roadManager.CheckConnectivityToTheCapital(tile);
    }

    public void ShowRoad(Vector2Int pos)
    {
        if (pos == Vector2Int.left)
        {
            roads[0].SetActive(true);
        }
        if (pos == Vector2Int.right)
        {
            roads[1].SetActive(true);
        }
        if (pos == Vector2Int.up)
        {
            roads[2].SetActive(true);
        }
        if (pos == Vector2Int.down)
        {
            roads[3].SetActive(true);
        }
        if (pos == new Vector2Int(-1,1))
        {
            roads[4].SetActive(true);
        }
        if (pos == new Vector2Int(1,-1))
        {
            roads[5].SetActive(true);
        }
        if (pos == new Vector2Int(-1,-1))
        {
            roads[6].SetActive(true);
        }
        if (pos == new Vector2Int(1,1))
        {
            roads[7].SetActive(true);
        }
    }
}
