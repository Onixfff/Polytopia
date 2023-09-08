using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class ExploringTask : BaseTask
{
    private Sequence _exploreSeq;
    public override int CalculatePriority(List<UnitController> units)
    {
        taskPriority = 1;
        return taskPriority;
    }

    protected override void TaskRealisation()
    {
        UnitAction(UnitsAssignedToTheTask, 0);
    }

    protected override void UnitAction(List<UnitController> units, int i)
    {
        if (units.Count <= i)
        {
            if (CheckInterestingPlace())
            {
                EndTask();
                return;
            }
            EndTurn();
            return;
        }
        
        TaskSeq = DOTween.Sequence();
        TaskSeq.Join(ExploreCloseTile(units[i]));
        TaskSeq.OnComplete((() =>
        {
            TaskSeq = null;
            if(FuseSeq == null)
                return;
            UnitAction(units, i+1);
        }));
        
        var inValX = 0f;
        FuseSeq = DOTween.Sequence();
        FuseSeq.Join(DOTween.To(() => inValX, x => inValX = x, 1, 1f));
        FuseSeq.OnComplete((() =>
        {
            FuseSeq = null;
            if(TaskSeq == null)
                return;
            UnitAction(units, i+1);
        }));
    }
    
    private Tween ExploreCloseTile(UnitController unit)
    {
        _exploreSeq = DOTween.Sequence();
        var tile = FindPathToExplore(unit);
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        if (tile == unit.occupiedTile)
        {
            _exploreSeq.Append(unit.transform.DOShakeRotation(0.05f, 10));
            Debug.Log($"Task - {name} not found path");
            return _exploreSeq;
        }
        
        _exploreSeq.Append(unit.MoveToTile(tile, dur));
        return _exploreSeq;
    }

    private Tile FindPathToExplore(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var allTiles = board.GetAllTile();
        var path = new List<Vector2Int>();

        var neededRad = 1;

        for (var i = 0; i <= 6; i++)
        {
            if (board.GetCloseTile(unit.occupiedTile, neededRad).Any(tile =>
                    !TaskManager.civilisationController.GetTileInExploreList().Contains(tile)))
            {
                break;
            } 
            neededRad++;
        }
        
        foreach (var tile in board.GetCloseTile(unit.occupiedTile, neededRad))
        {
            if(tile.tileType is Tile.TileType.Water or Tile.TileType.DeepWater)
                continue;
            if(!tile.IsTileFree())
                continue;
            if(TaskManager.civilisationController.GetTileInExploreList().Contains(tile))
                continue;
            
            var findPath = AStarAlgorithm.FindPath(unit.occupiedTile.pos, tile.pos, unit);
            
            if(findPath == null)
                continue;
            
            if (path.Count == 0)
                path = findPath;

            if (path.Count < findPath.Count)
            {
                path = findPath;
            }
        }

        if (path.Count == 0 || !allTiles[path[0]].IsTileFree())
            return FindPathToPoint(unit);
        
        if (unit.GetUnitInfo().moveRad == 2)
        {
            if (path.Count < 2 || !allTiles[path[1]].IsTileFree())
                return allTiles[path[0]];
        
            return allTiles[path[1]];
        }
        
        if (unit.GetUnitInfo().moveRad == 3)
        {
            if (path.Count < 2 || !allTiles[path[1]].IsTileFree())
                return allTiles[path[0]];
            
            if (path.Count < 3 || !allTiles[path[2]].IsTileFree())
                return allTiles[path[1]];
        
            return allTiles[path[2]];
        }
        
        return allTiles[path[0]];
    }

    private Tile FindPathToPoint(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var allTiles = board.GetAllTile();
        var points = TaskManager.pointsOfInteresting;
        var path = new List<Vector2Int>();
        
        foreach (var point in points)
        {
            var findPath = AStarAlgorithm.FindPath(unit.occupiedTile.pos, point, unit);
            if(findPath == null)
                continue;
            if(findPath.Count > 3)
                continue;
            
            if (path.Count == 0)
                path = findPath;

            if (path.Count < findPath.Count)
            {
                path = findPath;
            }
        }
        
        if (path.Count == 0 || !allTiles[path[0]].IsTileFree())
            return GoToRandomTile(unit);
        
        return allTiles[path[0]];
    }

    private Tile GoToRandomTile(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var tiles = board.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);

        foreach (var tile in tiles)
        {
            if(!tile.IsTileFree() && tile.unitOnTile.GetOwner().owner == unit.GetOwner().owner)
                continue;
            if(!tile.IsTileFree() && tile.unitOnTile.GetOwner().owner.CheckAlly(unit.GetOwner().owner))
                continue;
            if(tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != null && tile.GetHomeOnTile().owner.CheckAlly(unit.GetOwner().owner))
                continue;
            if(tile.isHasMountain && !unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain))
                continue;
            if(tile.tileType == Tile.TileType.Water && (!unit.GetUnitInfo().abilityTypes.Contains(UnitInfo.AbilityType.Float) || !tile.IsTileHasPort()))
                continue;
            if(tile.tileType == Tile.TileType.DeepWater && !unit.GetUnitInfo().abilityTypes.Contains(UnitInfo.AbilityType.Float))
                continue;

            return tile;
        }

        return unit.occupiedTile;
    }

    
    private bool CheckInterestingPlace()
    {
        foreach (var unit in UnitsAssignedToTheTask)
        {
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, 1);
            foreach (var tile in tiles)
            {
                if (tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                { 
                    TryAddPointOfInteresting(tile.pos);
                    return true;
                }

                if (tile.GetOwner() != null && tile.GetOwner().owner != unit.GetOwner().owner)
                {
                    if(tile.GetOwner().owner != null)
                        TryAddPointOfInteresting(tile.pos);
                }
                if (tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != unit.GetOwner().owner)
                {
                    if(tile.GetHomeOnTile().owner != null)
                        TryAddPointOfInteresting(tile.pos);
                    return true;
                }
            }
        }

        return false;
    }
}
