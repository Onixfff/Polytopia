using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchPath : Singleton<SearchPath>
{
    private Tile _startingPoint;
    private Tile _endingPoint;

    private Dictionary<Vector2Int, Tile> _tileDictionary = new Dictionary<Vector2Int, Tile>();

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
    private Queue<Tile> _queue = new Queue<Tile>();
    private Tile _searchingPoint;
    private bool _isExploring = true;

    private List<Tile> _path = new List<Tile>();
    
    public List<Tile> Path
    {
        get
        {
            LoadAllBlocks();
            BFS();
            CreatePath();
            
            return _path;
        }
    }

    public List<Tile> GetPath(Tile startTile, Tile endTile)
    {
        _startingPoint = startTile;
        _endingPoint = endTile;
        return Path;
    }

    private void CreatePath()
    {
        SetPath(_endingPoint);
        var previousNode = _endingPoint.isExploredFrom;

        while (previousNode != _startingPoint) 
        {
            SetPath(previousNode);
            previousNode = previousNode.isExploredFrom;
        }

        //SetPath(_startingPoint);
        _path.Reverse();
    }

    private void LoadAllBlocks()
    {
        foreach (var tile in LevelManager.Instance.gameBoardWindow.GetAllTile()/*.Where(tile => tile.tileType == Tile.TileType.Ground).ToList()*/)
        {
            /*if(!tile.IsTileFree())
                continue;*/
            if (_tileDictionary.ContainsKey(tile.pos))
            {
                Debug.LogWarning("2 Nodes present in same position. i.e nodes overlapped.");
            }
            else
            {
                _tileDictionary.Add(tile.pos, tile);
            }
        }
    }
    
    private bool BFS()
    {
        _queue.Enqueue(_startingPoint);
        while (_queue.Count > 0 && _isExploring) 
        {
            _searchingPoint = _queue.Dequeue();
            if (_searchingPoint == _endingPoint) 
            {
                _isExploring = false;
                return true;
            } 
            
            _isExploring = true;
            ExploreNeighbourNodes();
        }

        return false;
    }

    private void ExploreNeighbourNodes()
    {
        if (_isExploring == false)
            return;

        foreach (var tilee in _directions) 
        {
            var neighbourPos = _searchingPoint.pos + tilee;

            if (_tileDictionary.ContainsKey(neighbourPos))
            {
                var tile = _tileDictionary[neighbourPos];

                if (tile.isExplored) { }
                else
                {
                    _queue.Enqueue(tile);
                    tile.isExplored = true;
                    tile.isExploredFrom = _searchingPoint;
                }
            }
        }
    }
    
    private void SetPath(Tile tile) 
    {
        _path.Add(tile);
    }
}