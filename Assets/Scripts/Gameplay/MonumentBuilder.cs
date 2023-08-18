using System.Collections.Generic;
using NaughtyAttributes;
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
    public MonumentType _monumentType;

    private void Awake()
    {
        _placedMonuments ??= new List<MonumentType>();
        _availableMonuments ??= new List<MonumentType>();

    }

    [Button()]
    private void UnLock()
    {
        UnlockMonument(_monumentType);
    }
    
    public bool IsMonumentAvailable(MonumentType monumentType)
    {
        return _availableMonuments.Contains(monumentType);
    }

    public void UnlockMonument(MonumentType monumentType)
    {
        if(_placedMonuments.Contains(monumentType))
            return;
        var gameplayWindow = LevelManager.Instance.gameplayWindow;
        _availableMonuments.Add(monumentType);
        switch (monumentType)
        {
            case MonumentType.AltarOfPeace:
                gameplayWindow.UnlockTileTech(21);
                break;
            case MonumentType.EmperorTomb:
                gameplayWindow.UnlockTileTech(22);
                break;
            case MonumentType.EyeOfGod:
                gameplayWindow.UnlockTileTech(23);
                break;
            case MonumentType.GateOfPower:
                gameplayWindow.UnlockTileTech(24);
                break;
            case MonumentType.GrandBazaar:
                gameplayWindow.UnlockTileTech(25);
                break;
            case MonumentType.ParkOfFortune:
                gameplayWindow.UnlockTileTech(26);
                break;
            case MonumentType.TowerOfWisdom:
                gameplayWindow.UnlockTileTech(27);
                break;
        }
    }

    public void BlockMonument(MonumentType monumentType)
    {
        _availableMonuments.Remove(monumentType);
        _placedMonuments.Add(monumentType);
    }
}
