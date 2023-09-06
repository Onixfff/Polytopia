using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Ground,
        Water,
        DeepWater
    }
    
    public Action<Tile> OnClickOnTile;
    public UnitController unitOnTile;
    public BuildingUpgrade buildingUpgradePrefab;
    public Roads roads;
    public bool isHasMountain = false;
    public bool isHasOre = false;
    public bool isHasCrop = false;
    public bool isHasRuins = false;
    public bool isHasWhale = false;
    public bool isHasTree = false;
    public bool isHasMining = false;
    public TileType tileType = TileType.Ground;

    public bool isOpened;

    [SerializeField] private Image groundImage;
    [SerializeField] private Image fruitTileImage;
    [SerializeField] private Image treeTileImage;
    [SerializeField] private Image cropTileImage;
    [SerializeField] private Image animalTileImage;
    [SerializeField] private Image fishTileImage;
    [SerializeField] private Image mountainTileImage;
    [SerializeField] private Image ruinsTileImage;
    [SerializeField] private Image miningImage;
    [SerializeField] private Image freeTileImage;
    [SerializeField] private Image monumentImage;
    [SerializeField] private RectTransform centerRect;
    [SerializeField] private Image blueTargetImage;
    [SerializeField] private Image redTargetImage;
    [SerializeField] private Image selectedOutlineImage;
    
    
    [SerializeField] private GameObject fog;
    
    [SerializeField] private GameObject downLeft;
    [SerializeField] private GameObject downRight;
    [SerializeField] private GameObject upRight;
    [SerializeField] private GameObject upLeft;
    
    
    [SerializeField] private Button getInfoButton;
    [SerializeField] private Button ruinButton;

    private BuildingUpgrade _buildingUpgrade;
    
    private string _tileName = "Ground";
    private bool _isHomeOnTile;
    private bool _isSelected;
    private bool _isUnitSelected;
    private bool _isHomeSelected;
    private int _groundDefense = 0;
    private Home _homeOnTile;
    private Home _owner;
    private List<GameplayWindow.OpenedTechType> _techTypes;
    private Tween _timeTargetTween;

    public int GetGroundDefense()
    {
        _groundDefense = 1;
        if (_homeOnTile != null)
        {
            _groundDefense = 2;
            if (_homeOnTile.isHaveWall)
                _groundDefense = 4;
        }
        if (isHasMountain)
            _groundDefense = 2;
        if (unitOnTile != null)
        {
            if (unitOnTile.GetOwner().owner.technologies.Contains(TechInfo.Technology.Aqua) && (tileType == TileType.Water || tileType == TileType.DeepWater))
            {
                _groundDefense = 3;
            }
            if (unitOnTile.GetOwner().owner.technologies.Contains(TechInfo.Technology.Archery) && treeTileImage != null && treeTileImage.enabled && treeTileImage.gameObject.activeSelf)
            {
                _groundDefense = 3;
            }
        }

        return _groundDefense;
    }
    
    public bool IsSelected()
    {
        return _isSelected;
    }
    
    public Home GetOwner()
    {
        return _owner;
    }
    
    public Home GetHomeOnTile()
    {
        return _homeOnTile;
    }
    
    public void SetOwner(Home home)
    {
        _owner = home;
    }
    
    public void ChangeTileVisual(Home home)
    {
        if (home.owner != null)
        {
            var civilisationInfo = home.owner.civilisationInfo;
            if (tileType == TileType.Ground)
            {
                if (groundImage != null) groundImage.sprite = civilisationInfo.GroundSprite;
                if (animalTileImage != null) animalTileImage.sprite = civilisationInfo.AnimalSprite;
                if (treeTileImage != null) treeTileImage.sprite = civilisationInfo.TreeSprite;
                if (fruitTileImage != null) fruitTileImage.sprite = civilisationInfo.FruitSprite;
                if (mountainTileImage != null) mountainTileImage.sprite = civilisationInfo.MountainSprite;
            }
        }
    }
    
    public void SelectTile()
    {
        if (fog.activeSelf)
        {
            LevelManager.Instance.gameBoardWindow.DeselectAllTile();
            return;
        }
        if (_isSelected || _isUnitSelected || _isHomeSelected)
        {
            /*if (unitOnTile != null && _homeOnTile != null)
            {
                if (_isUnitSelected)
                {
                    DeselectedUnitOnTile();
                    _isSelected = true;
                    _isHomeSelected = true;
                    _homeOnTile.SelectHome();
                    return;
                }
                if (_isHomeSelected)
                {
                    DeselectedHomeOnTile();
                    _isSelected = true;
                    _isUnitSelected = true;
                    unitOnTile.SelectUnit();
                    return;
                }
            }*/
            LevelManager.Instance.SelectObject(null);
            DeselectedTile();
            return;
        }
        LevelManager.Instance.currentName = _tileName;
        selectedOutlineImage.gameObject.SetActive(true);
        if (unitOnTile != null)
        {
            _isSelected = true;
            _isUnitSelected = true;
            unitOnTile.SelectUnit();
        }
        else if(_homeOnTile != null)
        {
            _isSelected = true;
            _isHomeSelected = true;
            LevelManager.Instance.SelectObject(gameObject);
            _homeOnTile.SelectHome();
        }
        else
        {
            _isSelected = true;
            LevelManager.Instance.SelectObject(gameObject);
        }

        GetInfoTile();
    }
    
    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (ruinButton.gameObject.activeSelf && unitOnTile == null)
        {
            ruinButton.gameObject.SetActive(false);
        }
        
        if (currO == null || currO.TryGetComponent(out Tile tile))
        {
            HideTargetsTime();
        }
        
        if(!_isSelected && !_isUnitSelected && !_isHomeSelected) 
            return;
        
        if(pastO == gameObject || (unitOnTile != null && pastO == unitOnTile.gameObject))
            DeselectedTile();
        
        /*if(unitOnTile != null && pastO == unitOnTile.gameObject && (_homeOnTile == null || currO != _homeOnTile.gameObject))
            DeselectedUnitOnTile();
        
        if(_homeOnTile != null && pastO == _homeOnTile.gameObject && (unitOnTile == null || currO != unitOnTile.gameObject))
            DeselectedHomeOnTile();*/
        
        if (_owner != null && currO == gameObject && _homeOnTile == null && _owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            LevelManager.Instance.gameplayWindow.ShowTileButton(GetTileTypes(), this);
        }
    }

    public List<GameplayWindow.OpenedTechType> GetTileTypes()
    {
        _techTypes = new List<GameplayWindow.OpenedTechType>();
        if(fruitTileImage != null && fruitTileImage.enabled)
            _techTypes.Add(GameplayWindow.OpenedTechType.Fruit);
            
        if(treeTileImage != null && treeTileImage.enabled)
            _techTypes.Add(GameplayWindow.OpenedTechType.Tree);
            
        if(cropTileImage != null && cropTileImage.enabled && isHasCrop)
            _techTypes.Add(GameplayWindow.OpenedTechType.Crop);
            
        if(animalTileImage != null && animalTileImage.enabled)
            _techTypes.Add(GameplayWindow.OpenedTechType.Animal);
            
        if(fishTileImage != null && fishTileImage.enabled && !isHasWhale)
            _techTypes.Add(GameplayWindow.OpenedTechType.Fish);
            
        if(fishTileImage != null && fishTileImage.enabled && isHasWhale)
            _techTypes.Add(GameplayWindow.OpenedTechType.Whale);

        if(tileType == TileType.Water)
            _techTypes.Add(GameplayWindow.OpenedTechType.Water);
            
        if(tileType == TileType.DeepWater)
            _techTypes.Add(GameplayWindow.OpenedTechType.DeepWater);
            
        if(tileType == TileType.Ground)
            _techTypes.Add(GameplayWindow.OpenedTechType.Ground);
            
        if(freeTileImage.enabled)
            _techTypes.Add(GameplayWindow.OpenedTechType.Construct);
            
        if(monumentImage.enabled)
            _techTypes.Add(GameplayWindow.OpenedTechType.Monument);
        
        if(roads.isRoad)
            _techTypes.Add(GameplayWindow.OpenedTechType.Road);

        return _techTypes;
    }

    public BuildingUpgrade GetBuildingUpgrade()
    {
        return _buildingUpgrade;
    }
    
    public void DeselectedTile()
    {
        selectedOutlineImage.gameObject.SetActive(false);
        HideTargets();
        _isSelected = false;
        _isHomeSelected = false;
        _isUnitSelected = false;
    }
    
    public void DeselectedUnitOnTile()
    {
        selectedOutlineImage.gameObject.SetActive(false);
        _isUnitSelected = false;
    }
    
    public void DeselectedHomeOnTile()
    {
        selectedOutlineImage.gameObject.SetActive(false);
        _isHomeSelected = false;
    }
    
    public void UnlockTile(CivilisationController explorerCiv)
    {
        if (explorerCiv.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
        {
            explorerCiv.AddTileInExploreList(this);
        }
        else
        {
            isOpened = true;
            fog.SetActive(false);
            groundImage.enabled = true;
            if(isHasMountain && mountainTileImage != null) 
                mountainTileImage.gameObject.SetActive(true);
            if(isHasRuins && ruinsTileImage != null) 
                ruinsTileImage.gameObject.SetActive(true);
            if(_homeOnTile != null)
                _homeOnTile.gameObject.SetActive(true);
            if (unitOnTile != null)
            {
                unitOnTile.gameObject.SetActive(true);
                if (explorerCiv != unitOnTile.GetOwner().owner)
                {
                    explorerCiv.relationOfCivilisation.AddNewCivilisation(unitOnTile.GetOwner().owner, DiplomacyManager.RelationType.Neutral);
                    unitOnTile.GetOwner().owner.relationOfCivilisation.AddNewCivilisation(explorerCiv, DiplomacyManager.RelationType.None);

                }
            }
            ChangeHomeBoards();
            explorerCiv.GetCivilisationStats().AddExploredTile();
        }
        
    }
    
    public void ChangeHomeBoards()
    {
        if(_owner == null || _owner.owner == null)
            return;
        downLeft.SetActive(false);
        downRight.SetActive(false);
        upLeft.SetActive(false);
        upRight.SetActive(false);

        var board = LevelManager.Instance.gameBoardWindow;
        var DownLeft = board.GetTile(new Vector2Int(pos.x + 1, pos.y));
        var UpLeft = board.GetTile(new Vector2Int(pos.x - 1, pos.y));
        var DownRight = board.GetTile(new Vector2Int(pos.x, pos.y + 1));
        var UpRight = board.GetTile(new Vector2Int(pos.x, pos.y - 1));
        if (DownLeft == null || DownLeft._owner == null || DownLeft._owner.owner == null || (_owner != null && _owner.owner != null && DownLeft._owner.owner != _owner.owner))
        {
            if (isOpened)
            {
                downLeft.SetActive(true);
                downLeft.GetComponent<Image>().color = _owner.owner.civilisationInfo.CivilisationColor;
            }
        }
        if (UpLeft == null || UpLeft._owner == null || UpLeft._owner.owner == null || (_owner != null && _owner.owner != null && UpLeft._owner.owner != _owner.owner))
        {
            if (isOpened)
            {
                upRight.SetActive(true);
                upRight.GetComponent<Image>().color = _owner.owner.civilisationInfo.CivilisationColor;
            }
        }
        if (DownRight == null || DownRight._owner == null || DownRight._owner.owner == null || (_owner != null && _owner.owner != null && DownRight._owner.owner != _owner.owner))
        {
            if (isOpened)
            {
                downRight.SetActive(true);
                downRight.GetComponent<Image>().color = _owner.owner.civilisationInfo.CivilisationColor;
            }
        }
        if (UpRight == null || UpRight._owner == null || UpRight._owner.owner == null || (_owner != null && _owner.owner != null && UpRight._owner.owner != _owner.owner))
        {
            if (isOpened)
            {
                upLeft.SetActive(true);
                upLeft.GetComponent<Image>().color = _owner.owner.civilisationInfo.CivilisationColor;
            }
        }
    }
    
    public void SetTreeSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        treeTileImage.sprite = sprite;
        treeTileImage.enabled = true;
        isHasTree = true;
    }
    
    public void SetCropSprite()
    {
        if(_isHomeOnTile) return;
        cropTileImage.enabled = false;
        isHasCrop = true;
    }
    
    public void SetPumpkinSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        fruitTileImage.sprite = sprite;
        fruitTileImage.enabled = true;
    }
    
    public void SetAnimalSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        animalTileImage.sprite = sprite;
        animalTileImage.enabled = true;
    }
    
    public void SetMountainSprite(Sprite sprite, string tileName, bool isOre)
    {
        if(sprite == null || _isHomeOnTile) return;
        mountainTileImage.sprite = sprite;
        mountainTileImage.enabled = true;
        mountainTileImage.gameObject.SetActive(false);
        isHasMountain = true;
        isHasOre = isOre;
    }
    
    public void SetRuinsSprite(Sprite sprite, string tileName)
    {
        ruinsTileImage.gameObject.SetActive(false);
        ruinsTileImage.enabled = true;
        isHasRuins = true;
        ruinButton.onClick.RemoveAllListeners();
        ruinButton.onClick.AddListener(ExploreRuins);
    }

    public void ShowBlueTarget()
    {
        if (unitOnTile != null || fog.activeSelf)
            return;
        blueTargetImage.gameObject.SetActive(true);
    }

    public void ShowRedTarget(UnitController unit)
    {
        if(unitOnTile == null || fog.activeSelf) 
            return;
        redTargetImage.gameObject.SetActive(true);
        if (unitOnTile.CheckForKill(unit.GetDmg()))
        {
            unitOnTile.SweatingAnimationEnable();
        }
    }
    
    public void ShowRuinButton()
    {
        LevelManager.Instance.OnTurnBegin += Show;

        void Show()
        {
            ruinButton.gameObject.SetActive(true);
            LevelManager.Instance.OnTurnBegin -= Show;
        }
    }
    
    public void HideTargets()
    {
        blueTargetImage.gameObject.SetActive(false);
        redTargetImage.gameObject.SetActive(false);
        if (unitOnTile != null)
        {
            //unitOnTile.SweatingAnimationDisable();
        }
    }
    
    public bool IsTileFree()
    {
        return unitOnTile == null;
    }

    public bool IsTileHasPort()
    {
        return _tileName.Contains("Port");
    }
    
    public bool IsCanMoveTo()
    {
        return blueTargetImage.gameObject.activeSelf && !redTargetImage.gameObject.activeSelf;
    }

    public void ShowOre()
    {
        if (miningImage == null) 
            return;
        miningImage.gameObject.SetActive(true);
        miningImage.enabled = true;
    }
    
    public void ShowCrop()
    {
        if (cropTileImage == null) 
            return;
        cropTileImage.gameObject.SetActive(true);
        cropTileImage.enabled = true;
    }

    public bool IsCanAttackTo()
    {
        return redTargetImage.gameObject.activeSelf;
    }
    
    public RectTransform GetRectTransform()
    {
        return centerRect;
    }

    public void BuildHome(Home home)
    {
        _homeOnTile = home;
        SetOwner(home);
        ChangeTileVisual(home);

        isHasMountain = false;
        isHasCrop = false;
        isHasTree = false;
        isHasRuins = false;

        if (treeTileImage != null) Destroy(treeTileImage.gameObject);
        if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
        if (animalTileImage != null) Destroy(animalTileImage.gameObject);
        if (mountainTileImage != null) Destroy(mountainTileImage.gameObject);
        if (ruinsTileImage != null) Destroy(ruinsTileImage.gameObject);
        if (cropTileImage != null) Destroy(cropTileImage.gameObject);
        if (_owner.owner != null) 
            groundImage.sprite = _owner.owner.civilisationInfo.GroundSprite;
        tileType = TileType.Ground;
        _isHomeOnTile = true;
        _tileName = "Home";
    }

    public void HideTargetsTime()
    {
        var inValY = 0;
        _timeTargetTween = DOTween.To(() => inValY, x => inValY = x, 0, 0.01f).OnComplete((() =>
        {
            blueTargetImage.gameObject.SetActive(false);
            redTargetImage.gameObject.SetActive(false);   
        }));
    }
    
    public void BuyTileTech(int index, CivilisationInfo.ControlType controlType)
    {
        if(_owner == null)
            return;
        if(_owner.owner == null)
            return;
        
        if(controlType == CivilisationInfo.ControlType.Player && gameObject != LevelManager.Instance.GetSelectedObject())
            return;

        if (controlType == CivilisationInfo.ControlType.AI)
        {
        }
        
        BuildingUpgrade building = null;
        var buildingUpgrades = new List<BuildingUpgrade>();
        switch (index)
        {
            case 0:
                if (!_owner.owner.IsCanBuy(2)) 
                    return;
                _owner.owner.BuySomething(2);
                BuyFruit();
                _tileName = "Ground";
                break;
            case 1:
                if (!_owner.owner.IsCanBuy(2)) 
                    return;
                _owner.owner.BuySomething(2);
                BuyFish();
                _tileName = "Water";
                break;
            case 2:
                if (!_owner.owner.IsCanBuy(2)) 
                    return;
                _owner.owner.BuySomething(2);
                BuyAnimal();
                _tileName = "Ground";
                break;
            case 3:
                if (!_owner.owner.IsCanBuy(2)) 
                    return;
                _owner.owner.BuySomething(2);
                roads.isRoad = true;
                roads.CheckCloseTile();
                break;
            case 4:
                if (!_owner.owner.IsCanBuy(10)) 
                    return;
                _owner.owner.BuySomething(10);
                
                if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);

                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.Church);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[4];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                
                _tileName = "Ground";
                
                break;
            case 5:
                if (!_owner.owner.IsCanBuy(10)) 
                    return;
                _owner.owner.BuySomething(10);
                
                if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[5];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                cropTileImage.enabled = false;
                cropTileImage.gameObject.SetActive(false);
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.Farm);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.WindMill);
                foreach (var buildingUpgrade in buildingUpgrades)
                {
                    buildingUpgrade.AddLevelToBuilding(1);
                }
                
                _tileName = "Ground";
                break;
            case 6:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                
                if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                miningImage.sprite = _owner.owner.civilisationInfo.BuildSprites[6];
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                isHasMining = true;
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.Mine);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.Forge);
                foreach (var buildingUpgrade in buildingUpgrades)
                {
                    buildingUpgrade.AddLevelToBuilding(1);
                }
                _tileName = "Ground";
                break;
            case 7:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[5];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.MountainChurch);
                _tileName = "MountainChurch";
                
                break;
            case 8:
                if (!_owner.owner.IsCanBuy(10)) 
                    return;
                _owner.owner.BuySomething(10);
                if (fishTileImage != null) Destroy(fishTileImage.gameObject);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[8];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.Port);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.TradeHouse);
                foreach (var buildingUpgrade in buildingUpgrades)
                {
                    buildingUpgrade.AddLevelToBuilding(1);
                }
                
                
                _tileName = "Port";
                break;
            case 9:
                if (!_owner.owner.IsCanBuy(2)) 
                    return;
                _owner.owner.BuySomething(2);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[5];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                BuyFish();
                _owner.owner.AddMoney(10);
                _tileName = "Ground";
                break;
            case 10:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[10];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.LumberHut);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.SawMill);
                foreach (var buildingUpgrade in buildingUpgrades)
                {
                    _owner.AddFood(1);
                    buildingUpgrade.AddLevelToBuilding(1);
                }
                break;
            case 11:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                treeTileImage.enabled = false;
                treeTileImage.gameObject.SetActive(false);
                _owner.owner.AddMoney(3);
                _owner.owner.BuySomething(5);
                break;
            case 12:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[12];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.TradeHouse);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.Port);
                _buildingUpgrade.AddLevelToBuilding(buildingUpgrades.Count);
                break;
            case 13:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                treeTileImage.enabled = false;
                treeTileImage.gameObject.SetActive(false);
                cropTileImage.enabled = true;
                cropTileImage.gameObject.SetActive(true);
                isHasCrop = true;
                break;
            case 14:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[14];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.WindMill);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.Farm);
                _buildingUpgrade.AddLevelToBuilding(buildingUpgrades.Count);
                break;
            case 15:

                if (_buildingUpgrade != null)
                {
                    Destroy(_buildingUpgrade.gameObject);
                    freeTileImage.enabled = false;
                    freeTileImage.gameObject.SetActive(false);
                }
                break;
            case 16:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[16];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.Forge);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.Mine);
                _buildingUpgrade.AddLevelToBuilding(buildingUpgrades.Count);
                break;
            case 17:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[17];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.WaterChurch);
                break;
            case 18:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                treeTileImage.gameObject.SetActive(true);
                treeTileImage.enabled = true;
                _owner.owner.BuySomething(5);
                break;
            case 19:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[19];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.ForestChurch);
                break;
            case 20:
                if (!_owner.owner.IsCanBuy(5)) 
                    return;
                _owner.owner.BuySomething(5);
                freeTileImage.sprite = _owner.owner.civilisationInfo.BuildSprites[20];
                freeTileImage.enabled = true;
                freeTileImage.gameObject.SetActive(true);
                building = Instantiate(buildingUpgradePrefab, freeTileImage.transform);
                _buildingUpgrade = building;
                _buildingUpgrade.Init(BuildingUpgrade.BuildType.SawMill);
                buildingUpgrades = GetCountCloseBuildingOfType(BuildingUpgrade.BuildType.LumberHut);
                _buildingUpgrade.AddLevelToBuilding(buildingUpgrades.Count);
                _owner.AddFood(1);
                break;
            case 21:
                BuildMonument(MonumentBuilder.MonumentType.AltarOfPeace, 0);
                break;
            case 22:
                BuildMonument(MonumentBuilder.MonumentType.EmperorTomb, 1);
                break;
            case 23:
                BuildMonument(MonumentBuilder.MonumentType.EyeOfGod, 2);
                break;
            case 24:
                BuildMonument(MonumentBuilder.MonumentType.GateOfPower, 3);
                break;
            case 25:
                BuildMonument(MonumentBuilder.MonumentType.GrandBazaar, 4);
                break;
            case 26:
                BuildMonument(MonumentBuilder.MonumentType.ParkOfFortune, 5);
                break;
            case 27:
                BuildMonument(MonumentBuilder.MonumentType.TowerOfWisdom, 6);
                break;
        }
        
        List<BuildingUpgrade> GetCountCloseBuildingOfType(BuildingUpgrade.BuildType type)
        {
            var buildingUpgrades = new List<BuildingUpgrade>();
            var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, 1);
            foreach (var tile in closeTile)
            {
                if (tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType == type)
                    buildingUpgrades.Add(tile.GetBuildingUpgrade());
            }
            
            return buildingUpgrades;
        }
    }
    
    private void Start()
    {
        getInfoButton.onClick.AddListener(SelectTile);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
        LevelManager.Instance.gameplayWindow.OnTileTech += BuyTileTech;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
        LevelManager.Instance.gameplayWindow.OnTileTech -= BuyTileTech;
        _timeTargetTween.Kill();
    }
    
    private void GetInfoTile()
    {
        OnClickOnTile?.Invoke(this);
    }

    public void CreateWaterArea(int waterRad)
    {
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, waterRad);
        
        if (_homeOnTile != null)
            return;
        
        tileType = TileType.Water;
        
        foreach (var tile in tiles)
        {
            if(tile._owner == null)
                tile.tileType = TileType.Water;
        }

        SetWaterTile();
        
        foreach (var tile in tiles)
        {
            if(tile._owner == null)
                tile.SetWaterTile();
        }
    }
    
    public void SetWaterTile()
    {
        isHasMountain = false;
        if (treeTileImage != null) treeTileImage.gameObject.SetActive(false);
        if (fruitTileImage != null) fruitTileImage.gameObject.SetActive(false);
        if (animalTileImage != null) animalTileImage.gameObject.SetActive(false);
        if (miningImage != null) miningImage.gameObject.SetActive(false);
        if (mountainTileImage != null)
        {
            mountainTileImage.gameObject.SetActive(false);
            isHasMountain = false;
            isHasOre = false;
        }
        if (cropTileImage != null)
        {
            cropTileImage.gameObject.SetActive(false);
            isHasCrop = false;
        }
        var board = LevelManager.Instance.gameBoardWindow;
        var tiles = board.GetSideTile(this, 1);

        if (tiles.TrueForAll(tile => tile.tileType == TileType.Water || tile.tileType == TileType.DeepWater))
        {
            _tileName = "DeepWater";
            tileType = TileType.DeepWater;
            groundImage.sprite = LevelManager.Instance.waterSprites[1];
        }
        else
        {
            _tileName = "Water";
            groundImage.sprite = LevelManager.Instance.waterSprites[0];
        }
        
        var randomFish = UnityEngine.Random.Range(0, 6);
        switch (randomFish)
        {
            case 1:
                if (tileType == TileType.Water)
                {
                    fishTileImage.gameObject.SetActive(true);
                    fishTileImage.enabled = true;
                }
                if (tileType == TileType.DeepWater)
                {
                    fishTileImage.gameObject.SetActive(true);
                    fishTileImage.enabled = false;
                    fishTileImage.transform.GetChild(0).gameObject.SetActive(true);
                    isHasWhale = true;
                }
                break;
        }
    }

    public bool BuyFruit()
    {
        if(fruitTileImage == null)
            return false;
        Destroy(fruitTileImage.gameObject);
        _owner.AddFood(1);
        return true;
    }
    
    public bool BuyAnimal()
    {
        if(animalTileImage == null)
            return false;

        Destroy(animalTileImage.gameObject);
        _owner.AddFood(1);
        return true;
    }
    
    public bool BuyFish()
    {
        if(fishTileImage == null)
            return false;
        Destroy(fishTileImage.gameObject);
        _owner.AddFood(1);
        return true;
    }

    private void BuildMonument(MonumentBuilder.MonumentType type, int index)
    {
        _owner.owner.GetMonumentBuilder().BlockMonument(type);
        monumentImage.enabled = true;
        monumentImage.gameObject.SetActive(true);
        monumentImage.sprite = _owner.owner.civilisationInfo.MonumentSprites[index];
        _owner.AddFood(3);
        _owner.owner.AddPoint(400);
    }

    private void ExploreRuins()
    {
        ruinButton.onClick.RemoveAllListeners();
        ruinButton.gameObject.SetActive(false);
        ruinsTileImage.enabled = false;
        ruinsTileImage.gameObject.SetActive(false);
        isHasRuins = false;
        for (var i = 0; i < 30; i++)
        {
            var rand = Random.Range(0, 6);
            if (tileType == TileType.Ground)
            {
                if (rand == 0)
                {
                    unitOnTile.GetOwner().owner.AddMoney(10);
                    return;
                }
                if (unitOnTile.GetOwner().owner.capitalHome.owner == unitOnTile.GetOwner().owner)
                {
                    if (rand == 1)
                    {
                        unitOnTile.GetOwner().owner.capitalHome.CreateIndependentUnit(this, 6);
                        return;
                    }

                    if (rand == 2)
                    {
                        unitOnTile.GetOwner().owner.capitalHome.AddFood(3);
                        return;
                    }
                }

                if (rand == 3)
                {
                    unitOnTile.GetOwner().CreateScout(this);
                    return;
                }
            }

            if (rand == 4)
            {
                LevelManager.Instance.gameplayWindow.OpenRandomTechnology();
                return;
            }
            if (tileType is TileType.Water or TileType.DeepWater)
            {
                if (rand == 5)
                {
                    var unit = unitOnTile.GetOwner().owner.capitalHome.CreateIndependentUnit(this, 0);
                    unit.TurnIntoAShip(2);
                    return;
                }
            }
        }
    }
    
    #region ForA-star

    public Vector2Int pos;
    public bool obstacle;
    public float gCost = float.PositiveInfinity;
    public float hCost = float.PositiveInfinity;
    public float fCost = float.PositiveInfinity;
    public Tile previousTile;
    
    public void CalculateCost(Tile startTile, Tile targetTile)
    {
        gCost = Mathf.Abs(pos.x - startTile.pos.x) + Mathf.Abs(pos.y - startTile.pos.y);
        hCost = Mathf.Abs(pos.x - targetTile.pos.x) + Mathf.Abs(pos.y - targetTile.pos.y);
        fCost = gCost + hCost;
    }

    #endregion
}
