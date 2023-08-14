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
        EconomicManager.Instance.AddMoney(5);

        civilisationInfo = info;
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        if(civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
            technologies.AddRange(info.technology.startTechnologies);
        else
        {
            EconomicManager.Instance.AddMoney(100);
            technologies.AddRange(info.technology.startTechnologies);
        }
        civColor = info.CivilisationColor;
        CreateHome();
        SetupCivilisation();
        LevelManager.Instance.OnTurnBegin += () =>
        {
            var income = 0;
            foreach (var home in homes)
            {
                if(home.homeTile.unitOnTile != null && home.homeTile.unitOnTile.GetOwner().owner != this)
                    continue;
                income += home.GetIncome();
            }
            EconomicManager.Instance.AddMoney(income);
        };
        LevelManager.Instance.OnUnlockTechnology += AddNewTechnology;
    }
    
    public void AIInit(CivilisationInfo info)
    {
        civilisationInfo = info;
        technologies.AddRange(info.technology.startTechnologies);
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        civColor = info.CivilisationColor;
        CreateHome();
        LevelManager.Instance.OnTurnBegin += () =>
        {
            var income = 0;
            foreach (var home in homes)
            {
                income += home.GetIncome();
            }
            EconomicManager.Instance.AddAIMoney(income);
        };
    }
    
    public void AddNewTechnology(TechInfo.Technology technology)
    {
        technologies.Add(technology);
    }

    private void CreateHome()
    {
        var allTile = _gameBoardWindow.GetAllTile();
        var pos = _gameBoardWindow.GetAllTile().Keys.ToList().Last();
        var posLastTile = allTile[pos].pos;

        var factor = posLastTile.y / 4;
        
        var civPoses = new Vector2Int(factor, factor);
        var civPoses1 = new Vector2Int(posLastTile.y - factor, posLastTile.y - factor);
        var civPoses2 = new Vector2Int(posLastTile.y - factor, factor);
        var civPoses3 = new Vector2Int(factor, posLastTile.y - factor);
        
        var listPos = new List<Vector2Int>() { civPoses, civPoses1, civPoses2, civPoses3};
        var randPos = listPos[Random.Range(0, listPos.Count)];
        var tile = _gameBoardWindow.GetTile(randPos);
        if (tile.GetOwner() != null)
        {
            CreateHome();
            return;
        }

        var homeO = Instantiate(homePrefab, tile.transform);
        if(civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
            homeO.gameObject.SetActive(false);
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

    public void RemoveHome(Home home, List<UnitController> unitList = null)
    {
        homes.Remove(home);
        if (homes.Count == 0)
        {
            DestroyCivilisation();
            return;
        }

        if (unitList != null && unitList.Count != 0)
        {
            foreach (var unit in unitList)
            {
                homes[0].AddUnit(unit);
            }
        }
    }

    private void DestroyCivilisation()
    {
        LevelManager.Instance.RemoveCiv(this);
        LevelManager.Instance.CheckWin();
        DestroyAllUnits();
        Destroy(gameObject);
    }

    public void DestroyAllUnits()
    {
        var allUnits = new List<UnitController>();
        foreach (var home in homes)
        {
            allUnits.AddRange(home.GetUnitList());
        }
        if(allUnits.Count == 0)
            return;
        for (var i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].TakeDamage(10000);
        }
    }
}