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
                taskPriority = 4;
                break;
            }
            
            taskPriority = -1;
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
            if (_isHomeCapture)
            {
                EndTask();
                return;
            }
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
        if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != unit.GetOwner().owner)
        {
            unit.occupiedTile.homeOnTile.OccupyHome();
            _isHomeCapture = true;
            return _captureSeq;
        }
        var tile = ChooseHomeForCapture(unit);
        if (tile == null)
            return _captureSeq;
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        _captureSeq.Append(unit.MoveToTile(tile, dur));
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
            return true;
        
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);
        foreach (var tile in tiles)
        {
            if (tile.homeOnTile != null && tile.homeOnTile.owner != owner)
            {
                return true;
            }
        }

        return false;
    }
}
