using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Ground,
        Water
    }
    
    public Action<Tile> OnClickOnTile;
    public UnitController unitOnTile;
    public bool isSelected = false;
    public bool isHasMountain = false;
    public TileType tileType = TileType.Ground;
    public Home homeOnTile;
    
    public bool isExplored = false;
    public Tile isExploredFrom;

    [SerializeField] private Image groundImage;
    [SerializeField] private Image fruitTileImage;
    [SerializeField] private Image treeTileImage;
    [SerializeField] private Image animalTileImage;
    [SerializeField] private Image mountainTileImage;
    [SerializeField] private Image miningImage;
    [SerializeField] private Image freeTileImage;
    [SerializeField] private Image fishTileImage;
    [SerializeField] private RectTransform centerRect;
    [SerializeField] private Image blueTargetImage;
    [SerializeField] private Image redTargetImage;
    [SerializeField] private Image selectedOutlineImage;
    [SerializeField] private GameObject fog;
    [SerializeField] private Button getInfoButton;

    private string _tileName = "Ground";
    private bool _isHomeOnTile = false;
    private Home _owner;
    private List<GameplayWindow.OpenedTechType> _techTypes;
    
    public Home GetOwner()
    {
        return _owner;
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
                if (groundImage != null) groundImage.sprite = civilisationInfo.groundSprite;
                if (animalTileImage != null) animalTileImage.sprite = civilisationInfo.animalSprite;
                if (treeTileImage != null) treeTileImage.sprite = civilisationInfo.treeSprite;
                if (fruitTileImage != null) fruitTileImage.sprite = civilisationInfo.fruitSprite;
                if (mountainTileImage != null) mountainTileImage.sprite = civilisationInfo.mountainSprite;
            }
        }
    }
    
    public void SelectTile()
    {
        if (fog.activeSelf || isSelected)
        {
            LevelManager.Instance.SelectObject(null);
            return;
        }
        isSelected = true;
        LevelManager.Instance.currentName = _tileName;
        LevelManager.Instance.SelectObject(gameObject);
        selectedOutlineImage.gameObject.SetActive(true);

        GetInfoTile();
    }
    
    public void UnlockTile()
    {
        fog.SetActive(false);
        groundImage.enabled = true;
    }

    public void SetTreeSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        treeTileImage.sprite = sprite;
        treeTileImage.enabled = true;
        
        if (_tileName != "Ground")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetPumpkinSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        fruitTileImage.sprite = sprite;
        fruitTileImage.enabled = true;
        
        if (_tileName != "Ground")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetAnimalSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        animalTileImage.sprite = sprite;
        animalTileImage.enabled = true;
        
        if (_tileName != "Ground")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetMountainSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        mountainTileImage.sprite = sprite;
        mountainTileImage.enabled = true;
        mountainTileImage.gameObject.SetActive(true);
        isHasMountain = true;
        if (_tileName != "Ground")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }

    public void ShowBlueTarget()
    {
        if (unitOnTile != null || fog.activeSelf)
            return;
        blueTargetImage.gameObject.SetActive(true);
    }

    public void ShowRedTarget()
    {
        if(unitOnTile == null || fog.activeSelf) 
            return;
        redTargetImage.gameObject.SetActive(true);
    }
    
    public void HideTargets()
    {
        blueTargetImage.gameObject.SetActive(false);
        redTargetImage.gameObject.SetActive(false);
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
        homeOnTile = home;
        SetOwner(home);
        ChangeTileVisual(home);
        if (treeTileImage != null) Destroy(treeTileImage.gameObject);
        if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
        if (animalTileImage != null) Destroy(animalTileImage.gameObject);
        if (mountainTileImage != null) Destroy(mountainTileImage.gameObject);
        if (_owner.owner != null) 
            groundImage.sprite = _owner.owner.civilisationInfo.groundSprite;
        tileType = TileType.Ground;
        _isHomeOnTile = true;
        _tileName = "Home";
    }

    public void HideTargetsTime()
    {
        var inValY = 0;
        DOTween.To(() => inValY, x => inValY = x, 0, 0.01f).OnComplete((() =>
        {
            blueTargetImage.gameObject.SetActive(false);
            redTargetImage.gameObject.SetActive(false);   
        }));
    }
    
    private void Start()
    {
        getInfoButton.onClick.AddListener(SelectTile);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
        LevelManager.Instance.gameplayWindow.OnTileTech += BuyTileTech;
    }
    
    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if(currO == null || currO.TryGetComponent(out Tile tile))
            HideTargetsTime();
        
        if(isSelected == false) return;
        if(pastO == gameObject)
            DeselectedTile();

        if (_owner != null && currO == gameObject)
        {
            
            _techTypes = new List<GameplayWindow.OpenedTechType>();
            if(fruitTileImage != null && fruitTileImage.enabled)
                _techTypes.Add(GameplayWindow.OpenedTechType.Fruit);
            
            if(treeTileImage != null && treeTileImage.enabled)
                _techTypes.Add(GameplayWindow.OpenedTechType.Tree);
            
            if(animalTileImage != null && animalTileImage.enabled)
                _techTypes.Add(GameplayWindow.OpenedTechType.Animal);
            
            if(fishTileImage != null && fishTileImage.enabled)
                _techTypes.Add(GameplayWindow.OpenedTechType.Fish);

            if(tileType == TileType.Water)
                _techTypes.Add(GameplayWindow.OpenedTechType.Water);
            
            if(tileType == TileType.Ground)
                _techTypes.Add(GameplayWindow.OpenedTechType.Ground);
            
            if(freeTileImage.enabled)
                _techTypes.Add(GameplayWindow.OpenedTechType.Construct);
            
            LevelManager.Instance.gameplayWindow.ShowTileButton(_techTypes, this);
        }
    }

    private void DeselectedTile()
    {
        selectedOutlineImage.gameObject.SetActive(false);
        isSelected = false;
    }

    private void GetInfoTile()
    {
        OnClickOnTile?.Invoke(this);
    }
    
    private void BuyTileTech(int index)
    {
        if(gameObject != LevelManager.Instance.GetSelectedObject())
            return;
        switch (index)
        {
            case 0:
                if (!EconomicManager.Instance.IsCanBuy(2)) 
                    return;
                EconomicManager.Instance.BuySomething(2);
                BuyFruit();
                _tileName = "Ground";
                break;
            case 1:
                if (!EconomicManager.Instance.IsCanBuy(2)) 
                    return;
                EconomicManager.Instance.BuySomething(2);
                BuyAnimal();
                _tileName = "Ground";

                break;
            case 2:
                if (!EconomicManager.Instance.IsCanBuy(2)) 
                    return;
                EconomicManager.Instance.BuySomething(2);
                BuyFish();
                _tileName = "Water";

                break;
            case 3:
               
                if (_techTypes.TrueForAll(tech => tech == GameplayWindow.OpenedTechType.Ground))
                {
                    if (!EconomicManager.Instance.IsCanBuy(10)) 
                        return;
                    EconomicManager.Instance.BuySomething(10);
                    if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                    if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                    freeTileImage.sprite = _owner.owner.civilisationInfo.churchSprite;
                    freeTileImage.enabled = true;
                    freeTileImage.gameObject.SetActive(true);
                    _tileName = "Ground";
                    _owner.GetFood(2);
                }
                break;
            case 4:
                
                if (_techTypes.Contains(GameplayWindow.OpenedTechType.Water))
                {
                    if (!EconomicManager.Instance.IsCanBuy(10)) 
                        return;
                    EconomicManager.Instance.BuySomething(10);
                    if (fishTileImage != null) Destroy(fishTileImage.gameObject);
                    freeTileImage.sprite = _owner.owner.civilisationInfo.portSprite;
                    freeTileImage.enabled = true;
                    freeTileImage.gameObject.SetActive(true);
                    _tileName = "Port";
                    _owner.GetFood(2);
                }
                break;
            case 5:
                if (_techTypes.Contains(GameplayWindow.OpenedTechType.Ground))
                {
                    if (!EconomicManager.Instance.IsCanBuy(10)) 
                        return;
                    EconomicManager.Instance.BuySomething(10);
                    if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                    if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                    freeTileImage.sprite = _owner.owner.civilisationInfo.farmSprite;
                    freeTileImage.enabled = true;
                    freeTileImage.gameObject.SetActive(true);
                    _tileName = "Ground";
                    _owner.GetFood(2);
                }
                break;
            case 6:
                if (_techTypes.Contains(GameplayWindow.OpenedTechType.Ground) && isHasMountain)
                {
                    if (!EconomicManager.Instance.IsCanBuy(5)) 
                        return;
                    EconomicManager.Instance.BuySomething(5);
                    if (animalTileImage != null) Destroy(animalTileImage.gameObject);
                    if (fruitTileImage != null) Destroy(fruitTileImage.gameObject);
                    miningImage.sprite = _owner.owner.civilisationInfo.miningSprite;
                    _tileName = "Ground";
                    _owner.GetFood(2);
                }
                break;
        }
        
    }

    public void CreateWaterArea(int waterRad)
    {
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, waterRad);
        
        if (homeOnTile != null)
            return;
        
        tileType = TileType.Water;
        
        foreach (var tile in tiles)
        {
            if(tile.homeOnTile == null)
                tile.tileType = TileType.Water;
        }

        SetWaterTile();
        
        foreach (var tile in tiles)
        {
            if(tile.homeOnTile == null)
                tile.SetWaterTile();
        }
    }
    
    private void SetWaterTile()
    {
        isHasMountain = false;
        _tileName = "Water";
        treeTileImage.gameObject.SetActive(false);
        fruitTileImage.gameObject.SetActive(false);
        animalTileImage.gameObject.SetActive(false);
        mountainTileImage.gameObject.SetActive(false);

        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, 1);
        
        if(tiles.TrueForAll(tile => tile.tileType == TileType.Water))
            groundImage.sprite = LevelManager.Instance.waterSprites[1];
        else
            groundImage.sprite = LevelManager.Instance.waterSprites[0];
        var randomFish = UnityEngine.Random.Range(0, 6);
        switch (randomFish)
        {
            case 1:
                fishTileImage.gameObject.SetActive(true);
                fishTileImage.enabled = true;
                break;
        }
    }

    public bool BuyFruit()
    {
        if(fruitTileImage == null)
            return false;
        if (_owner != null && _owner.owner != null && !_owner.owner.technologies.Contains(TechInfo.Technology.Gather))
            return false;
        if(!fruitTileImage.enabled)
            return false;

        Destroy(fruitTileImage.gameObject);
        _owner.GetFood(1);
        return true;
    }
    
    public bool BuyAnimal()
    {
        if(animalTileImage == null)
            return false;
        if(_owner != null && _owner.owner != null && !_owner.owner.technologies.Contains(TechInfo.Technology.Hunt))
            return false;
        if(!animalTileImage.enabled)
            return false;

        Destroy(animalTileImage.gameObject);
        _owner.GetFood(1);
        return true;
    }
    
    public bool BuyFish()
    {
        if(fishTileImage == null)
            return false;
        if(_owner != null && _owner.owner != null && !_owner.owner.technologies.Contains(TechInfo.Technology.Fish))
            return false;
        if(!fishTileImage.enabled)
            return false;

        Destroy(fishTileImage.gameObject);
        _owner.GetFood(1);
        return true;
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
