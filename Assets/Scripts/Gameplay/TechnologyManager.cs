using System;
using System.Collections.Generic;
using Gameplay.SO;
using JetBrains.Annotations;
using UnityEngine;

public class TechnologyManager : MonoBehaviour
{
    public Action OnTechBuy;
        
    public void OpenTech(TechIcon techIcon, int price)
    {
        if (gameObject.activeSelf && !TryBuyTech(CalculatePrice(price)))
        {
            return;
        }
        OnTechBuy?.Invoke();
        
        switch (techIcon.type)
        {
            case TechInfo.Technology.Rider:
                    RidingTech();
                    break;
            case TechInfo.Technology.Gather:
                GatheringTech();
                break;
            case TechInfo.Technology.Mountain:
                MountainTech();
                break;
            case TechInfo.Technology.Fish:
                FishingTech();
                break;
            case TechInfo.Technology.Hunt:
                HuntingTech();
                break;
            case TechInfo.Technology.Roads:
                RoadsTech();
                break;
            case TechInfo.Technology.FreeSpirit:
                FreeSpiritTech();
                break;
            case TechInfo.Technology.Farming:
                FarmingTech();
                break;
            case TechInfo.Technology.Strategy:
                StrategyTech();
                break;
            case TechInfo.Technology.Mining:
                MiningTech();
                break;
            case TechInfo.Technology.Meditation:
                MeditationTech();
                break;
            case TechInfo.Technology.Sailing:
                SailingTech();
                break;
            case TechInfo.Technology.Whaling:
                WhalingTech();
                break;
            case TechInfo.Technology.Archery:
                ArcheryTech();
                break;
            case TechInfo.Technology.Forestry:
                ForestryTech();
                break;
            case TechInfo.Technology.Trade:
                TradeTech();
                break;
            case TechInfo.Technology.Chivalry:
                ChivalryTech();
                break;
            case TechInfo.Technology.Construction:
                ConstructionTech();
                break;
            case TechInfo.Technology.Diplomacy:
                DiplomacyTech();
                break;
            case TechInfo.Technology.Forge:
                ForgeTech();
                break;
            case TechInfo.Technology.Philosophy:
                PhilosophyTech();
                break;
            case TechInfo.Technology.Navigation:
                NavigationTech();
                break;
            case TechInfo.Technology.Aqua:
                AquatismTech();
                break;
            case TechInfo.Technology.Spiritualism:
                SpiritualismTech();
                break;
            case TechInfo.Technology.Mathematics:
                MathematicsTech();
                break;
        }
    }
    
    private int CalculatePrice(int price)
    {
        var techPrice = price;
        
        techPrice += LevelManager.Instance.gameBoardWindow.playerCiv.homes.Count - 1;
        if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(TechInfo.Technology.Philosophy))
        {
            techPrice = Mathf.RoundToInt((techPrice / 3) + 1);
        }

        return techPrice;
    }
    
    private bool TryBuyTech(int price)
    {
        if (!LevelManager.Instance.gameBoardWindow.playerCiv.IsCanBuy(price)) 
            return false;
        LevelManager.Instance.gameBoardWindow.playerCiv.BuySomething(price);
        return true;
    }

    #region Riding
    private void RidingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Rider);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(3);
    }
    private void RoadsTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Roads);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(3);
    }
    private void TradeTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Trade);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(12);
    }
    private void FreeSpiritTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.FreeSpirit);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(4);
    }
    private void ChivalryTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Chivalry);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(13);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(8);
    }
    
    #endregion
    #region Gathering
    private void GatheringTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Gather);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(0);
        LevelManager.Instance.gameBoardWindow.ShowAllCrop();
    }
    private void FarmingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Farming);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(5);
    }
    private void ConstructionTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Construction);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(14);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(15);
    }
    private void StrategyTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Strategy);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(2);
    }
    private void DiplomacyTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Diplomacy);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(4);
        foreach (var civ in LevelManager.Instance.GetCivilisationControllers())
        {
            civ.capitalHome.homeTile.UnlockTile(LevelManager.Instance.gameBoardWindow.playerCiv);
        }
    }

    #endregion
    #region Mountain
    private void MountainTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mountain);
        LevelManager.Instance.gameBoardWindow.ShowAllOre();
    }
    private void MiningTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mining);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(6);
    }
    private void ForgeTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Forge);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(6);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(16);
    }
    private void MeditationTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Meditation);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(7);
    }
    private void PhilosophyTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Philosophy);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(5);
    }
    #endregion
    #region Fish
    private void FishingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Fish);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(1);
    }
    private void SailingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Sailing);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(8);
    }
    private void NavigationTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Navigation);
        //LevelManager.Instance.gameplayWindow.UnlockTileTech(4);
    }
    private void WhalingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Whaling);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(9);
    }
    private void AquatismTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Aqua);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(17);
    }
    #endregion
    #region Hunt
    private void HuntingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Hunt);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(2);
    }
    private void ArcheryTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Archery);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(1);
    }
    private void SpiritualismTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Spiritualism);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(18);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(19);
    }
    private void ForestryTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Forestry);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(10);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(11);

    }
    private void MathematicsTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mathematics);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(7);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(20);
    }
    #endregion
}
