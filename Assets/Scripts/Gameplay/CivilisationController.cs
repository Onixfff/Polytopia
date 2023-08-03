using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
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
        var posLastTile = _gameBoardWindow.GetAllTile().Last().pos;
        var factor = posLastTile.y / 4;
        
        var civPoses = new Vector2Int(factor, factor);
        var civPoses1 = new Vector2Int(posLastTile.y - factor, posLastTile.y - factor);
        var civPoses2 = new Vector2Int(posLastTile.y - factor, factor);
        var civPoses3 = new Vector2Int(factor, posLastTile.y - factor);
        
        var listPos = new List<Vector2Int>() { civPoses, civPoses1, civPoses2, civPoses3};
        var randPos = listPos[Random.Range(0, listPos.Count)];
        var tile = _gameBoardWindow.GetTile(randPos);
        if (tile.homeOnTile != null)
        {
            CreateHome();
            return;
        }

        var homeO = Instantiate(homePrefab, tile.transform);
        homes.Add(homeO.GetComponent<Home>());
        homes[0].Init(this, tile.GetComponent<Tile>());
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
