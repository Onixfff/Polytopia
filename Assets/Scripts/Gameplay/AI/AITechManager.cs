﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class AITechManager : Singleton<AITechManager>
{
    public void TryBuyNeededTechnology(CivilisationController controller)
    {
        var allTech = new List<TechInfo.Technology>();
        var availableTech = new List<TechInfo.Technology>();
        var civTech = controller.technologies;

        allTech.AddRange(Enum.GetValues(typeof(TechInfo.Technology)).Cast<TechInfo.Technology>());

        foreach (var technology in allTech)
        {
            if (civTech.Contains(technology))
                continue;
            switch (technology)
            {
                case TechInfo.Technology.Rider:
                    availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Gather:
                    availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Mountain:
                    availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Fish:
                    availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Hunt:
                    availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Roads:
                    if (civTech.Contains(TechInfo.Technology.Rider))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.FreeSpirit:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Rider))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Farming:
                    if (civTech.Contains(TechInfo.Technology.Gather))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Strategy:
                    if (civTech.Contains(TechInfo.Technology.Gather))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Mining:
                    if (civTech.Contains(TechInfo.Technology.Mountain))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Meditation:
                    if(civTech.Count < 5)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Mountain))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Sailing:
                    if (civTech.Contains(TechInfo.Technology.Fish))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Whaling:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Fish))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Archery:
                    if (civTech.Contains(TechInfo.Technology.Hunt))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Forestry:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Hunt))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Trade:
                    if (civTech.Contains(TechInfo.Technology.Roads))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Chivalry:
                    if (civTech.Contains(TechInfo.Technology.FreeSpirit))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Construction:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Farming))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Diplomacy:
                    if (civTech.Contains(TechInfo.Technology.Strategy))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Forge:
                    if (civTech.Contains(TechInfo.Technology.Mining))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Philosophy:
                    if (civTech.Contains(TechInfo.Technology.Meditation))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Navigation:
                    if (civTech.Contains(TechInfo.Technology.Sailing))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Aqua:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Whaling))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Spiritualism:
                    if(civTech.Count < 8)
                        break;
                    if (civTech.Contains(TechInfo.Technology.Archery))
                        availableTech.Add(technology);
                    break;
                case TechInfo.Technology.Mathematics:
                    if (civTech.Contains(TechInfo.Technology.Forestry))
                        availableTech.Add(technology);
                    break;
            }
        }

        var minPrice = 100;
        var techs = new List<TechInfo.Technology>();
        foreach (var technology in availableTech)
        {
            var priseTech = CalculateTechPrice(technology, controller);
            if (priseTech <= minPrice &&
                priseTech <= controller.Money)
            {
                minPrice = priseTech;
                techs.Add(technology);
            }
        }

        if (techs.Count == 0)
            return;
        
        var rand = Random.Range(0, techs.Count);
        controller.AddMoney(-CalculateTechPrice(techs[rand], controller));
        controller.technologies.Add(techs[rand]);
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
