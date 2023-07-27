using System;
using System.Collections.Generic;
using DG.Tweening;
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
    public Vector2Int pos;
    public UnitController unitOnTile;
    public bool isSelected = false;
    public bool isHasMountain = false;
    public TileType tileType = TileType.Ground;
    public Home homeOnTile;

    [SerializeField] private Image groundImage;
    [SerializeField] private Image fruitTileImage;
    [SerializeField] private Image treeTileImage;
    [SerializeField] private Image animalTileImage;
    [SerializeField] private Image mountainTileImage;
    [SerializeField] private Image fishTileImage;
    [SerializeField] private Image blueTargetImage;
    [SerializeField] private Image redTargetImage;
    [SerializeField] private Image selectedOutlineImage;
    [SerializeField] private GameObject fog;
    [SerializeField] private Button getInfoButton;

    private string _tileName = "";
    private bool _isHomeOnTile = false;
    private Home _owner;

    public void ReplaceOwner(Home home)
    {
        if(_isHomeOnTile) return;

        _owner = home;
        homeOnTile = home;
        if(home.owner == null)
            return;
        var civilisationInfo = home.owner.civilisationInfo;
        if (tileType == TileType.Ground)
        {
            groundImage.sprite = civilisationInfo.groundSprite;
            animalTileImage.sprite = civilisationInfo.animalSprite;
            treeTileImage.sprite = civilisationInfo.treeSprite; 
            fruitTileImage.sprite = civilisationInfo.fruitSprite;
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

    public string GetTileName()
    {
        return _tileName;
    }

    public void SetTreeSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        treeTileImage.sprite = sprite;
        treeTileImage.enabled = true;
        
        if (_tileName != "")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetPumpkinSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        fruitTileImage.sprite = sprite;
        fruitTileImage.enabled = true;
        
        if (_tileName != "")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetAnimalSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        animalTileImage.sprite = sprite;
        animalTileImage.enabled = true;
        
        if (_tileName != "")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetMountainSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        mountainTileImage.sprite = sprite;
        mountainTileImage.enabled = true;
        mountainTileImage.gameObject.SetActive(false);
        isHasMountain = true;
        if (_tileName != "")
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
    
    public bool IsCanMoveTo()
    {
        return blueTargetImage.gameObject.activeSelf && !redTargetImage.gameObject.activeSelf;
    }
    
    public bool IsCanAttackTo()
    {
        return redTargetImage.gameObject.activeSelf;
    }
    
    public RectTransform GetRectTransform()
    {
        return fruitTileImage.GetComponent<RectTransform>();
    }

    public void BuildHome(Home home)
    {
        homeOnTile = home;
        Destroy(treeTileImage.gameObject); 
        Destroy(fruitTileImage.gameObject); 
        Destroy(animalTileImage.gameObject); 
        Destroy(mountainTileImage.gameObject); 
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

    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if(currO == null || currO.TryGetComponent(out Tile tile))
            HideTargetsTime();
        
        if(isSelected == false) return;
        if(pastO == gameObject)
            DeselectedTile();

        if (currO != null && currO == gameObject)
        {
            if (_owner != null)
            {
                if (fruitTileImage.gameObject.activeSelf)
                {
                    
                }
                else if(animalTileImage.gameObject.activeSelf)
                {
                    
                }
            }
        }
        
        if (_owner != null && currO == gameObject)
        {
            
            var types = new List<GameplayWindow.OpenedTechType>();
            if(fruitTileImage.enabled)
                types.Add(GameplayWindow.OpenedTechType.Fruit);
            
            if(treeTileImage.enabled)
                types.Add(GameplayWindow.OpenedTechType.Tree);
            
            if(animalTileImage.enabled)
                types.Add(GameplayWindow.OpenedTechType.Animal);
            
            LevelManager.Instance.gameplayWindow.ShowTileButton(types);
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
        if (!EconomicManager.Instance.IsCanBuy(2)) 
            return;
        if(gameObject != LevelManager.Instance.GetSelectedObject())
            return;
        Debug.Log(pos);
        EconomicManager.Instance.BuySomething(2);
        animalTileImage.gameObject.SetActive(false);
        fruitTileImage.gameObject.SetActive(false);
        _owner.GetFood(1);
    }

    public int waterRad = 1;

    [Button()]
    public void CreateWaterArea()
    {
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, waterRad);
        tileType = TileType.Water;
        foreach (var tile in tiles)
        {
            if(tile._owner != null && tile._owner.homeTile == this)
                continue;
            tile.tileType = TileType.Water;
        }
        SetWaterTile();
        foreach (var tile in tiles)
        {
            if(tile._owner != null && tile._owner.homeTile == this)
                continue;
            tile.SetWaterTile();
        }
    }
    private void SetWaterTile()
    {
        treeTileImage.gameObject.SetActive(false);
        fruitTileImage.gameObject.SetActive(false);
        animalTileImage.gameObject.SetActive(false);
        mountainTileImage.gameObject.SetActive(false);
        
        
        var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(this, 1);
        
        if(tiles.TrueForAll(tile => tile.tileType == TileType.Water))
            groundImage.sprite = LevelManager.Instance.waterSprites[1];
        else
            groundImage.sprite = LevelManager.Instance.waterSprites[0];
        var a = UnityEngine.Random.Range(0, 6);
        switch (a)
        {
            case 1:
                fishTileImage.gameObject.SetActive(true);
                break;
        }
    }
}
