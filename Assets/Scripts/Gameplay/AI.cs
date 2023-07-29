using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum PriorityEnum
    {
        OccupyCity,
        AttackEnemy,
        Scouting,
    }
    
    public int aiNumber;
    
    private CivilisationController _controller;
    private Sequence _unitsSeq;
    private Sequence _unitsActionSeq;
    
    private Vector2Int[] _directions = 
    {
        Vector2Int.up, 
        Vector2Int.right, 
        Vector2Int.down, 
        Vector2Int.left, 
        new Vector2Int(1, 1), 
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1), 
        new Vector2Int(1, -1)
    };

    private void Start()
    {
        _controller = GetComponent<CivilisationController>();
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

        foreach (var home in homes)
        {
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(home.homeTile, home.homeRad);
            foreach (var tile in tiles)
            {
                if(tile.BuyAnimal())
                    break;
                if(tile.BuyFruit())
                    break;
            }
        }

        var allUnits = new List<UnitController>();
        foreach (var home in homes)
        {
            allUnits.AddRange(home.GetUnitList());
        }

        UnitAction(allUnits, 0);

        foreach (var home in homes)
        {
            if(LevelManager.Instance.currentTurn % 2 == 0)
                home.AIBuyUnit();
        }
    }

    private void UnitAction(List<UnitController> units, int i)
    {
        if(units.Count < i)
            return;
        
        UnitPriority(units[i]).OnComplete(() =>
        {
            UnitAction(units, i+1);
        });
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

        if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != unit.GetOwner().owner)
        {
            OccupyVillage(unit.occupiedTile.homeOnTile);
        }
        
        if (tileForMove.Count > 0)
        {
            var tileWithHome = tileForMove.Find(tile => tile.homeOnTile);

            if (tileWithHome != null && tileWithHome.homeOnTile.owner != unit.GetOwner().owner)
            {
                if(!tileWithHome.IsTileFree() && tileWithHome.unitOnTile.GetOwner().owner != unit.GetOwner().owner)
                {
                    _unitsActionSeq.Append(unit.AttackUnitOnTile(tileWithHome.unitOnTile));    
                    return _unitsActionSeq;
                }
                
                if (tileWithHome.IsTileFree())
                {
                    _unitsActionSeq.Append(unit.MoveToTile(tileWithHome));
                    if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != unit.GetOwner().owner)
                    {
                        OccupyVillage(unit.occupiedTile.homeOnTile);
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
            var dir = ChooseMoveDirection(unit);
            var tile = LevelManager.Instance.gameBoardWindow.GetTile(dir);
            unit.MoveToTile(tile);
        }
        
        return _unitsActionSeq;
    }

    private void OccupyVillage(Home home)
    {
        home.OccupyHome();
        LevelManager.Instance.gameBoardWindow.RemoveVillage(home);
    }
    
    private void EndTurn()
    {
        AIController.Instance.OnAITurnEnded?.Invoke(aiNumber+1);
    }
    
    private Vector2Int ChooseMoveDirection(UnitController unit)
    {
        var randDir = Random.Range(0, _directions.Length);
        var dir = _directions[randDir] + unit.occupiedTile.pos;
        var tile = LevelManager.Instance.gameBoardWindow.GetTile(dir);
        if (tile != null && !tile.IsTileFree())
        {
            for (var i = 0; i < _directions.Length; i++)
            {
                dir = _directions[i];
                if (tile.IsTileFree())
                {
                    break;
                }

                if (i == _directions.Length - 1)
                {
                    dir = unit.occupiedTile.pos;
                    break;
                }
            }
        }

        if (unit.aiFromTile == null || unit.occupiedTile == unit.aiFromTile)
        {
            return dir;
        }

        var a = (unit.occupiedTile.pos - unit.aiFromTile.pos) + unit.occupiedTile.pos;
        var nextTile = LevelManager.Instance.gameBoardWindow.GetTile(a);
        
        if (nextTile == null || !nextTile.IsTileFree() || nextTile.tileType == Tile.TileType.Water)
        {
            return dir;
        }

        dir = nextTile.pos;
        return dir;
    }
    
    private void AIBuyTech(CivilisationController aiController, TechInfo.Technology tech)
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
        }));*/
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
            OccupyVillage(unit.occupiedTile.homeOnTile);
            unit.aiHomeExploring.exploringUnit = null;
            unit.aiHomeExploring = null;
            return unit.occupiedTile;
        }
        else
        {
            var tile = SearchPath.Instance.GetPath(unit.occupiedTile, unit.aiHomeExploring.homeTile)[0];
            return tile;
        }
    }

}