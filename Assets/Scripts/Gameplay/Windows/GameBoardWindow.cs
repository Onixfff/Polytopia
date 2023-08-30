using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class GameBoardWindow : BaseWindow
{
    public CivilisationController playerCiv;
    [SerializeField] private RectTransform tileParent;
    [SerializeField] private RectTransform unitParent;
    [SerializeField] private Transform uiParent;

    [SerializeField] private List<Home> generatedVillage;
    [SerializeField] private RectTransform tilePrefab;

    [SerializeField] private int mapSize = 16;
    [SerializeField] private CivilisationController civilisationPrefab;
    [SerializeField] private CivilisationController defaultCiv;
    [SerializeField] private Home village;
    [SerializeField] private GameInfo gameInfo;
    [SerializeField] private int countWaterZone = 6;
    [SerializeField] private int countRuin = 5;

    private Dictionary<Vector2Int ,Tile> _generatedTiles;
    private Sequence _generateSeq;

    public void DeselectAllTile()
    {
        foreach (var tile in _generatedTiles.Values.ToList())
        {
            tile.DeselectedTile();
        }
    }
    
    public Transform GetUIParent()
    {
        return uiParent;
    }
    
    public void ShowAllOre()
    {
        foreach (var vector2Int in _generatedTiles.Keys.ToList())
        {
            var tile = _generatedTiles[vector2Int];
            if(tile.isHasOre)
                tile.ShowOre();
        }
    }
    
    public void ShowAllCrop()
    {
        foreach (var vector2Int in _generatedTiles.Keys.ToList())
        {
            var tile = _generatedTiles[vector2Int]; 
            if(tile.isHasCrop)
                tile.ShowCrop();
        }
    }

    public RectTransform GetUnitParent()
    {
        return unitParent;
    }

    public Tile GetTile(Vector2Int pos)
    {
        if (!_generatedTiles.ContainsKey(pos))
            return null;
        return _generatedTiles[pos];
    }
    
    public Dictionary<Vector2Int ,Tile> GetAllTile()
    {
        return _generatedTiles;
    }
    
    public List<Tile> GetCloseTile(Tile tile, int radius)
    {
        var closeTiles = new List<Tile>();

        var tilePos = tile.pos;

        var minX = tilePos.x - radius;
        var minY = tilePos.y - radius;
        var maxX = tilePos.x + radius;
        var maxY = tilePos.y + radius;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if(!_generatedTiles.ContainsKey(new Vector2Int(x, y)))
                    continue;
                var closeTile = _generatedTiles[new Vector2Int(x, y)];
                if(closeTile.pos == tile.pos)
                    continue;
                closeTiles.Add(closeTile);
            }
        }
        
        return closeTiles;
    }
    
    public List<Tile> GetSideTile(Tile tile, int radius)
    {
        var sideTiles = new List<Tile>();

        var tilePos = tile.pos;

        var minX = tilePos.x - radius;
        var minY = tilePos.y - radius;
        var maxX = tilePos.x + radius;
        var maxY = tilePos.y + radius;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if(x == minX && y == minY)
                    continue;
                if(x == minX && y == maxY)
                    continue;
                if(x == maxX && y == minY)
                    continue;
                if(x == maxX && y == maxY)
                    continue;
                if(!_generatedTiles.ContainsKey(new Vector2Int(x, y)))
                    continue;
                var sideTile = _generatedTiles[new Vector2Int(x, y)];
                if(sideTile.pos == tile.pos)
                    continue;
                sideTiles.Add(sideTile);
            }
        }
        return sideTiles;
    }

    public bool IsThisTheNearestTile(Tile tile1, Tile tile2, int rad)
    {
        var closeTile = GetCloseTile(tile1, rad);
        return closeTile.Contains(tile2);
    }
    
    public List<Home> GetVillage()
    {
        return generatedVillage;
    }
    
    public void RemoveVillage(Home home)
    {
        generatedVillage.Remove(home);
    }
    
    private void Awake()
    {
        LevelManager.Instance.gameBoardWindow = this;
        
        _generateSeq = DOTween.Sequence();
        GenerateBoard();
        var inVal = 0f;
        _generateSeq.Append(DOTween.To(() => inVal, x => x = inVal, 0f, 0.1f).OnComplete((GenerateEnvironment)));
        var inVal1 = 0f;
        _generateSeq.Append(DOTween.To(() => inVal1, x => x = inVal1, 0f, 0.1f).OnComplete((CreateCivilisations)));
        var inVal2 = 0f;
        _generateSeq.Append(DOTween.To(() => inVal2, x => x = inVal2, 0f, 0.1f).OnComplete((GenerateWater)));
        var inVal3 = 0f;
        _generateSeq.Append(DOTween.To(() => inVal3, x => x = inVal3, 0f, 0.1f).OnComplete((GenerateVillage)));
        var inVal4 = 0f;
        _generateSeq.Append(DOTween.To(() => inVal4, x => x = inVal4, 0f, 0.1f).OnComplete((GenerateRuins)));
    }

    private void OnDestroy()
    {
        _generateSeq.Kill();
    }
    
    [Button()]
    private void GenerateBoard()
    {
        if(_generatedTiles != null) RemoveBoard();
        _generatedTiles ??= new Dictionary<Vector2Int, Tile>();
        var width = (int)(mapSize / Mathf.Sqrt(mapSize));
        for (var i = width; i > 0; i--)
        {
            for (var j = 0; j < width; j++)
            {
                var tileRectTransform = Instantiate(tilePrefab, tileParent);
                
                tileRectTransform.anchoredPosition = new Vector2(((i + j) * 50f) - Mathf.Sqrt(mapSize)*50, (i - j) * 31.5f);
                var ja = j + 1;
                tileRectTransform.name = width - i + 1 + ", " + ja;
                var tile = tileRectTransform.GetComponent<Tile>();
                tile.pos = new Vector2Int(width - i + 1, ja);
                _generatedTiles.Add(tile.pos, tile);
                
                SubscribeOnTile(tile);
            }
        }
    }

    [Button()]
    private void GenerateEnvironment()
    {
        foreach (var vector2Int in _generatedTiles.Keys.ToList())
        {
            var tile = _generatedTiles[vector2Int];
            if(tile.GetHomeOnTile() != null)
                continue;
            var a = Random.Range(0, 10);
            switch (a)
            {
                case 0: 
                    tile.SetPumpkinSprite(defaultCiv.civilisationInfo.FruitSprite, defaultCiv.civilisationInfo.fruitName);
                    break;
                case 1:
                    tile.SetTreeSprite(defaultCiv.civilisationInfo.TreeSprite, defaultCiv.civilisationInfo.treeName);
                    break;
                case 3:
                    tile.SetTreeSprite(defaultCiv.civilisationInfo.TreeSprite, defaultCiv.civilisationInfo.treeName);
                    tile.SetAnimalSprite(defaultCiv.civilisationInfo.AnimalSprite, defaultCiv.civilisationInfo.animalName);
                    break;
                case 4:
                    tile.SetAnimalSprite(defaultCiv.civilisationInfo.AnimalSprite, defaultCiv.civilisationInfo.animalName);
                    break;
                case 5:
                    tile.SetMountainSprite(defaultCiv.civilisationInfo.MountainSprite, defaultCiv.civilisationInfo.mountainName, true);
                    break;
                case 6:
                    tile.SetMountainSprite(defaultCiv.civilisationInfo.MountainSprite, defaultCiv.civilisationInfo.mountainName, false);
                    break;
                case 7:
                    tile.SetCropSprite();
                    break;
                default:
                    tile.SetTreeSprite(null, defaultCiv.civilisationInfo.groundName);
                    tile.SetPumpkinSprite(null, defaultCiv.civilisationInfo.groundName);
                    tile.SetAnimalSprite(null, defaultCiv.civilisationInfo.groundName);
                    break;
            }
        }
    }
    
    [Button()]
    private void GenerateRuins()
    {
        for (var i = 0; i < countRuin; i++)
        {
            Tile randomTile = null;
            for(var j = 100; j >= 0; j--)
            {
                if (j == 0)
                    return;
                
                randomTile = _generatedTiles[_generatedTiles.Keys.ToList()[Random.Range(0, _generatedTiles.Keys.ToList().Count)]];
                if(randomTile.tileType == Tile.TileType.Water)
                    continue;
                if(randomTile.unitOnTile != null)
                    continue;
                if(GetCloseTile(randomTile, 1).Find(tile => tile.GetOwner()))
                    continue;
                if(GetCloseTile(randomTile, 1).Find(tile => tile.isHasRuins))
                    continue;
                if(randomTile.GetOwner() != null)
                    continue;
                break;
            }

            if (randomTile != null)
            {
                randomTile.SetRuinsSprite(null, "Ruins");
            }
        }
    }
    
    [Button()]
    private void GenerateWater()
    {
        for (var i = 0; i < countWaterZone; i++)
        {
            var randomTile = _generatedTiles[_generatedTiles.Keys.ToList()[Random.Range(0, _generatedTiles.Keys.ToList().Count)]];
            randomTile.CreateWaterArea(Random.Range(0, 2));
        }
        
        UpdateWaterVisual();
    }
    
    [Button()]
    private void UpdateWaterVisual()
    {
        foreach (var tile in _generatedTiles.Values.ToList().Where(tile => tile.tileType == Tile.TileType.Water || tile.tileType == Tile.TileType.DeepWater))
        {
            tile.SetWaterTile();
        }
    }

    [Button()]
    private void CreateCivilisations()
    {
        var randomCiv = Random.Range(0, gameInfo.playerCivilisationInfoLists.Count);
        var listCiv = new List<int> { randomCiv };
        var civilisationController = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
        playerCiv = civilisationController;
        LevelManager.Instance.gameplayWindow.SetPlayerCiv(playerCiv);
        LevelManager.Instance.AddCiv(playerCiv);
        playerCiv.Init(gameInfo.playerCivilisationInfoLists[randomCiv]);
        AIController.Instance.countAi = gameInfo.playersCount - 1;
        
        for (var i = 0; i < gameInfo.playersCount - 1; i++)
        {
            var randC = Random.Range(0, gameInfo.civilisationInfoLists.Count);
            while (true)
            {
                if(listCiv.Contains(randC))
                    randC = Random.Range(0, gameInfo.civilisationInfoLists.Count);
                else
                    break;
            }
            listCiv.Add(randC);
            
            var civilisation1 = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
            LevelManager.Instance.AddCiv(civilisation1);
            civilisation1.AIInit(gameInfo.civilisationInfoLists[randC]);
            
            var ai = Instantiate(AIController.Instance.aiPrefab, civilisation1.transform);
            ai.aiNumber = i;
            AIController.Instance.AddAI(ai);
        }
    }
    
    [Button()]
    private void UpdateRuinsVisual()
    {
        foreach (var tile in _generatedTiles.Values.ToList().Where(tile => tile.tileType == Tile.TileType.Water || tile.tileType == Tile.TileType.DeepWater))
        {
            tile.SetWaterTile();
        }
    }
    
    [Button()]
    private void GenerateVillage()
    {
        for (var i = 0; i < 6; i++)
        {
            Tile randomTile = null;
            for(var j = 100; j >= 0; j--)
            {
                if (j == 0)
                    return;
                
                randomTile = _generatedTiles[_generatedTiles.Keys.ToList()[Random.Range(0, _generatedTiles.Keys.ToList().Count)]];
                if(randomTile.tileType == Tile.TileType.Water || randomTile.tileType == Tile.TileType.DeepWater)
                    continue;
                if(randomTile.unitOnTile != null)
                    continue;
                if(GetCloseTile(randomTile, 1).Find(tile => tile.GetOwner()))
                    continue;
                if(randomTile.GetOwner() != null)
                    continue;

                break;
            }

            if (randomTile != null)
            {
                var home = Instantiate(village, randomTile.transform);
                home.gameObject.SetActive(false);
                generatedVillage.Add(home);
                randomTile.isHasMountain = false;
                home.Init(null, randomTile);
                home.homeType = Home.HomeType.Village;
            }
        }
    }
    
    [Button()]
    private void RemoveBoard()
    {
        for (var i = 0; i < _generatedTiles.Count; i++)
        {
            var a = _generatedTiles.Keys.ToList()[i];
            DestroyImmediate(_generatedTiles[a].gameObject);
        }
        
        _generatedTiles.Clear();
    }
    
    [Button()]
    private void UnlockAllTile()
    {
        foreach (var vector2Int in _generatedTiles.Keys.ToList())
        {
            var tile = _generatedTiles[vector2Int];
            tile.UnlockTile(playerCiv);
        }
    }
    
    private void SubscribeOnTile(Tile tile)
    {
        tile.OnClickOnTile += ShowTileInfo;
    }
 
    private void ShowTileInfo(Tile tile)
    {
        //LevelManager.Instance.gameplayWindow.SetTileName(tile.GetTileName());
    }
}