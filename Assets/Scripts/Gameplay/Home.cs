using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public bool isHaveWall = false;
    public bool isIndependent = false;
    [SerializeField] private Button occupyButton;
    [SerializeField] private List<UnitController> unitPrefabs;
    [SerializeField] private List<UnitController> cloakUnitsPrefabs;
    [SerializeField] private Scout scoutPrefab;
    [SerializeField] private List<GameObject> homeFoodBlocks;
    [SerializeField] private List<GameObject> buildingPrefab;
    [SerializeField] private GameObject centerBlockPrefab;
    [SerializeField] private GameObject blockScrollView;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private Transform blockParent;

    #region private

    private List<UnitController> _unitList;
    private List<Tile> _controlledTiles;
    private HomeInfo _homeInfo;
    private HomeCreator _homeCreator;
    //private int _boardRad;
    
    private int _homeLevel = 0;
    private int _foodFromNextLvl = 2;
    private int _foodCount;
    private int _unitCapacity = 2;
    private bool _isCapital = true;
    private bool _isWorkshop = false;

    #endregion

    public void Init(CivilisationController controller, Tile tile)
    {
        if (!isIndependent)
        {
            homeTile = tile;
            SetHomePos();
            
            _unitList ??= new List<UnitController>();
            _controlledTiles ??= new List<Tile>();
            if (controller != null)
            {
                SetOwner(controller);
                if(owner.capitalHome != null)
                    _isCapital = false;
                _homeInfo = controller.civilisationInfo.Home;
                InitHomeCreator();
                
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
        }
        else
        {
            _isCapital = false;
            SetOwner(controller);
            _homeInfo = controller.civilisationInfo.Home;
            _unitList ??= new List<UnitController>();
        }
        
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
            if (_isCapital)
            {
                owner.capitalHome = this;
            }
            var _boardRad = 1;
            var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, _boardRad);
            homeTile.BuildHome(this);
            foreach (var ti in tiles)
            {
                if(ti.GetOwner() != null && ti.GetOwner() != this)
                    continue;
                ti.SetOwner(this);
                _controlledTiles.Add(ti);
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
            _controlledTiles.Add(homeTile);
            blockScrollView.SetActive(false);
        }
        
        #endregion
    }
    
    public List<Tile> GetControlledTiles()
    {
        _controlledTiles ??= new List<Tile>();
        return _controlledTiles;
    }

    public void SelectHome()
    {
        if (owner == null) return;
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            if(homeTile.IsTileFree())
                LevelManager.Instance.gameplayWindow.ShowHomeButton();
        }
    }

    public void AddFood(int count)
    {
        var a = count;
        for (var i = 0; i < count; i++)
        {
            _foodCount++;
            a--;
            ChangeFoodBlock(true);
            if (_foodCount >= _foodFromNextLvl)
            {
                LeveUp(a);
                return;
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
                
            }
        }
    }
    
    public int GetIncome()
    {
        if(isIndependent)
            return 0;
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

    public void CreateScout(Tile tile)
    {
        var scout = Instantiate(scoutPrefab, transform);
        scout.rectTransform.anchoredPosition = new Vector2(tile.GetRectTransform().anchoredPosition.x, tile.GetRectTransform().anchoredPosition.y);
        scout.transform.SetParent(LevelManager.Instance.gameBoardWindow.GetUnitParent());
        scout.gameObject.SetActive(true);
        scout.StartExploring(this, tile);
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

    public void CreateHomeWall()
    {
        isHaveWall = true;
        wallObject.SetActive(true);
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
            _controlledTiles.Add(ti);
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
    }
    
    public void CreateSuperUnit()
    {
        if (!homeTile.IsTileFree())
        {
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, 1);
            var freeTile = closeTile.Find(tile => tile.IsTileFree());
            if (freeTile != null)
                homeTile.unitOnTile.MoveToTile(freeTile);
            else
                homeTile.unitOnTile.TakeDamage(null, 10000);
        }
        var unitObject = Instantiate(unitPrefabs[9], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(owner.independentHome, homeTile, true);
        owner.independentHome.AddUnit(unit);
    }
    
    public UnitController CreateIndependentUnit(Tile tile, int index)
    {
        if (!tile.IsTileFree())
        {
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(tile, 1);
            var freeTile = closeTile.FindAll(tile => tile.IsTileFree());
            if (tile.tileType != Tile.TileType.Water || tile.tileType != Tile.TileType.DeepWater)
            {
                freeTile.RemoveAll(til => til.tileType == Tile.TileType.Water);
                freeTile.RemoveAll(til => til.tileType == Tile.TileType.DeepWater);
            }
            else
                freeTile.RemoveAll(til => til.tileType is Tile.TileType.Ground);
            if (freeTile[0] != null)
                tile.unitOnTile.MoveToTile(freeTile[0]);
            else
                tile.unitOnTile.TakeDamage(null, 10000);
        }
        var unitObject = Instantiate(unitPrefabs[index], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        var unit = unitObject.GetComponent<UnitController>();
        unit.Init(owner.independentHome, tile, true);
        owner.independentHome.AddUnit(unit);
        return unit;
    }

    [Button()]
    private void aLevelUp()
    {
        LeveUp(0);
    }
    
    private void LeveUp(int leftovers)
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
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            var alert = LevelManager.Instance.gameplayWindow.alertWindow;
            alert.HomeLevelUp(this, _homeLevel, leftovers);
        }
        else
        {
            var rand = Random.Range(0, 3);
            if (_homeLevel == 1)
            {
                BuildForge();
            }

            if (_homeLevel == 2)
            {
                if (rand == 0)
                    owner.AddMoney(5);
                else
                    CreateHomeWall();
            }

            if (_homeLevel == 3)
            {
                if (rand == 0)
                    AddFood(3);
                else
                    IncreaseBoarder();
            }

            if (_homeLevel >= 4)
            {
                if (rand == 0)
                    BuildPark();
                else
                    CreateSuperUnit();
            }
            AddFood(leftovers);
        }
        
        ChangeUnitBlock();
        
        void RemoveAllFood()
        {
            foreach (var foodBlock in homeFoodBlocks)
            {
                foodBlock.transform.GetChild(0).gameObject.SetActive(false);
            }
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
            unit.SetOwner(owner.independentHome);
            RemoveUnit(unit);
        }

        var gameplayWindow = LevelManager.Instance.gameplayWindow;
        if (owner != null)
        {
            owner.RemoveHome(this, _unitList);
        }
        
        homeType = HomeType.City;
        homeTile.unitOnTile.GetOwner().RemoveUnit(homeTile.unitOnTile);
        homeTile.unitOnTile.DisableUnit();
        HideOccupyButton();
        Init(homeTile.unitOnTile.GetOwner().owner, homeTile);
        homeTile.unitOnTile.SetOwner(this);
        AddUnit(homeTile.unitOnTile);
        if (owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            gameplayWindow.ShowCaptureHomeAlert();
        }
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
            if(homeTile.unitOnTile == null)
                return;
            if(homeTile.unitOnTile.GetOwner().owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
                return;

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
    
    private void ChangeUnitBlock()
    {
        if(isIndependent)
            return;
        foreach (var homeFoodBlock in homeFoodBlocks)
        {
            homeFoodBlock.transform.GetChild(1).gameObject.SetActive(false);
        }

        for (var i = 0; i < _unitList.Count; i++)
        {
            homeFoodBlocks[i].transform.GetChild(1).gameObject.SetActive(true);
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

    public void Infiltrate(Home home)
    {
        for (var i = 0; i <= _homeLevel; i++)
        {
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(homeTile, 2);
            closeTile.RemoveAll(tile => !tile.IsTileFree());
            closeTile.RemoveAll(tile => tile.tileType == Tile.TileType.DeepWater);
            closeTile.RemoveAll(tile => tile.isHasMountain && !home.owner.technologies.Contains(TechInfo.Technology.Mountain));
            if(closeTile.Count <= 0)
                break;
            
            var rand = Random.Range(0, closeTile.Count);
            var randTile = closeTile[rand];
            var parent = LevelManager.Instance.gameBoardWindow.GetUnitParent();
            UnitController unit = null;
            if (randTile.tileType == Tile.TileType.Ground)
            {
                unit = Instantiate(cloakUnitsPrefabs[0], parent);
            }
            else if(randTile.tileType == Tile.TileType.Water)
            {
                unit = Instantiate(cloakUnitsPrefabs[1], parent);
            }

            if (unit != null)
            {
                unit.Init(home.owner.independentHome, randTile, true);
                home.owner.independentHome.AddUnit(unit);
            }
        }
    }
    
    private void BuyStartUnit()
    {
        if(isIndependent)
            return;
        if(!homeTile.IsTileFree() || _unitList.Count >= _unitCapacity)
            return;
        var stIndex = owner.civilisationInfo.StartUnitIndex;
        var unitController = Instantiate(unitPrefabs[stIndex], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        unitController.Init(this, homeTile, false);
        unitController.EnableUnit();
        AddUnit(unitController);
    }
    
    public void BuyUnit(int unitIndex, CivilisationInfo.ControlType controlType)
    {
        if(isIndependent)
            return;
        if(!homeTile.IsTileFree() || _unitList.Count >= _unitCapacity)
            return;
        if(!homeTile.IsSelected() && controlType == CivilisationInfo.ControlType.Player)
            return;

        if (!owner.IsCanBuy(owner.civilisationInfo.Units[unitIndex].price)) 
            return;
        owner.BuySomething(owner.civilisationInfo.Units[unitIndex].price);
        
        var unit = Instantiate(unitPrefabs[unitIndex], LevelManager.Instance.gameBoardWindow.GetUnitParent());
        unit.Init(this, homeTile, false);
        AddUnit(unit);
    }

    public void AddUnit(UnitController unit)
    {
        _unitList ??= new List<UnitController>();
        _unitList.Add(unit);
        ChangeUnitBlock();
    }
    
    public void RemoveUnit(UnitController unit)
    {
        if (_unitList != null)
        {
            _unitList.Remove(unit);
            ChangeUnitBlock();
        }
    }

    #endregion

    #region Anim
    
    [SerializeField] private AnimationCurve unitSelectAnimationCurve;
    [SerializeField] private float unitSelectAnimHeight = 20f;
    [SerializeField] private float unitSelectAnimTime = 0.2f;
    
    private Tween _selectJump;
    public void AnimSelect()
    {
        if(_selectJump != null) return;
        
        _selectJump = homeTile.transform.DOLocalMoveY(homeTile.transform.localPosition.y + unitSelectAnimHeight, unitSelectAnimTime).SetEase(unitSelectAnimationCurve).OnComplete((
            () =>
            {
                _selectJump.Kill();
                _selectJump = null;
            }));
    }

    #endregion
}