using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public enum HomeType
    {
        Village,
        City
    }
    
    public Tile homeTile;
    public CivilisationController owner;
    public HomeType homeType = HomeType.City;
    [SerializeField] private Button occupyButton;
    [SerializeField] private List<UnitController> unitPrefabs;
    [SerializeField] private List<GameObject> homeFoodBlocks;
    [SerializeField] private List<GameObject> buildingPrefab;
    [SerializeField] private GameObject centerBlockPrefab;
    [SerializeField] private GameObject blockScrollView;
    [SerializeField] private Transform blockParent;
    private List<UnitController> _unitList;
    private HomeInfo _homeInfo;
    private HomeCreator _homeCreator;
    //private int _boardRad;
    
    private int _homeLevel = 0;
    private int _foodFromNextLvl = 2;
    private int _foodCount;
    private int _unitCapacity = 2;
    private bool _isCapital = true;
    private bool _isWorkshop = false;
    private bool _isPark = false;
    
    public void Init(CivilisationController controller, Tile tile)
    {
        homeTile = tile;
        SetHomePos();
        
        _unitList ??= new List<UnitController>();
        if (controller != null)
        {
            _homeInfo = controller.civilisationInfo.Home;
            InitHomeCreator();
            
            SetOwner(controller);
            
            InitBlockScrollView();
            
            if(!controller.homes.Contains(this))
                controller.homes.Add(this);
            
            UpdateVisual();
            
            InitHome();
            
            LevelManager.Instance.gameplayWindow.OnUnitSpawn += BuyUnit;
            BuyStartUnit();
        }
        else
        {
            InitVillage();
        }

        occupyButton.onClick.RemoveAllListeners();
        occupyButton.onClick.AddListener(OccupyHome);

        #region InitFunc
        
        void SetHomePos()
        {
            transform.position = homeTile.GetRectTransform().position;
            transform.SetSiblingIndex(transform.GetSiblingIndex()-2);
        }

        void InitHomeCreator()
        {
            _homeCreator = GetComponent<HomeCreator>();
            GetComponent<Image>().enabled = false;
            if (_isCapital)
            {
                _homeCreator.CreateCapital();
                _homeCreator.CreateHome();
                UpdateVisual();
            }
            else
            {
                _homeCreator.CreateHome();
                UpdateVisual();
            }
        }

        void InitBlockScrollView()
        {
            blockScrollView.transform.SetParent(LevelManager.Instance.gameBoardWindow.GetUIParent());
            blockScrollView.SetActive(true);
            if(controller.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
                blockScrollView.SetActive(false);
        }

        void InitHome()
        {
            var _boardRad = 1;
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
            homeTile.BuildHome(this);
            foreach (var ti in tiles)
            {
                if(ti.GetOwner() != null && ti.GetOwner() != this)
                    continue;
                ti.SetOwner(this);
                ti.ChangeTileVisual(this);
            }
            _boardRad = 2;
            var tiless = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
            homeTile.ChangeHomeBoards();
            foreach (var til1e in tiless)
            {
                til1e.ChangeHomeBoards();
            }
        }

        void InitVillage()
        {
            GetComponent<Image>().enabled = true;
            _isCapital = false;
            homeTile.BuildHome(this);
            blockScrollView.SetActive(false);
        }
        
        #endregion
    }
    
    public void SelectHome()
    {
        if (owner == null) return;
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            LevelManager.Instance.gameplayWindow.ShowHomeButton();
        }
    }

    public void AddFood(int count)
    {
        for (var i = 0; i < count; i++)
        {
            _foodCount++;
            ChangeFoodBlock(true);
            if (_foodCount >= _foodFromNextLvl)
            {
                LeveUp();
            }
        }
    }
    
    public void RemoveFood(int count)
    {
        for (var i = 0; i < count; i++)
        {
            _foodCount--;
            ChangeFoodBlock(false);
            if (_foodCount >= _foodFromNextLvl)
            {
                LeveUp();
            }
        }
    }
    
    public int GetIncome()
    {
        var homeIncomeStars = 0;
        var i = 0;
        if (_isCapital)
        {
            i++;
        }

        if (_isWorkshop)
        {
            i++;
        }
        
        homeIncomeStars = 1 + _homeLevel + i;
        
        return homeIncomeStars;
    }
    public int GetIncomePoint()
    {
        var homeIncomePoint = 0;
        
        homeIncomePoint = 100 + (50 * _homeLevel);
        
        return homeIncomePoint;
    }

    #region LevelUp
    
    public void UpdateVisual()
    {
        _homeCreator.UpdateVisual(_homeInfo);
    }
    
    public void BuildForge()
    {
        _homeCreator.CreateForge();
        _isWorkshop = true;
    }

    public void CreateExplorer()
    {
        
    }
    
    public void CreateHomeWall()
    {
        
    }

    public void IncreaseBoarder()
    {
        var _boardRad = 2;
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
        foreach (var ti in tiles)
        {
            if(ti.GetOwner() != null && ti.GetOwner() != this)
                continue;
            ti.SetOwner(this);
            ti.ChangeTileVisual(this);
        }
        _boardRad = 3;
        var tiless = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
        homeTile.ChangeHomeBoards();
        foreach (var til1e in tiless)
        {
            til1e.ChangeHomeBoards();
        }
    }
    
    public void BuildPark()
    {
        _homeCreator.CreatePark();
        owner.AddPoint(250);
        _isPark = true;
    }
    
    public void CreateSuperUnit()
    {
        if (!homeTile.IsTileFree())
        {
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, 1);
            var freeTile = closeTile.Find(tile => tile.IsTileFree());
            homeTile.unitOnTile.MoveToTile(freeTile);
        }
        var unitObject = Instantiate(unitPrefabs[9], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(this, homeTile, 9);
        AddUnit(unit);
    }
    
    [Button()]
    private void LeveUp()
    {
        var block = Instantiate(centerBlockPrefab, blockParent);
        block.transform.SetSiblingIndex(blockParent.childCount-2);
        homeFoodBlocks.Insert(homeFoodBlocks.Count - 1, block);
        if (homeFoodBlocks.Count > 3)
        {
            var scale = 2;
            if (homeFoodBlocks.Count < 7)
                scale = 1;
            if (homeFoodBlocks.Count > 10)
                scale = 0;
            var sizeDelta = homeFoodBlocks.First().GetComponent<RectTransform>().sizeDelta;
            sizeDelta -= new Vector2(scale, 0);
            foreach (var foodBlock in homeFoodBlocks)
            {
                foodBlock.GetComponent<RectTransform>().sizeDelta = sizeDelta;
            }
        }
        RemoveAllFood();
        _foodCount = 0;
        _foodFromNextLvl++;
        
        _homeLevel++;
        
        _unitCapacity++;
        
        _homeCreator.LevelUpHome();
        UpdateVisual();
        
        owner.GetCivilisationStats().CheckMaxLevelCity(_homeLevel);
        
        void RemoveAllFood()
        {
            foreach (var foodBlock in homeFoodBlocks)
            {
                foodBlock.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            var alert = WindowsManager.Instance.CreateWindow<AlertWindow>("AlertWindow");
            alert.ShowWindow();
            alert.OnTop();
            alert.HomeLevelUp(this, _homeLevel);
        }
    }

    private void CheckPriceForLevel()
    {
        if (_homeLevel == 1)
        {
            
        }
        if (_homeLevel == 2)
        {
            
        }
        if (_homeLevel == 3)
        {
            
        }
        if (_homeLevel >= 4)
        {
            
        }
    }
    
    #endregion

    #region Occupy
    
    public void OccupyHome()
    {
        for (var i = _unitList.Count - 1; i >= 0; i--)
        {
            var unit = _unitList[i];
            unit.SetOwner(null);
            RemoveUnit(unit);
        }

        if(owner != null)
            owner.RemoveHome(this, _unitList);
        homeType = HomeType.City;
        homeTile.unitOnTile.GetOwner().RemoveUnit(homeTile.unitOnTile);
        HideOccupyButton();
        Init(homeTile.unitOnTile.GetOwner().owner, homeTile);
        homeTile.unitOnTile.SetOwner(this);
        AddUnit(homeTile.unitOnTile);
    }

    public void SetOwner(CivilisationController controller)
    {
        owner = controller;
    }

    public void ShowOccupyButton()
    {
        LevelManager.Instance.OnTurnBegin += Show;

        void Show()
        {
            occupyButton.gameObject.SetActive(true);
            LevelManager.Instance.OnTurnBegin -= Show;
        }
    }
    
    public void HideOccupyButton()
    {
        occupyButton.gameObject.SetActive(false);
    }
    
    #endregion
    
    private void OnDestroy()
    {
        LevelManager.Instance.gameplayWindow.OnUnitSpawn -= BuyUnit;
    }
    
    private void ChangeUnitBlock(bool isAdd)
    {
        if (isAdd)
        {
            var a = homeFoodBlocks[_unitList.Count - 1];
            a.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            var a = homeFoodBlocks[_unitList.Count];
            a.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    
    private void ChangeFoodBlock(bool isAdd)
    {
        switch (isAdd)
        {
            case true:
            {
                if (_foodCount - 1 < 0)
                {
                    var a = homeFoodBlocks[Mathf.Abs(_foodCount) - 1];
                    a.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    var a = homeFoodBlocks[_foodCount - 1];
                    a.transform.GetChild(0).gameObject.SetActive(true);
                }
                break;
            }
            case false:
            {
                if (_foodCount - 1 < 0)
                {
                    var a = homeFoodBlocks[Mathf.Abs(_foodCount) - 1];
                    a.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    var a = homeFoodBlocks[_foodCount - 1];
                    a.transform.GetChild(0).gameObject.SetActive(false);
                }
                break;
            }
        }
    }
    
    #region Unit
    
    public List<UnitController> GetUnitList()
    {
        return _unitList;
    }
    
    private void BuyStartUnit()
    {
        if(!homeTile.IsTileFree() || _unitList.Count >= _unitCapacity)
            return;
        var stIndex = owner.civilisationInfo.StartUnitIndex;
        var unitObject = Instantiate(unitPrefabs[stIndex], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(this, homeTile, stIndex);
        unit.EnableUnit();
        AddUnit(unit);
    }
    
    private void BuyUnit(int unitIndex)
    {
        if(!homeTile.IsTileFree() || !homeTile.IsSelected() || _unitList.Count >= _unitCapacity)
            return;

        if (!owner.IsCanBuy(owner.civilisationInfo.Units[unitIndex].price)) 
            return;
        owner.BuySomething(owner.civilisationInfo.Units[unitIndex].price);
        
        var unitObject = Instantiate(unitPrefabs[unitIndex], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(this, homeTile, unitIndex);
        AddUnit(unit);
    }

    public void AddUnit(UnitController unit)
    {
        _unitList.Add(unit);
        ChangeUnitBlock(true);
    }
    
    public void RemoveUnit(UnitController unit)
    {
        _unitList.Remove(unit);
        ChangeUnitBlock(false);
    }

    public void AIBuyUnit(CivilisationController controller)
    {
        if(!homeTile.IsTileFree() || _unitList.Count >= _unitCapacity)
            return;
        
        var unitIndexForBuy = new List<int>(){0};
        
        if (controller.technologies.Contains(TechInfo.Technology.Rider))
        {
            unitIndexForBuy.Add(1);
        }
        if (controller.technologies.Contains(TechInfo.Technology.Strategy))
        {
            unitIndexForBuy.Add(2);
        }
        if (controller.technologies.Contains(TechInfo.Technology.Archery))
        {
            unitIndexForBuy.Add(3);
        }
        
        var randUnit = unitIndexForBuy[Random.Range(0, unitIndexForBuy.Count)];
        
        
        var unitObject = Instantiate(unitPrefabs[randUnit], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        if(!homeTile.isOpened)
            unitObject.gameObject.SetActive(false);
        var unit = unitObject.GetComponent<UnitController>();
        unit.aiName = "unit" + Random.Range(0, 1000000);
        unit.Init(this, homeTile, 0);
        AddUnit(unit);
    }

    
    #endregion
}