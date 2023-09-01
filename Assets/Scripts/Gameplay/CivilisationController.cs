using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class CivilisationController : MonoBehaviour
{
    public event Action OnMoneyChanged;

    public CivilisationInfo civilisationInfo;
    public List<Home> homes;
    public Home independentHome;
    public Home capitalHome;
    public Color civColor;
    public List<TechInfo.Technology> technologies;
    public Dictionary<CivilisationController, int> TurnWhenIWasAttack;

    private int _turnWhenIAttack;
    public int turnWhenIAttack
    {
        get => _turnWhenIAttack;
        
        set
        {
            _turnWhenIAttack = value;
            civilisationStats.SetTurnWithoutAttack(0);
        }
    }

    public RelationOfCivilisation relationOfCivilisation;
    public string civilName = "";
    
    public int Money
    {
        get => _money;
        private set
        {
            _money = value;
            OnMoneyChanged?.Invoke();
        }
    }
    
    public int Point
    {
        get => _point;
        private set
        {
            _point = value;
            OnMoneyChanged?.Invoke();
        }
    }

    [SerializeField] private GameObject homePrefab;
    [SerializeField] private MonumentBuilder monumentBuilder;
    [SerializeField] private CivilisationStats civilisationStats;
    private GameBoardWindow _gameBoardWindow;
    private List<Tile> _exploredTiles;
    private int _money;
    private int _point;

    public void Init(CivilisationInfo info)
    {
        TurnWhenIWasAttack = new Dictionary<CivilisationController, int>();
        
        civilisationInfo = info;
        civColor = info.CivilisationColor;
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        AddNewTechnology(info.technology.startTechnologies);

        AddPoint(565);
        CreateHome();
        SetupCivilisation();
        InitIndependentHome();

        if (info.controlType == CivilisationInfo.ControlType.Player)
        {
            civilName = "Player";
            AddMoney(5);
            AddMoney(20);
            
            LevelManager.Instance.OnTurnBegin += () =>
            {
                AddMoney(GetCurrentIncome());
                var incomePoint = 0;
                foreach (var home in homes)
                {
                    if(home.homeTile.unitOnTile != null && home.homeTile.unitOnTile.GetOwner().owner != this)
                        continue;
                    incomePoint += home.GetIncomePoint();
                }
                AddPoint(incomePoint);
            };
            LevelManager.Instance.OnUnlockTechnology += AddNewTechnology;
        }
        else
        {
            civilName = info.civilisationName + "" + Random.Range(10000, 99999).ToString();
            switch (GameManager.Instance.difficult)
            {
                case GameManager.Difficult.Easy:
                    AddMoney(5);
                    break;
                case GameManager.Difficult.Normal:
                    AddMoney(10);
                    break;
                case GameManager.Difficult.Hard:
                    AddMoney(20);
                    break;
            }

            LevelManager.Instance.OnTurnBegin += () =>
            {
                AddMoney(GetCurrentIncome());
                var incomePoint = 0;
                foreach (var home in homes)
                {
                    if(home.homeTile.unitOnTile != null && home.homeTile.unitOnTile.GetOwner().owner != this)
                        continue;
                    incomePoint += home.GetIncomePoint();
                }
                switch (GameManager.Instance.difficult)
                {
                    case GameManager.Difficult.Easy:
                        incomePoint -= 2;
                        break;
                    case GameManager.Difficult.Normal:
                        incomePoint += 1;
                        break;
                    case GameManager.Difficult.Hard:
                        incomePoint += 3;
                        break;
                }
                AddPoint(incomePoint);
            };
        }
        

        void InitIndependentHome()
        {
            independentHome.Init(this, null);
        }
    }

    public int GetCurrentIncome()
    {
        var income = 0;
        foreach (var home in homes)
        {
            if(home.homeTile.unitOnTile != null && home.homeTile.unitOnTile.GetOwner().owner != this)
                continue;
            income += home.GetIncome();
        }

        return income;
    }

    public MonumentBuilder GetMonumentBuilder()
    {
        return monumentBuilder;
    }
    
    public CivilisationStats GetCivilisationStats()
    {
        return civilisationStats;
    }
    
    public void BuySomething(int cost)
    {
        Money -= cost;
    }

    public bool IsCanBuy(int cost)
    {
        return Money >= cost;
    }

    public void AddMoney(int count)
    {
        Money += count;
        civilisationStats.AddStars(count);
    }
    
    public void AddPoint(int count)
    {
        Point += count;
    }

    public bool CheckAlly(CivilisationController controller)
    {
        return relationOfCivilisation.GetRelation(controller) == DiplomacyManager.RelationType.Peace;
    }
    
    public DiplomacyManager.RelationType GetRelation(CivilisationController controller)
    {
        return relationOfCivilisation.GetRelation(controller);
    }
    
    public void ChangeAnotherCivRelationAfterAttack(CivilisationController controller, bool isAttack)
    {
        if (isAttack)
        {
            Relation(0);
            if (TurnWhenIWasAttack.ContainsKey(controller))
                TurnWhenIWasAttack[controller] = LevelManager.Instance.currentTurn;
            else
                TurnWhenIWasAttack.Add(controller, LevelManager.Instance.currentTurn);
        }

        void Relation(int index)
        {
            Req();
            LevelManager.Instance.OnTurnBegin -= Req;
            LevelManager.Instance.OnTurnBegin += Req;

            void Req()
            {
                
                DiplomacyManager.RelationType relationType;
                if (index <= 2)
                {
                    relationType = DiplomacyManager.RelationType.War;
                    
                }
                else
                {
                    relationType = DiplomacyManager.RelationType.Neutral;
                    LevelManager.Instance.OnTurnBegin -= Req;
                }
                index += 1;
                
                //Debug.Log(civilName + " relation to " + controller.civilName + " how - " + relationType);
                ChangeRelation(controller, relationType);
            }
        }
    }

    public void ProposeAnAlliance(CivilisationController controller)
    {
        ChangeRelation(controller, DiplomacyManager.RelationType.Peace);
    }

    public void ChangeRelation(CivilisationController controller, DiplomacyManager.RelationType relationType)
    {
        relationOfCivilisation.SetRelation(controller, relationType);
        controller.relationOfCivilisation.SetRelation(this, relationType);
    }
    
    public List<Home> GetAllHome()
    {
        return homes;
    }

    public List<UnitController> GetAllUnit()
    {
        var a = new List<UnitController>();
        foreach (var home in homes)
        {
            a.AddRange(home.GetUnitList());
        }

        return a;
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
    
    public void AddTileInExploreList(Tile tile)
    {
        _exploredTiles ??= new List<Tile>();
        _exploredTiles.Add(tile);
    }
    
    public List<Tile> GetTileInExploreList()
    {
        _exploredTiles ??= new List<Tile>();
        return _exploredTiles;
    }
    
    private void AddNewTechnology(TechInfo.Technology technology)
    {
        switch (technology)
        {
            case TechInfo.Technology.Mountain:
                break;
            case TechInfo.Technology.Hunt:
                break;
            case TechInfo.Technology.Rider:
                break;
            case TechInfo.Technology.Fish:
                break;
            case TechInfo.Technology.Gather:
                break;
            case TechInfo.Technology.FreeSpirit:
                break;
            case TechInfo.Technology.Farming:
                break;
            case TechInfo.Technology.Strategy:
                break;
            case TechInfo.Technology.Mining:
                break;
            case TechInfo.Technology.Archery:
                break;
            case TechInfo.Technology.Sailing:
                break;
            case TechInfo.Technology.Chivalry:
                break;
            case TechInfo.Technology.Diplomacy:
                break;
            case TechInfo.Technology.Forge:
                break;
            case TechInfo.Technology.Philosophy:
                break;
            case TechInfo.Technology.Mathematics:
                break;
            case TechInfo.Technology.Meditation:
                
                break;
            case TechInfo.Technology.Roads:
                break;
            case TechInfo.Technology.Trade:
                break;
            case TechInfo.Technology.Construction:
                break;
            case TechInfo.Technology.Navigation:
                break;
            case TechInfo.Technology.Whaling:
                break;
            case TechInfo.Technology.Aqua:
                break;
            case TechInfo.Technology.Spiritualism:
                break;
            case TechInfo.Technology.Forestry:
                break;
        }
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
        var tiles = _gameBoardWindow.GetCloseTile(homes[0].homeTile, 2);
        homes[0].homeTile.UnlockTile(this);
        foreach (var tile in tiles)
        {
            tile.UnlockTile(this);
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
            allUnits[i].TakeDamage(null, 10000);
        }
    }
}