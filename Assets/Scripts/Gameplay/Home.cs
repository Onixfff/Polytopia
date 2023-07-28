using System.Collections.Generic;
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
    
    public void Init(CivilisationController controller, Tile tile)
    {
        homeTile = tile;
        transform.position = homeTile.GetRectTransform().position;
        homeTile.ReplaceOwner(this);
        transform.SetSiblingIndex(transform.GetSiblingIndex()-2);
        _unitList = new List<UnitController>();
        if (controller != null)
        {
            _homeInfo = controller.civilisationInfo.home;
            SetOwner(controller);
            UpdateVisual(_homeInfo.homeSprites[_homeLevel]);
            homeTile.BuildHome(this);
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, 1);
            foreach (var ti in tiles)
            {
                ti.ReplaceOwner(this);
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
    
    public void OccupyHome()
    {
        homeType = HomeType.City;
        homeTile.unitOnTile.GetOwner().RemoveUnit(homeTile.unitOnTile);
        occupyButton.gameObject.SetActive(false);
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

    private void HomeOnClick()
    {
        homeTile.SelectTile();
        if (owner == null) return;
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
            LevelManager.Instance.gameplayWindow.ShowHomeButton();
    }
    
    private void BuyUnit(int unitIndex)
    {
        if(!homeTile.IsTileFree() || !homeTile.isSelected)
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

    public void AIBuyUnit()
    {
        if(!homeTile.IsTileFree())
            return;
        
        var unitObject = Instantiate(unitPrefabs[0], homeTile.transform.parent);
        var unit = unitObject.GetComponent<UnitController>();
        unit.aiName = "unit" + Random.Range(0, 1000000);
        unit.Init(this, homeTile, 0);
        AddUnit(unit);
    }

    public UnitController exploringUnit;
}