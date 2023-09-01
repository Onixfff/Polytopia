using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoveToPointOfInterestTask : BaseTask
{
    private Sequence _moveSeq;

    public override int CalculatePriority(List<UnitController> units)
    {
        if (TaskManager.pointsOfInteresting.Count == 0)
        {
            taskPriority = 1;
        }
        else
        {
            taskPriority = 3;
        }
            
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
            EndTask();
            return;
        }
        
        MoveToNearbyPointOfInterest(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }

    private Tween MoveToNearbyPointOfInterest(UnitController unit)
    {
        _moveSeq = DOTween.Sequence();

        _moveSeq.Append(unit.MoveToTile(FindPath(unit)));
        
        return _moveSeq;
    }
    
    private Tile FindPath(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var allTiles = board.GetAllTile();
        var points = TaskManager.pointsOfInteresting;
        var path = new List<Vector2Int>();
        
        foreach (var point in points)
        {
            var findPath = AStarAlgorithm.FindPath(unit.occupiedTile.pos, point, unit);
            if(findPath.Count > 3)
                continue;
            
            if (path.Count == 0)
                path = findPath;

            if (path.Count < findPath.Count)
            {
                path = findPath;
            }
        }
        
        if (allTiles.Count == 0 || !allTiles[path[0]].IsTileFree())
            return unit.occupiedTile;
        
        return allTiles[path[0]];
    }
}
