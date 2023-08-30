using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class PatrolTask : BaseTask
{
    private Sequence _exploreSeq;

    public override int CalculatePriority(List<UnitController> units)
    {
        if (CheckEnemyWithUnit())
        {
            taskPriority = 0;
            return taskPriority;
        }
        taskPriority = 1;
        return taskPriority;
    }

    protected override void TaskRealisation()
    {
        UnitAction(UnitsAssignedToTheTask, 0);
    }

    protected override void UnitAction(List<UnitController> units, int i)
    {
        if (CheckEnemyWithUnit())
        {
            EndTask();
            return;
        }
        if (units.Count <= i)
        {
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
        var tile = ChooseTileForPatrol(unit);
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        _exploreSeq.Append(unit.MoveToTile(tile, dur));
        return _exploreSeq;
    }
    
    private Tile ChooseTileForPatrol(UnitController unit)
    {
        var tile = unit.GetOwner().homeTile;
        var unitTile = unit.occupiedTile;
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(tile, 2);
        var unitHasMountTech = tile.GetHomeOnTile().owner.technologies.Contains(TechInfo.Technology.Mountain);
        
        closeTile.RemoveAll(tile => tile == null);
        closeTile.RemoveAll(tile => !tile.IsTileFree());
        closeTile.RemoveAll(tile => tile.tileType == Tile.TileType.Water);
        closeTile.RemoveAll(tile => tile.isHasMountain && !unitHasMountTech);
        if (closeTile.Count == 0)
            return tile;
        
        var closeUnitTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unitTile, 1);
        var randTile = closeUnitTile[Random.Range(0, closeUnitTile.Count)];
        
        var i = 0;
        while (!closeTile.Contains(randTile))
        {
            i++;
            randTile = closeUnitTile[Random.Range(0, closeUnitTile.Count)];
            if (i > 100)
            {
                randTile = unitTile;
                break;
            }
        }
        
        return randTile;
    }
    
    private bool CheckEnemyWithUnit()
    {
        if (UnitsAssignedToTheTask == null) return false;
        foreach (var unit in UnitsAssignedToTheTask)
        {
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile,
                unit.GetUnitInfo().rad);
            foreach (var tile in tiles)
            {
                if (tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                {
                    return true;
                }

                if (tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != unit.GetOwner().owner)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
