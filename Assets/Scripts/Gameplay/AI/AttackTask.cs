using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class AttackTask : BaseTask
{
    private Sequence _attackSeq;

    public override int CalculatePriority(List<UnitController> units)
    {
        if (IsEnemyNearby(units))
        {
            taskPriority = 5;
            return taskPriority;
        }
        
        taskPriority = -1;
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
        
        ExploreCloseTile(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }
    
    private Tween ExploreCloseTile(UnitController unit)
    {
        _attackSeq = DOTween.Sequence();
        var unitForAttack = ChooseUnitForAttack(unit);
        _attackSeq.Append(unit.AttackUnitOnTile(unitForAttack));
        return _attackSeq;
    }
    
    private UnitController ChooseUnitForAttack(UnitController unit)
    {
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().rad);
        var unitHasMountTech = unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain);
        var closeUnits = new List<UnitController>();
        
        closeTile.RemoveAll(tile => tile == null);
        
        foreach (var tile in closeTile)
        {
            if(tile.IsTileFree())
                continue;
            
            closeUnits.Add(tile.unitOnTile);
        }

        closeUnits.RemoveAll(unite => unite.GetOwner().owner == unit.GetOwner().owner);
        
        var minDef = closeUnits.Min(unite => unite.GetUnitInfo().def);
        var unitForAttack = closeUnits.FirstOrDefault(unite => unite.GetUnitInfo().def == minDef);

        return unitForAttack;
    }
    
    private bool IsEnemyNearby(List<UnitController> units)
    {
        foreach (var unit in units)
        {
            var owner = unit.GetOwner().owner;

            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile,
                unit.GetUnitInfo().rad);
            if (tiles.Any(tile => tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner))
            {
                return true;
            }
        }

        return false;
    }
}
