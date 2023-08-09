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
                taskPriority = 3;
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
        var dur = 0f;
        if (unit.occupiedTile.isOpened || tile.isOpened)
            dur = 0.2f;
        _captureSeq.Append(unit.MoveToTile(tile, dur));
        return _captureSeq;
    }

    private Tile FindPath(UnitController unit)
    {
        var allTiles = LevelManager.Instance.gameBoardWindow.GetAllTile();
        var civHomes = LevelManager.Instance.gameBoardWindow.playerCiv.homes;
        if (civHomes == null || civHomes.Count == 0)
            return unit.occupiedTile;
        var index = Random.Range(0, civHomes.Count);
        var path = AStarAlgorithm.FindPath(unit.occupiedTile.pos, civHomes[index].homeTile.pos, unit);
        if (path == null || path.Count == 0)
            return unit.occupiedTile;
        if (!allTiles[path[0]].IsTileFree())
            return unit.occupiedTile;
        return allTiles[path[0]];
    }
}
