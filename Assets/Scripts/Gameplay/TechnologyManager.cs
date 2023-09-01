using System;
using System.Collections.Generic;
using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyManager : MonoBehaviour
{
    public Action OnTechBuy;
    
    [SerializeField] private Button ridingButton;
    [SerializeField] private Button roadsButton;
    [SerializeField] private Button tradeButton;
    [SerializeField] private Button freeSpiritButton;
    [SerializeField] private Button chivalryButton;
    
    [SerializeField] private Button gatheringButton;
    [SerializeField] private Button farmingButton;
    [SerializeField] private Button constructButton;
    [SerializeField] private Button strategyButton;
    [SerializeField] private Button diplomacyButton;
    
    [SerializeField] private Button mountainButton;
    [SerializeField] private Button miningButton;
    [SerializeField] private Button forgeButton;
    [SerializeField] private Button meditationButton;
    [SerializeField] private Button philosophyButton;
    
    [SerializeField] private Button fishingButton;
    [SerializeField] private Button sailingButton;
    [SerializeField] private Button navigationButton;
    [SerializeField] private Button whalingButton;
    [SerializeField] private Button aquaButton;
    
    [SerializeField] private Button huntingButton;
    [SerializeField] private Button forestryButton;
    [SerializeField] private Button mathematicsButton;
    [SerializeField] private Button archeryButton;
    [SerializeField] private Button spiritualismButton;
    
    [SerializeField] private int priceFirstTier = 1;
    [SerializeField] private int priceTwoTier = 2;
    [SerializeField] private int priceThreeTier = 3;
    
    [SerializeField] private GameObject techObject;

    public void Init()
    {
        #region Rider

        ridingButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceFirstTier)))
            {
                UnlockButtons(ridingButton, priceFirstTier, new List<Button>() { freeSpiritButton, roadsButton });
                RidingTech();
            }
        });
        roadsButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(roadsButton, priceTwoTier, new List<Button>() { tradeButton });
                RoadsTech();
            }
        });
        tradeButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(tradeButton, priceThreeTier);
                TradeTech();
            }
        });
        freeSpiritButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(freeSpiritButton, priceTwoTier, new List<Button>() { chivalryButton });
                FreeSpiritTech();
            }
        });
        chivalryButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(chivalryButton, priceThreeTier);
                ChivalryTech();
            }
        });

        #endregion

        #region Gathering

        gatheringButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceFirstTier)))
            {
                UnlockButtons(gatheringButton, priceFirstTier, new List<Button>() { farmingButton, strategyButton });
                GatheringTech();
            }
        });
        farmingButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(farmingButton, priceTwoTier, new List<Button>() { constructButton });
                FarmingTech();
            }
        });
        constructButton.onClick.AddListener(() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(constructButton, priceThreeTier);
                ConstructionTech();
            }
        });
        strategyButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(strategyButton, priceTwoTier, new List<Button>() { diplomacyButton });
                StrategyTech();
            }
        }));
        diplomacyButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(diplomacyButton, priceThreeTier);
                DiplomacyTech();
            }
        }));

        #endregion

        #region Mountain

        mountainButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceFirstTier)))
            {
                UnlockButtons(mountainButton, priceFirstTier, new List<Button>() { miningButton, meditationButton });
                MountainTech();
            }
        }));
        miningButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(miningButton, priceTwoTier, new List<Button>() { forgeButton });
                MiningTech();
            }
        }));
        forgeButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(forgeButton, priceThreeTier);
                ForgeTech();
            }
        }));
        meditationButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(meditationButton, priceTwoTier, new List<Button>() { philosophyButton });
                MeditationTech();
            }
        }));
        philosophyButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(philosophyButton, priceThreeTier);
                PhilosophyTech();
            }
        }));

        #endregion

        #region Fish

        fishingButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceFirstTier)))
            {
                UnlockButtons(fishingButton, priceFirstTier, new List<Button>() { sailingButton, whalingButton });
                FishingTech();
            }
        }));
        sailingButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(sailingButton, priceTwoTier, new List<Button>() { navigationButton });
                SailingTech();
            }
        }));
        navigationButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(navigationButton, priceThreeTier);
                NavigationTech();
            }
        }));
        whalingButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(whalingButton, priceTwoTier, new List<Button>() { aquaButton });
                WhalingTech();
            }
        }));
        aquaButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(aquaButton, priceThreeTier);
                AquatismTech();
            }
        }));

        #endregion

        #region Hunt

        huntingButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceFirstTier)))
            {
                UnlockButtons(huntingButton, priceFirstTier, new List<Button>() { archeryButton, forestryButton });
                HuntingTech();
            }
        }));
        archeryButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(archeryButton, priceTwoTier, new List<Button>() { spiritualismButton });
                ArcheryTech();
            }
        }));
        spiritualismButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(spiritualismButton, priceThreeTier);
                SpiritualismTech();
            }
        }));
        forestryButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceTwoTier)))
            {
                UnlockButtons(forestryButton, priceTwoTier, new List<Button>() { mathematicsButton });
                ForestryTech();
            }
        }));
        mathematicsButton.onClick.AddListener((() =>
        {
            if (!techObject.activeSelf || TryBuyTech(CalculatePrice(priceThreeTier)))
            {
                UnlockButtons(mathematicsButton, priceThreeTier);
                MathematicsTech();
            }
        }));

        #endregion
    }

    private void UnlockButtons(Button thisButton, int price, List<Button> buttons = null)
    {
        thisButton.GetComponent<TechIcon>().BuyTech();
        if (buttons == null)
        {
            OnTechBuy?.Invoke();
            return;
        }
        
        foreach (var button in buttons)
        {
            var techIcon = button.GetComponent<TechIcon>();
            techIcon.UnlockTech(CalculatePrice(price));
        }
        OnTechBuy?.Invoke();
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
