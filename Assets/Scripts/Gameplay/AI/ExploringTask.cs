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
        if (CheckInterestingPlace())
        {
            EndTask();
            return;
        }
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
        
        ExploreCloseTile(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }
    
    private Tween ExploreCloseTile(UnitController unit)
    {
        _exploreSeq = DOTween.Sequence();
        var tile = FindPath(unit);
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        _exploreSeq.Append(unit.MoveToTile(tile, dur));
        return _exploreSeq;
    }

    private Tile FindPath(UnitController unit)
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
            return unit.occupiedTile;
        
        return allTiles[path[0]];
    }

    private bool CheckInterestingPlace()
    {
        foreach (var unit in UnitsAssignedToTheTask)
        {
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().rad);
            foreach (var tile in tiles)
            {
                if (tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                {
                    TryAddPointOfInteresting(tile.pos);
                    return true;
                }

                if (tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != unit.GetOwner().owner)
                {
                    TryAddPointOfInteresting(tile.pos);
                    return true;
                }
            }
        }

        return false;
    }
}
