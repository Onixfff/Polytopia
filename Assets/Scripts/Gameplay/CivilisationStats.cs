using System;
using UnityEngine;

public class CivilisationStats : MonoBehaviour
{
    [SerializeField] private CivilisationController civilisationController;
    
    private int _countKilledUnits;
    private int _countReceivedStars;
    private int _countExploreTile;
    private int _countConnectedCity;
    private int _countUnlockedTechnology;
    private int _turnWithoutAttack;
    private int _maxCityLevel;

    private void Awake()
    {
        if (civilisationController.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            LevelManager.Instance.OnUnlockTechnology += technology =>
            {
                AddUnlockedTech();
            };
        }
    }

    public void AddKill()
    {
        _countKilledUnits++;
        if (_countKilledUnits == 10)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.GateOfPower);
        }
    }

    public void AddStars(int count)
    {
        _countReceivedStars += count;
        if (_countReceivedStars >= 100)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.EmperorTomb);
        }
    }

    public void AddExploredTile()
    {
        _countExploreTile++;
        var count = LevelManager.Instance.gameBoardWindow.GetAllTile().Count;
        if (_countExploreTile == count)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.EyeOfGod);
        }
    }

    public void AddConnectedCity()
    {
        _countConnectedCity++;
        if (_countConnectedCity == 5)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.GrandBazaar);
        }
    }
    
    public void AddUnlockedTech()
    {
        _countUnlockedTechnology++;
        if (_countUnlockedTechnology >= 25)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.TowerOfWisdom);
        }
    }
    
    public void CheckMaxLevelCity(int level)
    {
        if (level > _maxCityLevel)
            _maxCityLevel = level;
        
        if (_maxCityLevel == 5)
        {
            civilisationController.GetMonumentBuilder().UnlockMonument(MonumentBuilder.MonumentType.ParkOfFortune);
        }
    }
}