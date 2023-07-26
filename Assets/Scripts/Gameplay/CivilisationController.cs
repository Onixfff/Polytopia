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
    public Home mainHome;
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
        if (randomTile.GetComponent<Tile>().tileType == Tile.TileType.Water)
        {
            CreateHome();
            return;
        }
        var homeO = Instantiate(homePrefab, randomTile.transform);
        mainHome = homeO.GetComponent<Home>();
        mainHome.Init(this, randomTile.GetComponent<Tile>());
    }
    
    private void SetupCivilisation()
    {
        var tiles = _gameBoardWindow.GetCloseTile(mainHome.homeTile, 2).Select(tile => tile.GetComponent<Tile>()).ToList();
        mainHome.homeTile.UnlockTile();
        foreach (var tile in tiles)
        {
            tile.UnlockTile();
        }
    }
}
