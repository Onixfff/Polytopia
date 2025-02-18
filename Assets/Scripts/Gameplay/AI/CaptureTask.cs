using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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
                taskPriority = 5;
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
            EndTask();
            //EndTurn();
            return;
        }
        
        TaskSeq = DOTween.Sequence();
        TaskSeq.Join(CaptureCloseHome(units[i]));
        TaskSeq.OnComplete((() =>
        {
            TaskSeq = null;
            if(FuseSeq == null)
                return;
            UnitAction(units, i+1);
        }));
        
        var inValX = 0f;
        FuseSeq = DOTween.Sequence();
        FuseSeq.Join(DOTween.To(() => inValX, x => inValX = x, 1, 1f));
        FuseSeq.OnComplete((() =>
        {
            FuseSeq = null;
            if(TaskSeq == null)
                return;
            UnitAction(units, i+1);
        }));
    }
    
    private Tween CaptureCloseHome(UnitController unit)
    {
        _captureSeq = DOTween.Sequence();
        if (unit.occupiedTile.GetHomeOnTile() != null && unit.occupiedTile.GetHomeOnTile().owner != unit.GetOwner().owner)
        {
            TryRemovePointOfInteresting(unit.occupiedTile.pos);
            unit.occupiedTile.GetHomeOnTile().OccupyHome();
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
        closeTile.RemoveAll(tile => tile.GetHomeOnTile() == null);

        if (closeTile.Any(tile => tile.GetHomeOnTile().owner == null))
        {
            return closeTile.Find(tile => tile.GetHomeOnTile().owner == null);
        }
        var home = closeTile.Find(tile => tile.GetHomeOnTile().owner != owner && !tile.GetHomeOnTile().owner.CheckAlly(owner));
        return home;
    }
    
    private bool CheckInterestingPlace(UnitController unit)
    {
        var owner = unit.GetOwner().owner;
        if (unit.occupiedTile.GetHomeOnTile() != null && unit.occupiedTile.GetHomeOnTile().owner != owner)
            return true;
        
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);
        foreach (var tile in tiles)
        {
            if(!tile.IsTileFree())
                continue;
            
            
            if (tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != owner)
            {
                if (tile.GetHomeOnTile().owner == null)
                    return true;
                var isAlly = tile.GetHomeOnTile().owner.CheckAlly(owner);
                if(!isAlly)
                    return true;
            }
        }

        return false;
    }
}
