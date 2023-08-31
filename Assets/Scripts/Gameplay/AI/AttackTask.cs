using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class AttackTask : BaseTask
{
    private Sequence _attackSeq;

    private void OnDestroy()
    {
        _attackSeq.Kill();
    }

    public override int CalculatePriority(List<UnitController> units)
    {
        var lastPr = taskPriority;
        if (IsEnemyNearby(units))
        {
            taskPriority = 5;
            if (taskPriority != lastPr)
            {
                
            }
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
        if (unitForAttack != null) _attackSeq.Append(unit.AttackUnitOnTile(unitForAttack));
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

        closeUnits.RemoveAll(unite => unite == null);
        closeUnits.RemoveAll(unite => unite.GetOwner().owner == unit.GetOwner().owner);
        closeUnits.RemoveAll(unite => unite.GetOwner().owner.CheckAlly(unit.GetOwner().owner));
        if (closeTile.Count == 0)
            return null;
        var minHp = closeUnits.OrderBy(unite => unite.GetHp()).ToList();
        var unitForAttack = closeUnits.FirstOrDefault(unite => unite.GetHp() == minHp.First().GetHp());

        return unitForAttack;
    }
    
    private bool IsEnemyNearby(List<UnitController> units)
    {
        foreach (var unit in units)
        {
            var owner = unit.GetOwner().owner;

            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile,
                unit.GetUnitInfo().rad);
            var allClosetUnits = tiles.FindAll(tile => tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner);
            if (allClosetUnits.Count > 0)
            {
                if(allClosetUnits.Count > 1) AddPointOfInteresting(unit.occupiedTile.pos);

                var allNonFriendlyUnitTiles = tiles.FindAll(tile => tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner);
                var relationTypes = allNonFriendlyUnitTiles.Select(tile => tile.unitOnTile.GetOwner().owner.GetRelation(unit.GetOwner().owner)).ToList();
                
                if (relationTypes.Contains(DiplomacyManager.RelationType.War))
                {
                    return true;
                }

                if (relationTypes.Contains(DiplomacyManager.RelationType.Neutral))
                { 
                    return true;
                }
            }
        }

        return false;
    }
}
