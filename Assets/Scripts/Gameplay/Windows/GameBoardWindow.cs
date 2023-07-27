using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBoardWindow : BaseWindow
{
    [SerializeField] private List<GameObject> boardPrefabs;

    [SerializeField] private List<Tile> generatedTiles;
    [SerializeField] private RectTransform tilePrefab;
    [SerializeField] private RectTransform tileParent;

    [SerializeField] private int mapSize = 16;
    [SerializeField] private CivilisationController civilisationPrefab;
    [SerializeField] private CivilisationController defaultCiv;
    [SerializeField] private Home village;
    [SerializeField] private GameInfo gameInfo;
    [SerializeField] private int waterFrequency = 10;

    private void Awake()
    {
        LevelManager.Instance.gameBoardWindow = this;

        GenerateBoard();
        CreateCivilisations();
        GenerateEnvironment();
        GenerateWater();
        GenerateVillage();
    }

    public List<Tile> GetAllTile()
    {
        return generatedTiles;
    }
    
    public List<Tile> GetCloseTile(Tile tile, int radius)
    {
        var closeTiles = new List<Tile>();
        var allTiles = generatedTiles.Select(til => til.GetComponent<Tile>()).ToList();

        var tilePos = tile.pos;

        var minX = tilePos.x - radius;
        var minY = tilePos.y - radius;
        var maxX = tilePos.x + radius;
        var maxY = tilePos.y + radius;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                var closeTile = allTiles.Find(til => til.pos == new Vector2Int(x, y));
                if(closeTile == null || closeTile.pos == tile.pos)
                    continue;
                closeTiles.Add(closeTile);
            }
        }
        
        return closeTiles;
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
        var tiles = generatedTiles.Select(tile => tile.GetComponent<Tile>()).ToList();
        
        foreach (var tile in tiles)
        {
            if(!tile.IsTileFree() || tile.homeOnTile != null)
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
        var a = Random.Range(0, gameInfo.playerCivilisationInfoLists.Count);
        var civilisation = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
        civilisation.GetComponent<CivilisationController>().Init(gameInfo.playerCivilisationInfoLists[a]);

        for (var i = 0; i < gameInfo.playersCount - 1; i++)
        {
            var civilisation1 = Instantiate(civilisationPrefab, DynamicManager.Instance.transform);
            civilisation1.GetComponent<CivilisationController>().AIInit(gameInfo.civilisationInfoLists[Random.Range(0, gameInfo.civilisationInfoLists.Count)]);
        }
    }
    
    [Button()]
    private void GenerateVillage()
    {
        for (var i = 0; i < 6; i++)
        {
            Tile randomTile;
            while (true)
            {
                randomTile = generatedTiles[Random.Range(0, generatedTiles.Count)];
                if(randomTile.tileType == Tile.TileType.Water)
                    continue;
                if(randomTile.unitOnTile != null)
                    continue;
                break;
            }

            var home = Instantiate(village, randomTile.transform);
            home.Init(null, randomTile);
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