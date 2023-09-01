using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;

public class AITechManager : Singleton<AITechManager>
{
    public void TryBuyNeededTechnology(CivilisationController controller)
    {
        var allTech = new List<TechInfo.Technology>();
        var civTech = controller.technologies;
        
        allTech.AddRange(Enum.GetValues(typeof(TechInfo.Technology)).Cast<TechInfo.Technology>());

        foreach (var technology in civTech)
        {
            if(allTech.Contains(technology))
                allTech.Remove(technology);
        }

        var maxPrice = 0;
        TechInfo.Technology? techForBuy = null;
        foreach (var technology in allTech)
        {
            var priseTech = CalculateTechPrice(technology, controller);
            if (priseTech > maxPrice &&
                priseTech <= controller.Money)
            {
                maxPrice = priseTech;
                techForBuy = technology;
            }
        }

        if (techForBuy == null)
            return;

        controller.AddMoney(-CalculateTechPrice(techForBuy, controller));
        controller.technologies.Add((TechInfo.Technology)techForBuy);
    }


    public int CalculateTechPrice(TechInfo.Technology? techType, CivilisationController controller)
    {
        var priceFirstTier = 1;
        var priceTwoTier = 2;
        var priceThreeTier = 3;

        var techPrice = 0;

        
        switch (techType)
        {
            case TechInfo.Technology.Rider:
                techPrice = priceFirstTier;
                break;
            case TechInfo.Technology.Gather:
                techPrice = priceFirstTier;
                break;
            case TechInfo.Technology.Mountain:
                techPrice = priceFirstTier;
                break;
            case TechInfo.Technology.Fish:
                techPrice = priceFirstTier;
                break;
            case TechInfo.Technology.Hunt:
                techPrice = priceFirstTier;
                break;
            case TechInfo.Technology.Roads:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.FreeSpirit:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Farming:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Strategy:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Mining:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Meditation:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Sailing:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Whaling:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Archery:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Forestry:
                techPrice = priceTwoTier;
                break;
            case TechInfo.Technology.Trade:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Chivalry:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Construction:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Diplomacy:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Forge:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Philosophy:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Navigation:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Aqua:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Spiritualism:
                techPrice = priceThreeTier;
                break;
            case TechInfo.Technology.Mathematics:
                techPrice = priceThreeTier;
                break;
        }

        techPrice = techPrice + controller.homes.Count - 1;
        if (controller.technologies.Contains(TechInfo.Technology.Philosophy))
            techPrice = Mathf.RoundToInt((techPrice / 3) + 1);

        return techPrice;
    }
}
