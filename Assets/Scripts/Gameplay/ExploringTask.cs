using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class ExploringTask : BaseTask
{
    private Sequence _exploreSeq;

    public override int CalculatePriority()
    {
        TaskPriority++;
        return base.CalculatePriority();
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
        
        ExploreCloseTile(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }
    
    private Tween ExploreCloseTile(UnitController unit)
    {
        _exploreSeq = DOTween.Sequence();
        var tile = ChooseTileForExploring(unit);
        _exploreSeq.Append(unit.MoveToTile(tile));
        return _exploreSeq;
    }

    private Tile ChooseTileForExploring(UnitController unit)
    {
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, 1);
        var unitHasMountTech = unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain);
        
        closeTile.RemoveAll(tile => tile == null);
        closeTile.RemoveAll(tile => !tile.IsTileFree());
        closeTile.RemoveAll(tile => tile.tileType == Tile.TileType.Water);
        closeTile.RemoveAll(tile => tile.isHasMountain && !unitHasMountTech);
        closeTile.RemoveAll(tile => tile == null);
        
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
                //tile
            }
        }

        return true;
    }
}
