using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int aiNumber;

    private CivilisationController _controller;
    private Sequence _unitsSeq;
    private Sequence _unitsActionSeq;
    private List<UnitController> _allUnits;
    public AIQuestGiver aiQuestGiver;
    
    private void Start()
    {
        _controller = GetComponentInParent<CivilisationController>();
    }

    public void StartTurn()
    {
        var homes = _controller.GetAllHome();

        var random = new System.Random();
        var type = typeof(TechInfo.Technology);
        var values = type.GetEnumValues();
        var techIndex = random.Next(values.Length);
        var tech = (TechInfo.Technology)values.GetValue(techIndex);
        _controller.technologies.Add(tech);
        homes.RemoveAll(home => home.owner != _controller);

        /*foreach (var home in homes)
        {
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(home.homeTile, home.homeRad);
            foreach (var tile in tiles)
            {
                if(tile.BuyAnimal())
                    break;
                if(tile.BuyFruit())
                    break;
            }
        }*/

        _allUnits ??= new List<UnitController>();
        foreach (var home in homes)
        {
            foreach (var unit in home.GetUnitList())
            {
                if(!_allUnits.Contains(unit) && unit.GetOwner().owner == _controller)
                    _allUnits.Add(unit);
            }
        }

        _allUnits.RemoveAll(unit => unit == null);
        if (_allUnits.Count != 0) 
            UnitAction(_allUnits);
        foreach (var home in homes)
        {
            if(LevelManager.Instance.currentTurn % 2 == 0)
                home.AIBuyUnit(_controller);
        }

        if (_allUnits.Count == 0)
        {
            var val = 0;
            DOTween.To(() => val, x => x = val, 1, 0.01f).OnComplete(EndTurn);
        }
    }

    private void EndTurn()
    {
        AIController.Instance.OnAITurnEnded?.Invoke(aiNumber+1);
    }

    private void UnitAction(List<UnitController> units)
    {
        aiQuestGiver.AddUnitsToList(units);
        aiQuestGiver.AssignTasks();
        aiQuestGiver.OnTaskAreDistributed += EndTurn;
    }
    
    private Tween UnitPriority(UnitController unit)
    {
        _unitsActionSeq = DOTween.Sequence();
        
        var tilesForAttack = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().rad);
        var tilesForMove = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);
        var targetForAttack = new List<UnitController>();
        var tileForMove = new List<Tile>();
        
        foreach (var att in tilesForAttack)
        {
            if (!att.IsTileFree())
            {
                if (att.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                {
                    targetForAttack.Add(att.unitOnTile);
                }
            }
        }

        foreach (var move in tilesForMove)
        {
            if (move.IsTileFree() && move.tileType == Tile.TileType.Ground)
            {
                tileForMove.Add(move);
            }
        }

        tileForMove.Remove(unit.aiFromTile);

        if (unit.occupiedTile.GetHomeOnTile() != null && unit.occupiedTile.GetHomeOnTile().owner != unit.GetOwner().owner)
        {
            var inVal = 0f;
            _unitsActionSeq.Append(DOTween.To(() => inVal, x => x = inVal, 0f, 0.1f));
            OccupyVillage(unit.occupiedTile.GetHomeOnTile());
            return _unitsActionSeq;
        }
        
        if (tileForMove.Count > 0)
        {
            var tileWithHome = tileForMove.Find(tile => tile.GetHomeOnTile());

            if (tileWithHome != null && tileWithHome.GetHomeOnTile().owner != unit.GetOwner().owner)
            {
                if(!tileWithHome.IsTileFree() && tileWithHome.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                {
                    _unitsActionSeq.Append(unit.AttackUnitOnTile(tileWithHome.unitOnTile));    
                    return _unitsActionSeq;
                }
                
                if (tileWithHome.IsTileFree())
                {
                    var dur = 0f;
                    if (unit.occupiedTile.isOpened || tileWithHome.isOpened)
                        dur = 0.2f;
                    _unitsActionSeq.Append(unit.MoveToTile(tileWithHome, dur));
                    if (unit.occupiedTile.GetHomeOnTile() != null && unit.occupiedTile.GetHomeOnTile().owner != unit.GetOwner().owner)
                    {
                        OccupyVillage(unit.occupiedTile.GetHomeOnTile());
                    }
                 
                    return _unitsActionSeq;
                }
                
            }
        }
        
        if (targetForAttack.Count > 0)
        {
            _unitsActionSeq.Append(unit.AttackUnitOnTile(targetForAttack[Random.Range(0, targetForAttack.Count)]));
            return _unitsActionSeq;
        }

        if (tileForMove.Count > 0)
        {
            var dir = ChooseTileForMove(unit);
            if (dir == null)
                dir = unit.occupiedTile;
            var dur = 0f;
            if (unit.occupiedTile.isOpened || dir.isOpened)
                dur = 0.2f;
            _unitsActionSeq.Append(unit.MoveToTile(dir, dur));
        }
        
        return _unitsActionSeq;
    }

    private void OccupyVillage(Home home)
    {
        home.OccupyHome();
        LevelManager.Instance.gameBoardWindow.RemoveVillage(home);
    }

    private Tile ChooseTileForMove(UnitController unit)
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
        else
        {
            if (closeTile.Count == 0)
                return unit.occupiedTile;
            return LevelManager.Instance.gameBoardWindow.GetTile(closeTile[Random.Range(0, closeTile.Count)].pos);
        }
    }
    
    /*private void AIBuyTech(CivilisationController aiController, TechInfo.Technology tech)
    {
        aiController.technologies.Add(tech);
    }
    
    private void UnitsPerformAction(List<UnitController> units, int index)
    {
        if(units.Count < index)
            return;
        
        var homes = _controller.GetAllHome();
        /*units[index].MoveToTile(GetTileForMove(units[index])).OnComplete((() =>
        {
            UnitsPerformAction(units, index+1);
        }));#1#
    }
    
    private Tile GetTileForMove(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;

        if (unit.aiHomeExploring == null)
        {
            for (var i = 0; i <= 5; i++)
            {
                var village = board.FindRandomVillage(unit.occupiedTile);
                if (village == null)
                    return unit.occupiedTile;
                if (village.exploringUnit != null)
                {
                    village = board.FindRandomVillage(unit.occupiedTile);
                }
                else
                {
                    unit.aiHomeExploring = village;
                    unit.aiHomeExploring.exploringUnit = unit;
                    break;
                }
            }
        }

        if (unit.aiHomeExploring.homeTile.pos == unit.occupiedTile.pos)
        {
            OccupyVillage(unit.occupiedTile._homeOnTile);
            unit.aiHomeExploring.exploringUnit = null;
            unit.aiHomeExploring = null;
            return unit.occupiedTile;
        }
        else
        {
            var tile = SearchPath.Instance.GetPath(unit.occupiedTile, unit.aiHomeExploring.homeTile)[0];
            return tile;
        }
    }*/
}