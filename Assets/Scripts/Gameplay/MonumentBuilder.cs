using System.Collections.Generic;
using UnityEngine;

public class MonumentBuilder : MonoBehaviour
{
    public enum MonumentType
    {
        AltarOfPeace,
        EmperorTomb,
        EyeOfGod,
        GateOfPower,
        GrandBazaar,
        ParkOfFortune,
        TowerOfWisdom
    }

    [SerializeField] private CivilisationController controller;
    private List<MonumentType> _availableMonuments;
    private List<MonumentType> _placedMonuments;

    public bool IsMonumentAvailable(MonumentType monumentType)
    {
        return _availableMonuments.Contains(monumentType);
    }

    public void UnlockMonument(MonumentType monumentType)
    {
        if(_placedMonuments.Contains(monumentType))
            return;
        
        _availableMonuments.Add(monumentType);
    }

    public void BlockMonument(MonumentType monumentType)
    {
        _availableMonuments.Remove(monumentType);
        _placedMonuments.Add(monumentType);
    }
}
