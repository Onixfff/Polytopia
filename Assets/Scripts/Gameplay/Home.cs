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
    [SerializeField] private Button homeButton;
    [SerializeField] private Button occupyButton;
    [SerializeField] private Image homeImage;
    [SerializeField] private List<UnitController> unitPrefabs;
    private List<UnitController> _unitList;
    private HomeInfo _homeInfo;
    private int _homeLevel = 0;
    private int _foodFromNextLvl = 2;
    private int _foodCount = 2;
    private int _unitCapacity = 2;
    
    public void Init(CivilisationController controller, Tile tile, bool isFirstInit = true)
    {
        homeTile = tile;
        transform.position = homeTile.GetRectTransform().position;
        transform.SetSiblingIndex(transform.GetSiblingIndex()-2);
        _unitList = new List<UnitController>();
        if (controller != null)
        {
            _homeInfo = controller.civilisationInfo.home;
            SetOwner(controller);
            if(!controller.homes.Contains(this))
                controller.homes.Add(this);
            if (_homeLevel >= 0 && _homeLevel < _homeInfo.homeSprites.Count)
                UpdateVisual(_homeInfo.homeSprites[_homeLevel]);
            var rad = 1;
            if (isFirstInit)
            {
                rad = 3;
            }
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, rad);
            foreach (var ti in tiles)
            {
                ti.SetOwner(this);
                ti.ChangeTileVisual(this);
            }
            LevelManager.Instance.gameplayWindow.OnUnitSpawn += BuyUnit;
            EconomicManager.Instance.AddMoney(5);
            homeTile.isSelected = true;
            BuyUnit(0);
            homeTile.isSelected = false;
        }
        else
        {

        }
        homeTile.BuildHome(this);

        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(HomeOnClick);
        occupyButton.onClick.RemoveAllListeners();
        occupyButton.onClick.AddListener(OccupyHome);
    }
    
    public void GetFood(int count)
    {
        for (var i = 0; i < count; i++)
        {
            _foodCount++;
            if (_foodCount >= _foodFromNextLvl)
            {
                _foodCount = 0;
                _foodFromNextLvl++;
                _homeLevel++;
            }
        }

        if (_homeLevel >= 0 && _homeLevel < _homeInfo.homeSprites.Count)
            UpdateVisual(_homeInfo.homeSprites[_homeLevel]);
    }

    public void SetOwner(CivilisationController controller)
    {
        owner = controller;
    }

    public void ShowOccupyButton()
    {
        occupyButton.gameObject.SetActive(true);
    }
    
    public void HideOccupyButton()
    {
        occupyButton.gameObject.SetActive(false);
    }
    
    public void OccupyHome()
    {
        if(owner != null)
            owner.RemoveHome(this, _unitList);
        _unitList.Clear();
        homeType = HomeType.City;
        homeTile.unitOnTile.GetOwner().RemoveUnit(homeTile.unitOnTile);
        HideOccupyButton();
        Init(homeTile.unitOnTile.GetOwner().owner, homeTile, false);
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

    private void HomeOnClick()
    {
        homeTile.SelectTile();
        if (owner == null) return;
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
            LevelManager.Instance.gameplayWindow.ShowHomeButton();
    }
    
    private void BuyUnit(int unitIndex)
    {
        if(!homeTile.IsTileFree() || !homeTile.isSelected || _unitList.Count >= _unitCapacity)
            return;

        if (!EconomicManager.Instance.IsCanBuy(owner.civilisationInfo.units[unitIndex].price)) 
            return;
        EconomicManager.Instance.BuySomething(owner.civilisationInfo.units[unitIndex].price);
        
        var unitObject = Instantiate(unitPrefabs[unitIndex], homeTile.transform.parent);
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(this, homeTile, unitIndex);
        AddUnit(unit);
    }

    public void AddUnit(UnitController unit)
    {
        _unitList.Add(unit);
    }

    public void RemoveUnit(UnitController unit)
    {
        _unitList.Remove(unit);
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
        
        
        var unitObject = Instantiate(unitPrefabs[randUnit], homeTile.transform.parent);
        var unit = unitObject.GetComponent<UnitController>();
        unit.aiName = "unit" + Random.Range(0, 1000000);
        unit.Init(this, homeTile, 0);
        AddUnit(unit);
    }
    

    public UnitController exploringUnit;
}