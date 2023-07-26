using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button occupyButton;
    [SerializeField] private Image homeImage;
    [SerializeField] private List<UnitController> unitPrefabs;
    public Tile homeTile;
    public CivilisationController owner;
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
        homeButton.onClick.AddListener(HomeOnClick);
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
        occupyButton.gameObject.SetActive(false);
        Init(homeTile.unitOnTile.GetOwner().owner, homeTile);
    }

    public void UpdateVisual(Sprite sprite)
    {
        homeImage.sprite = sprite;
        
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
        //_unitList.Add(unit);
    }
    
}