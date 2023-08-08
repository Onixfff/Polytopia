using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;

public class PatrolTask : BaseTask
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
        _exploreSeq.Append(unit.MoveToTile(tile));
        return _exploreSeq;
    }
    
    private Tile ChooseTileForPatrol(UnitController unit)
    {
        var tile = unit.GetOwner().homeTile;
        var unitTile = unit.occupiedTile;
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(tile, 2);
        var unitHasMountTech = tile.homeOnTile.owner.technologies.Contains(TechInfo.Technology.Mountain);
 
        closeTile.RemoveAll(tile => tile == null);
        closeTile.RemoveAll(tile => !tile.IsTileFree());
        closeTile.RemoveAll(tile => tile.tileType == Tile.TileType.Water);
        closeTile.RemoveAll(tile => tile.isHasMountain && !unitHasMountTech);
        if (closeTile.Count == 0)
            return tile;
        
        
        var index = closeTile.IndexOf(closeTile.Find(tile1 => tile1.pos == unitTile.pos));

        if (index >= closeTile.Count)
        {
            return closeTile[0];
        }

        return closeTile[index + 1];
    }
    
    private bool CheckEnemyWithUnit()
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

                if (tile.homeOnTile != null && tile.homeOnTile.owner != unit.GetOwner().owner)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
