using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SendTroopsTask : BaseTask
{
    private Sequence _captureSeq;
    private bool _isEnemyFind;
    public override int CalculatePriority(List<UnitController> units)
    {
        foreach (var unit in units)
        {
            var owner = unit.GetOwner().owner;
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, Mathf.Max(unit.GetUnitInfo().rad, unit.GetUnitInfo().moveRad));
            
            
            if (tiles.Any(tile => (tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner) || (tile.homeOnTile != null && tile.homeOnTile.owner != null && tile.homeOnTile.owner != owner)))
            {
                //taskPriority = 4;
            }
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
        
        SendUnit(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }
    
    private Tween SendUnit(UnitController unit)
    {
        _captureSeq = DOTween.Sequence();
        var tile = FindPath(unit);
        _captureSeq.Append(unit.MoveToTile(tile));
        return _captureSeq;
    }

    private Tile FindPath(UnitController unit)
    {
        var civHomes = LevelManager.Instance.gameBoardWindow.playerCiv.homes;
        var path = SearchPath.Instance.GetPath(unit.occupiedTile, civHomes[Random.Range(0, civHomes.Count)].homeTile);
        return path[0];
    }
}
