using System.Collections.Generic;
using Gameplay.SO;
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
    public int homeRad = 1;
    public HomeType homeType = HomeType.City;
    [SerializeField] private Button occupyButton;
    [SerializeField] private Image homeImage;
    [SerializeField] private List<UnitController> unitPrefabs;
    [SerializeField] private List<GameObject> homeFoodBlocks;
    [SerializeField] private GameObject centerBlockPrefab;
    [SerializeField] private GameObject blockScrollView;
    [SerializeField] private Transform blockParent;
    private List<UnitController> _unitList;
    private HomeInfo _homeInfo;
    private int _boardRad;
    
    private int _homeLevel = 0;
    private int _foodFromNextLvl = 2;
    private int _foodCount;
    private int _unitCapacity = 2;
    private int _homeIncome = 1;
    private bool _isCapital = true;
    
    public void Init(CivilisationController controller, Tile tile)
    {
        homeTile = tile;
        transform.position = homeTile.GetRectTransform().position;
        transform.SetSiblingIndex(transform.GetSiblingIndex()-2);
        _unitList = new List<UnitController>();
        if (controller != null)
        {
            _homeInfo = controller.civilisationInfo.Home;
            SetOwner(controller);
            blockScrollView.transform.SetParent(LevelManager.Instance.gameBoardWindow.GetUIParent());
            blockScrollView.SetActive(true);
            if(controller.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
                blockScrollView.SetActive(false);
            if(!controller.homes.Contains(this))
                controller.homes.Add(this);
            if (_homeLevel >= 0 && _homeLevel < _homeInfo.homeSprites.Count)
                UpdateVisual(_homeInfo.homeSprites[_homeLevel]);
            _boardRad = 1;
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
            homeTile.BuildHome(this);
            foreach (var ti in tiles)
            {
                if(ti.GetOwner() != null)
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
            LevelManager.Instance.gameplayWindow.OnUnitSpawn += BuyUnit;
            BuyStartUnit();
        }
        else
        {
            _isCapital = false;
            homeTile.BuildHome(this);
            blockScrollView.SetActive(false);
        }

        occupyButton.onClick.RemoveAllListeners();
        occupyButton.onClick.AddListener(OccupyHome);
    }
    
    public void SelectHome()
    {
        if (owner == null) return;
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            LevelManager.Instance.gameplayWindow.ShowHomeButton();
        }
    }
    
    public void GetFood(int count)
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
    
    public int GetIncome()
    {
        return _homeIncome;
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
    
    public void OccupyHome()
    {
        for (var i = _unitList.Count - 1; i >= 0; i--)
        {
            var unit = _unitList[i];
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

    public void UpdateVisual(Sprite sprite)
    {
        homeImage.sprite = sprite;
        
    }

    public List<UnitController> GetUnitList()
    {
        return _unitList;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.gameplayWindow.OnUnitSpawn -= BuyUnit;
    }
    
    private void LeveUp()
    {
        var block = Instantiate(centerBlockPrefab, blockParent);
        block.transform.SetSiblingIndex(blockParent.childCount-1);
        homeFoodBlocks.Insert(homeFoodBlocks.Count - 2, block);
        RemoveAllFood();
        _foodCount = 0;
        _foodFromNextLvl++;
        _homeLevel++;
        _unitCapacity++;
        ChangeIncome(1);
        if (_homeLevel < _homeInfo.homeSprites.Count)
            UpdateVisual(_homeInfo.homeSprites[_homeLevel]);
        
        void RemoveAllFood()
        {
            foreach (var foodBlock in homeFoodBlocks)
            {
                foodBlock.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
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
        if (isAdd)
        {
            var a = homeFoodBlocks[_foodCount-1];
            a.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            var a = homeFoodBlocks[_foodCount-1];
            a.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void ChangeIncome(int value)
    {
        _homeIncome += value;
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

        if (!EconomicManager.Instance.IsCanBuy(owner.civilisationInfo.Units[unitIndex].price)) 
            return;
        EconomicManager.Instance.BuySomething(owner.civilisationInfo.Units[unitIndex].price);
        
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
        unitObject.gameObject.SetActive(false);
        var unit = unitObject.GetComponent<UnitController>();
        unit.aiName = "unit" + Random.Range(0, 1000000);
        unit.Init(this, homeTile, 0);
        AddUnit(unit);
    }
    
    public UnitController exploringUnit;
}