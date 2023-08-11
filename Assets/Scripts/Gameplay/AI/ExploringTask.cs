using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using NaughtyAttributes;
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
        var tile = ChooseTileForExploring(unit);
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        _exploreSeq.Append(unit.MoveToTile(tile, dur));
        return _exploreSeq;
    }

    private Tile ChooseTileForExploring(UnitController unit)
    {
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);
        var unitHasMountTech = unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain);
        
        closeTile.RemoveAll(tile => tile == null);
        closeTile.RemoveAll(tile => !tile.IsTileFree());
        closeTile.RemoveAll(tile => tile.tileType == Tile.TileType.Water);
        closeTile.RemoveAll(tile => tile.isHasMountain && !unitHasMountTech);
        
        if (unit.aiFromTile == null || unit.occupiedTile == unit.aiFromTile)
        {
            if (closeTile.Count == 0)
                return unit.occupiedTile;
            return LevelManager.Instance.gameBoardWindow.GetTile(closeTile[Random.Range(0, closeTile.Count)].pos);
        }
        var forwardDir = (unit.occupiedTile.pos - unit.aiFromTile.pos) + unit.occupiedTile.pos;
        var nextTile = LevelManager.Instance.gameBoardWindow.GetTile(forwardDir);
        if(closeTile.Contains(nextTile))
            return nextTile;

        if (closeTile.Count == 0)
            return unit.occupiedTile;
        return LevelManager.Instance.gameBoardWindow.GetTile(closeTile[Random.Range(0, closeTile.Count)].pos);
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
