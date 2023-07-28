using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CivilisationController : MonoBehaviour
{
    [SerializeField] private GameObject homePrefab;
    public CivilisationInfo civilisationInfo;
    public List<Home> homes;
    public Color civColor;
    public List<TechInfo.Technology> technologies;

    private GameBoardWindow _gameBoardWindow;

    public void Init(CivilisationInfo info)
    {
        civilisationInfo = info;
        technologies.AddRange(info.technology.startTechnologies);
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        civColor = info.civilisationColor;
        CreateHome();
        SetupCivilisation();

        LevelManager.Instance.OnUnlockTechnology += AddNewTechnology;
    }
    
    public void AIInit(CivilisationInfo info)
    {
        civilisationInfo = info;
        technologies.AddRange(info.technology.startTechnologies);
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        civColor = info.civilisationColor;
        CreateHome();
    }
    
    public void AddNewTechnology(TechInfo.Technology technology)
    {
        technologies.Add(technology);
    }
    
    private void CreateHome()
    {
        var allTile = _gameBoardWindow.GetAllTile();
        var randomTile = allTile[Random.Range(0, allTile.Count)];
        if (randomTile.GetComponent<Tile>().tileType == Tile.TileType.Water || LevelManager.Instance.gameBoardWindow.GetCloseTile(randomTile, 2).Find(tile => tile.homeOnTile))
        {
            CreateHome();
            return;
        }
        var homeO = Instantiate(homePrefab, randomTile.transform);
        homes.Add(homeO.GetComponent<Home>());
        homes[0].Init(this, randomTile.GetComponent<Tile>());
    }
    
    private void SetupCivilisation()
    {
        var tiles = _gameBoardWindow.GetCloseTile(homes[0].homeTile, 2).Select(tile => tile.GetComponent<Tile>()).ToList();
        homes[0].homeTile.UnlockTile();
        foreach (var tile in tiles)
        {
            tile.UnlockTile();
        }
    }
    
    public List<Home> GetAllHome()
    {
        return homes;
    }
}
