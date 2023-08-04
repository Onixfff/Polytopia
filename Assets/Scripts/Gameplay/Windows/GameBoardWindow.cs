using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class GameBoardWindow : BaseWindow
{
    [SerializeField] private List<GameObject> boardPrefabs;

    [SerializeField] private List<Tile> generatedTiles;
    [SerializeField] private List<Home> generatedVillage;
    [SerializeField] private RectTransform tilePrefab;
    [SerializeField] private RectTransform tileParent;

    [SerializeField] private int mapSize = 16;
    [SerializeField] private CivilisationController civilisationPrefab;
    [SerializeField] private CivilisationController defaultCiv;
    [SerializeField] private Home village;
    [SerializeField] private GameInfo gameInfo;
    [SerializeField] private int waterFrequency = 10;

    private Sequence _generateSeq;

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
    }

    private void OnDestroy()
    {
        _generateSeq.Kill();
    }
    
    public void ShowAllOre()
    {
        foreach (var tile in generatedTiles)
        {
            if(tile.isHasMountain)
                tile.ShowOre();
        }
    }

    public Tile GetTile(Vector2Int pos)
    {
        return generatedTiles.Find(tile => tile.pos == pos);
    }
    
    public List<Tile> GetAllTile()
    {
        return generatedTiles;
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
                var closeTile = generatedTiles.Find(til => til.pos == new Vector2Int(x, y));
                if(closeTile == null || closeTile.pos == tile.pos)
                    continue;
                closeTiles.Add(closeTile);
            }
        }
        
        return closeTiles;
    }

    public bool IsThisTheNearestTile(Tile tile1, Tile tile2)
    {
        var closeTile = GetCloseTile(tile1, 1);
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

    public Home FindNearbyVillage(Tile tile)
    {
        var dist = 1000f;
        Home home = null;
        foreach (var village in generatedVillage)
        {
            var point = village.homeTile.pos;
            var point2 = tile.pos;
            var distance = Vector2.Distance(point, point2);
            if(distance < dist)
            {
                home = village;
                dist = distance;
            }
        }

        return home;
    }
    
    public Home FindRandomVillage(Tile tile)
    {
        var listVillage = generatedVillage.Where(village => village.homeType == Home.HomeType.Village).ToList();
        var rand = Random.Range(0, listVillage.Count);
        if (rand >= 0 && rand < listVillage.Count)
            return listVillage[rand];
        else
            return null;
    }

    [Button()]
    private void GenerateBoard()
    {
        if(generatedTiles != null) RemoveBoard();
        generatedTiles ??= new List<Tile>();
        var width = (int)(mapSize / Mathf.Sqrt(mapSize));
        for (var i = width; i > 0; i--)
        {
            for (var j = 0; j < width; j++)
            {
                var tileRectTransform = Instantiate(tilePrefab, tileParent);
                
                tileRectTransform.anchoredPosition = new Vector2(((i + j) * 50f) - Mathf.Sqrt(mapSize)*50, (i - j) * 31.5f);
                var ja = j + 1;
                tileRectTransform.name = i + ", " + ja;
                var tile = tileRectTransform.GetComponent<Tile>();
                tile.pos = new Vector2Int(i, ja);
                generatedTiles.Add(tile);
                
                SubscribeOnTile(tile);
            }
        }
    }

    [Button()]
    private void GenerateEnvironment()
    {
        foreach (var tile in generatedTiles)
        {
            if(tile.homeOnTile != null)
                continue;
            var a = Random.Range(0, 9);
            switch (a)
            {
                case 0: 
                    tile.SetPumpkinSprite(defaultCiv.civilisationInfo.fruitSprite, defaultCiv.civilisationInfo.fruitName);
                    break;
                case 1:
                    tile.SetTreeSprite(defaultCiv.civilisationInfo.treeSprite, defaultCiv.civilisationInfo.treeName);
                    break;
                case 3:
                    tile.SetTreeSprite(defaultCiv.civilisationInfo.treeSprite, defaultCiv.civilisationInfo.treeName);
                    tile.SetAnimalSprite(defaultCiv.civilisationInfo.animalSprite, defaultCiv.civilisationInfo.animalName);
                    break;
                case 4:
                    tile.SetAnimalSprite(defaultCiv.civilisationInfo.animalSprite, defaultCiv.civilisationInfo.animalName);
                    break;
                case 5:
                    tile.SetMountainSprite(defaultCiv.civilisationInfo.mountainSprite, defaultCiv.civilisationInfo.mountainName);
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
    private void GenerateWater()
    {
        foreach (var tile in generatedTiles)
        {
            if(!tile.IsTileFree() || tile.homeOnTile != null)
                continue;
            
            var a = Random.Range(0, waterFrequency);
            switch (a)
            {
                case 0:
                    tile.waterRad = 0;
                    tile.CreateWaterArea();
                    break;
                case 1:
                    tile.waterRad = 1;
                    tile.CreateWaterArea();
                    break;
                case 2:
                    tile.waterRad = 1;
                    tile.CreateWaterArea();
                    break;
            }
        }
    }

    [Button()]
    private void CreateCivilisations()
    {
        var randomCiv = Random.Range(0, gameInfo.playerCivilisationInfoLists.Count);
        var listCiv = new List<int> { randomCiv };
        var civilisation = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
        civilisation.GetComponent<CivilisationController>().Init(gameInfo.playerCivilisationInfoLists[randomCiv]);
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
            var civilisation1 = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
            civilisation1.GetComponent<CivilisationController>().AIInit(gameInfo.civilisationInfoLists[randC]);
            
            var ai = Instantiate(AIController.Instance.aiPrefab, civilisation1.transform);
            ai.aiNumber = i;
            AIController.Instance.AddAI(ai);
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
                
                randomTile = generatedTiles[Random.Range(0, generatedTiles.Count)];
                if(randomTile.tileType == Tile.TileType.Water)
                    continue;
                if(randomTile.unitOnTile != null)
                    continue;
                if(GetCloseTile(randomTile, 1).Find(tile => tile.homeOnTile))
                    continue;
                if(randomTile.homeOnTile != null)
                    continue;

                break;
            }

            if (randomTile != null)
            {
                var home = Instantiate(village, randomTile.transform);
                generatedVillage.Add(home);
                home.Init(null, randomTile);
                home.homeType = Home.HomeType.Village;
            }
        }
    }
    
    [Button()]
    private void RemoveBoard()
    {
        for (var i = 0; i < generatedTiles.Count; i++)
        {
            DestroyImmediate(generatedTiles[i].gameObject);
        }
        
        generatedTiles.Clear();
    }
    
    [Button()]
    private void UnlockAllTile()
    {
        foreach (var tile in generatedTiles.Select(tile => tile.GetComponent<Tile>()))
        {
            tile.UnlockTile();
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