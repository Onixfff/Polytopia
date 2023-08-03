using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CaptureTask : BaseTask
{
    private Sequence _captureSeq;
    private bool _isHomeCapture;

    public override int CalculatePriority(List<UnitController> units)
    {
        foreach (var unit in units)
        {
            if (CheckInterestingPlace(unit))
            {
                if (TaskPriority == -1)
                {
                    TaskPriority = 2;
                    break;
                }
                
                TaskPriority++;
            }
            else
            {
                if(TaskPriority > -1)
                    TaskPriority--;
            }
        }
        return base.CalculatePriority(units);
    }

    protected override void TaskRealisation()
    {
        UnitAction(UnitsAssignedToTheTask, 0);
    }

    protected override void UnitAction(List<UnitController> units, int i)
    {
        if (units.Count <= i)
        {
            if (_isHomeCapture)
            {
                Debug.Log("Capture");
                EndTask();
                return;
            }
            Debug.Log("PreCapture");
            EndTurn();
            return;
        }
        
        CaptureCloseHome(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
    }
    
    private Tween CaptureCloseHome(UnitController unit)
    {
        _captureSeq = DOTween.Sequence();
        Debug.Log("captureHome");
        if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != unit.GetOwner().owner)
        {
            unit.occupiedTile.homeOnTile.OccupyHome();
            _isHomeCapture = true;
            return _captureSeq;
        }
        var tile = ChooseHomeForCapture(unit);
        if (tile == null)
            return _captureSeq;
        _captureSeq.Append(unit.MoveToTile(tile));
        return _captureSeq;
    }
    
    private Tile ChooseHomeForCapture(UnitController unit)
    {
        var owner = unit.GetOwner().owner;
        
        
        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, 1);
        
        closeTile.RemoveAll(tile => tile == null);
        closeTile.RemoveAll(tile => !tile.IsTileFree());
        
        var home = closeTile.FirstOrDefault(tile => tile.homeOnTile != null && tile.homeOnTile.owner != unit.GetOwner().owner);
        return home;
    }
    
    private bool CheckInterestingPlace(UnitController unit)
    {
        var owner = unit.GetOwner().owner;
        if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != owner)
            return unit.occupiedTile;
        
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().rad);
        foreach (var tile in tiles)
        {
            if (tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner)
            {
                return true;
            }

            if (tile.homeOnTile != null && tile.homeOnTile.owner != owner)
            {
                return true;
            }
        }

        return false;
    }
}
