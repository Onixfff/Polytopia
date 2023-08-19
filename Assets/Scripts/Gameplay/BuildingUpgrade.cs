using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUpgrade : MonoBehaviour
{
    #region enum
    
    public enum BuildType
    {
        Church,
        ForestChurch,
        WaterChurch,
        MountainChurch,
        SawMill,
        WindMill,
        TradeHouse,
        Forge,
        
        Farm,
        Mine,
        Port,
        LumberHut
    }
    public BuildType currentType;

    #endregion
    
    [SerializeField] private List<Sprite> churchSprites;
    [SerializeField] private List<Sprite> forestChurchSprites;
    [SerializeField] private List<Sprite> waterChurchSprites;
    [SerializeField] private List<Sprite> mountainChurchSprites;
    [SerializeField] private List<Sprite> sawMillSprites;
    [SerializeField] private List<Sprite> windMillSprites;
    [SerializeField] private List<Sprite> tradeHouseSprites;
    [SerializeField] private List<Sprite> forgeSprites;
    
    private Tile _tile;
    private Image _image;
    
    private int _dateConstruction;
    private int _index = 0;
    private int _allAddedStars = 0;
    private int _allAddedPoints = 0;
    private int _allAddedPopulation = 0;
    
    public void Init(BuildType type)
    {
        var parent = transform.parent;
        _tile = parent.parent.GetComponent<Tile>();
        _image = parent.GetComponent<Image>();
        currentType = type;
        _dateConstruction = LevelManager.Instance.currentTurn;
        LevelManager.Instance.OnTurnBegin += TurnBegin;
        _allAddedStars = 0;
        _allAddedPoints = 0;
        _allAddedPopulation = 0;
        CheckProduces();
    }

    public void AddLevelToBuilding(int count)
    {
        UpdateManufactureVisual(count);
        CheckProduces();
    }

    public void CheckProduces()
    {
        switch (currentType)
        {
            case BuildType.Church:
                GivePopulationToOwner(-_allAddedPopulation + 1);
                GivePointToOwner(-_allAddedPoints + 100 + 50 * _index);
                break;
            case BuildType.ForestChurch:
                GivePopulationToOwner(-_allAddedPopulation + 1);
                GivePointToOwner(-_allAddedPoints + 100 + 50 * _index);
                break;
            case BuildType.WaterChurch:
                GivePopulationToOwner(-_allAddedPopulation + 1);
                GivePointToOwner(-_allAddedPoints + 100 + 50 * _index);
                break;
            case BuildType.MountainChurch:
                GivePopulationToOwner(-_allAddedPopulation + 1);
                GivePointToOwner(-_allAddedPoints + 100 + 50 * _index);
                break;
            case BuildType.SawMill:
                GivePopulationToOwner(-_allAddedPopulation + 1 + _index);
                break;
            case BuildType.WindMill:
                GivePopulationToOwner(-_allAddedPopulation + 1 + _index);
                break;
            case BuildType.TradeHouse:
                GiveStarsToOwner(2 * _index);
                break;
            case BuildType.Forge:
                GivePopulationToOwner(-_allAddedPopulation + 2 + 2 * _index);
                break;
            case BuildType.Farm:
                GivePopulationToOwner(2);
                break;
            case BuildType.Mine:
                GivePopulationToOwner(2);
                break;
            case BuildType.Port:
                GivePopulationToOwner(2);
                break;
            case BuildType.LumberHut:
                GivePopulationToOwner(1);
                break;
        }
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnTurnBegin -= TurnBegin;
        switch (currentType)
        {
            case BuildType.Church:
                
                break;
            
            case BuildType.ForestChurch:
                
                break;
            
            case BuildType.WaterChurch:
                
                break;
            
            case BuildType.MountainChurch:
                
                break;
            
            case BuildType.SawMill:
                //_tile.GetOwner().
                break;
            
            case BuildType.WindMill:
                
                break;
            
            case BuildType.TradeHouse:
                
                break;
            
            case BuildType.Forge:
                
                break;
        }
    }

    private void TurnBegin()
    {
        if (LevelManager.Instance.currentTurn - _dateConstruction == 5)
        {
            UpdateChurchVisual(_index);
            _index++;
            _dateConstruction = LevelManager.Instance.currentTurn;
        }

        switch (currentType)
        {
            case BuildType.TradeHouse:
                GiveStarsToOwner(2 * _index);
                break;
        }
    }

    private void UpdateChurchVisual(int index)
    {
        switch (currentType)
        {
            case BuildType.Church:
                if(index >= churchSprites.Count)
                    return;
                _image.sprite = churchSprites[index];
                break;
            
            case BuildType.ForestChurch:
                if(index >= forestChurchSprites.Count)
                    return;
                _image.sprite = forestChurchSprites[index];
                break;
            
            case BuildType.WaterChurch:
                if(index >= waterChurchSprites.Count)
                    return;
                _image.sprite = waterChurchSprites[index];
                break;
            
            case BuildType.MountainChurch:
                if(index >= mountainChurchSprites.Count)
                    return;
                _image.sprite = mountainChurchSprites[index];
                break;
        }
    }
    
    private void UpdateManufactureVisual(int level)
    {
        switch (currentType)
        {
            case BuildType.SawMill:
                if (_index >= sawMillSprites.Count)
                {
                    _index = sawMillSprites.Count - 1;
                    _image.sprite = sawMillSprites[level];
                    return;
                }
                _image.sprite = sawMillSprites[_index];
                break;
            
            case BuildType.WindMill:
                if (_index >= windMillSprites.Count)
                {
                    _index = windMillSprites.Count - 1;
                    _image.sprite = windMillSprites[level];
                    return;
                }
                _image.sprite = windMillSprites[_index];
                break;
            
            case BuildType.TradeHouse:
                if (_index >= tradeHouseSprites.Count)
                {
                    _index = tradeHouseSprites.Count - 1;
                    _image.sprite = tradeHouseSprites[level];
                    return;
                }
                _image.sprite = tradeHouseSprites[_index];
                break;
            
            case BuildType.Forge:
                if (_index >= forgeSprites.Count)
                {
                    _index = forgeSprites.Count - 1;
                    _image.sprite = forgeSprites[level];
                    return;
                }
                _image.sprite = forgeSprites[_index];
                break;
        }
        _index += level;
    }

    private void GiveStarsToOwner(int count)
    {
        _tile.GetOwner().owner.AddMoney(count);
        _allAddedStars += count;
    }
    
    private void GivePointToOwner(int count)
    {
        _tile.GetOwner().owner.AddPoint(count);
        _allAddedPoints += count;
    }
    
    private void GivePopulationToOwner(int count)
    {
        _tile.GetOwner().AddFood(count);
        _allAddedPopulation += count;
    }
}
