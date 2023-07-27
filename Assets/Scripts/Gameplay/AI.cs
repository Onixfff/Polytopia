using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public int aiNumber;
    
    private CivilisationController _controller;
    private Dictionary<UnitController, List<Tile>> _unitFinder;

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

        foreach (var home in homes)
        {
            foreach (var unit in home.GetUnitList())
            {
                unit.MoveToTile(GetTileForMove(unit));
            }
        }
    }

    private Tile GetTileForMove(UnitController unit)
    {
        _unitFinder ??= new Dictionary<UnitController, List<Tile>>();
        var board = LevelManager.Instance.gameBoardWindow;
        var village = board.FindNearbyVillage(unit.occupiedTile);
        var tiles = new List<Tile>();
        if (!_unitFinder.ContainsKey(unit))
        {
            tiles = SearchPath.Instance.GetPath(unit.occupiedTile, village.homeTile);
            tiles.Remove(tiles.First());
            _unitFinder.Add(unit, tiles);
        }
        else
        {
            _unitFinder.TryGetValue(unit, out tiles);
            tiles.Remove(tiles.First());
            _unitFinder[unit] = tiles;
        }
        
        Debug.Log(unit.occupiedTile.pos + " to " + village.homeTile.pos);
        Debug.Log(tiles[0].pos);
        
        return tiles[0];
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

