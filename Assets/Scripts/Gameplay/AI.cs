using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public int aiNumber;
    
    private CivilisationController _controller;

    private void Start()
    {
        _controller = GetComponent<CivilisationController>();
    }

    public void StartTurn()
    {
        var homes = _controller.GetAllHome();

        var random = new Random();
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
        
        UnitsMove(allUnits, 0);

        foreach (var home in homes)
        {
            home.AIBuyUnit();
        }
    }

    private void UnitsMove(List<UnitController> units, int index)
    {
        if(units.Count < index)
            return;
        
        var homes = _controller.GetAllHome();
        units[index].MoveToTile(GetTileForMove(units[index])).OnComplete((() =>
        {
            UnitsMove(units, index+1);
        }));
    }

    private Tile GetTileForMove(UnitController unit)
    {
        var board = LevelManager.Instance.gameBoardWindow;

        if (unit.aiHomeExploring == null)
        {
            for (var i = 0; i <= 5; i++)
            {
                var village = board.FindRandomVillage(unit.occupiedTile);
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
            var tile = SearchPath.Instance.GetPath(unit.occupiedTile, unit.aiHomeExploring.homeTile);
            if (tile == null)
                return unit.occupiedTile;
            return tile[0];
        }
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
    
    private void AIBuyTech(CivilisationController aiController, TechInfo.Technology tech)
    {
        aiController.technologies.Add(tech);
    }
}

