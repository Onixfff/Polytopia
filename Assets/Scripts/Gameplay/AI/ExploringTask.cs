using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
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
        var gameBoard = LevelManager.Instance.gameBoardWindow;
        var unitInfo = unit.GetUnitInfo();
        var unitHasMountTech = unit.GetOwner().owner.technologies.Contains(TechInfo.Technology.Mountain);
        var occupiedTile = unit.occupiedTile;
        var tileForMove = new List<Tile>();
        if (occupiedTile.tileType == Tile.TileType.Ground)
        {
            foreach (var close in gameBoard.GetCloseTile(occupiedTile, 1))
            {
                if(close.isHasMountain && !unitHasMountTech)
                    continue;
                
                if(close.tileType == Tile.TileType.Water || close.tileType == Tile.TileType.DeepWater)
                    continue;
                
                tileForMove.Add(close);
                if(close.isHasMountain || close.isHasTree)
                    continue;
                if(unitInfo.moveRad == 1)
                    continue;
            
                var closeTile = gameBoard.GetCloseTile(close, 1);
                foreach (var tile in closeTile)
                {
                    if (tile.isHasMountain && !unitHasMountTech)
                        continue;

                    tileForMove.Add(tile);
                    if(tile.isHasMountain || tile.isHasTree || tile.tileType == Tile.TileType.Water || tile.tileType == Tile.TileType.DeepWater)
                        continue;
                    if(unitInfo.moveRad < 3)
                        continue;
                    var clTile = gameBoard.GetCloseTile(tile, 1);
                    foreach (var cTile in clTile)
                    {
                        if(cTile.isHasMountain && !unitHasMountTech)
                            continue;
            
                        tileForMove.Add(cTile);
                    }
                }
            }
        }
        else
        {
            foreach (var close in gameBoard.GetCloseTile(occupiedTile, 1))
            {
                if(close.isHasMountain && !unitHasMountTech)
                    continue;
            
                tileForMove.Add(close);
                if(close.tileType == Tile.TileType.Ground)
                    continue;
                if(unitInfo.moveRad == 1)
                    continue;
            
                var closeTile = gameBoard.GetCloseTile(close, 1);
                foreach (var tile in closeTile)
                {
                    if(tile.isHasMountain && !unitHasMountTech)
                        continue;
            
                    tileForMove.Add(tile);
                    if(close.tileType == Tile.TileType.Ground)
                        continue;
                    if(unitInfo.moveRad < 3)
                        continue;
                    
                    var clTile = gameBoard.GetCloseTile(tile, 1);
                    foreach (var cTile in clTile)
                    {
                        if(cTile.isHasMountain && !unitHasMountTech)
                            continue;
            
                        tileForMove.Add(cTile);
                    }
                }
            }
        }
        
        tileForMove.RemoveAll(tile => !tile.IsTileFree());
        
        if (unit.aiFromTile == null || unit.occupiedTile == unit.aiFromTile)
        {
            if (tileForMove.Count == 0)
                return unit.occupiedTile;
            return gameBoard.GetTile(tileForMove[Random.Range(0, tileForMove.Count)].pos);
        }
        var forwardDir = (unit.occupiedTile.pos - unit.aiFromTile.pos) + unit.occupiedTile.pos;
        var nextTile = gameBoard.GetTile(forwardDir);
        if(tileForMove.Contains(nextTile))
            return nextTile;

        if (tileForMove.Count == 0)
            return unit.occupiedTile;
        return gameBoard.GetTile(tileForMove[Random.Range(0, tileForMove.Count)].pos);
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
                    AddPointOfInteresting(tile.pos);
                    return true;
                }
            }
        }

        return false;
    }
}
