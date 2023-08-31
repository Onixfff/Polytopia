using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;

public static class AStarAlgorithm
{
    public static List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int targetPosition, UnitController unit = null)
    {
        var tiles = LevelManager.Instance.gameBoardWindow.GetAllTile();
        //var tiles = AstarTest.Instance.GeneratedTiles;
        foreach (var tile in tiles.Keys.ToList().Select(vector2Int => tiles[vector2Int]))
        { 
            tile.previousTile = null;
            tile.fCost = float.PositiveInfinity;
            tile.gCost = float.PositiveInfinity;
            tile.hCost = float.PositiveInfinity;
        }
        var startTile = tiles[startPosition];
        var targetTile = tiles[targetPosition];

        var openList = new List<Tile>();
        var closedList = new List<Tile>();

        startTile.CalculateCost(startTile, targetTile);
        openList.Add(startTile);
        
        while (openList.Count > 0)
        {
            var currentTile = GetLowestFCostTile(openList);

            if (currentTile == targetTile)
            {
                var path = new List<Vector2Int>();
                while (currentTile.previousTile != null)
                {
                    path.Add(currentTile.pos);
                    currentTile = currentTile.previousTile;
                }
                path.Reverse();
                return path;
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (var neighborPosition in GetNeighborPositions(currentTile.pos))
            {
                if (!tiles.ContainsKey(neighborPosition) || closedList.Contains(tiles[neighborPosition]))
                    continue;
                
                if(tiles[neighborPosition].obstacle)
                    continue;

                if (neighborPosition != targetPosition)
                {
                    if (unit != null)
                    {
                        if(!tiles[neighborPosition].IsTileFree() && tiles[neighborPosition].unitOnTile.GetOwner().owner == unit.GetOwner().owner)
                            continue;
                        if(tiles[neighborPosition].isHasMountain && !unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain))
                            continue;
                        if(tiles[neighborPosition].tileType == Tile.TileType.Water && !unit.GetUnitInfo().abilityTypes.Contains(UnitInfo.AbilityType.Float))
                            continue;
                    }
                    else
                    {
                        if(tiles[neighborPosition].isHasMountain && !LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(TechInfo.Technology.Mountain))
                            continue;
                        if(tiles[neighborPosition].tileType is Tile.TileType.Water or Tile.TileType.DeepWater)
                            continue;
                    }
                }
                
                var neighborTile = tiles[neighborPosition];
                var newGCost = currentTile.gCost + 1;

                if (newGCost < neighborTile.gCost)
                {
                    neighborTile.previousTile = currentTile;
                    neighborTile.gCost = newGCost;
                    neighborTile.CalculateCost(startTile, targetTile);

                    if (!openList.Contains(neighborTile))
                    {
                        openList.Add(neighborTile);
                    }
                }
            }
        }

        return null;
    }

    private static Tile GetLowestFCostTile(List<Tile> tileList)
    {
        var lowestFCostTile = tileList[0];
        for (var i = 1; i < tileList.Count; i++)
        {
            if (tileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tileList[i];
            }
        }
        return lowestFCostTile;
    }

    private static List<Vector2Int> GetNeighborPositions(Vector2Int position)
    {
        var neighborPositions = new List<Vector2Int>
        {
            new Vector2Int(position.x - 1, position.y), // left
            new Vector2Int(position.x + 1, position.y), // right
            new Vector2Int(position.x, position.y - 1), // up
            new Vector2Int(position.x, position.y + 1), // down
            new Vector2Int(position.x - 1, position.y - 1), // 
            new Vector2Int(position.x + 1, position.y - 1), // 
            new Vector2Int(position.x + 1, position.y + 1), // 
            new Vector2Int(position.x - 1, position.y + 1) // 
        };
        return neighborPositions;
    }
}
