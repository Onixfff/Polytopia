using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    private Home _capital;
    private List<Home> _connectedHomes;

    public void CheckConnectivityToTheCapital(Tile newTile)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        
        _connectedHomes ??= new List<Home>();
        var roadTiles = new List<Tile> { newTile };

        CheckCloseRoads(board.GetCloseTile(newTile, 1));
        var homes = new List<Home>();
        foreach (var roadTile in roadTiles)
        {
            homes.AddRange(board.GetCloseTile(roadTile, 1).FindAll(tile => tile.GetHomeOnTile() != null).Select(tile => tile.GetHomeOnTile()).ToList());
        }

        homes.RemoveAll(home => home.owner == null);
        
        var capital = homes.Find(home => home.owner.capitalHome == home);
        if(capital == null)
            return;

        if (_capital == null)
            _capital = capital;
        foreach (var home in homes)
        {
            if(_connectedHomes.Contains(home))
                continue;
            _connectedHomes.Add(home);
            home.AddFood(1);
            _capital.AddFood(1);
        }
        
        void CheckCloseRoads(List<Tile> tiles)
        {
            foreach (var tile in tiles.Where(tile => tile.roads.isRoad))
            {
                if(tile.GetOwner() != null && tile.GetOwner().owner != newTile.GetOwner().owner)
                    continue;
                if(roadTiles.Contains(tile))
                    continue;
                
                roadTiles.Add(tile);
                var closeTile = board.GetCloseTile(tile, 1);
                CheckCloseRoads(closeTile);
            }
        }
    }
}
