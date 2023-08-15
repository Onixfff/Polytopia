using System.Collections.Generic;
using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyManager : MonoBehaviour
{
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
    
    [SerializeField] private int priceFirstTier = 5;
    [SerializeField] private int priceTwoTier = 10;
    [SerializeField] private int priceThreeTier = 15;

    private void Start()
    {
        #region Rider
        ridingButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceFirstTier))
            {
                UnlockButtons(ridingButton, new List<Button>(){freeSpiritButton, roadsButton});
                RidingTech();
            }
        });
        roadsButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(roadsButton, new List<Button>(){tradeButton});
                RoadsTech();
            }
        });
        tradeButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(tradeButton);
                TradeTech();
            }
        });
        freeSpiritButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(freeSpiritButton, new List<Button>(){chivalryButton});
                FreeSpiritTech();
            }
        });
        chivalryButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(chivalryButton);
                ChivalryTech();    
            }
        });
        #endregion
        #region Gathering
        gatheringButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceFirstTier))
            {
                UnlockButtons(gatheringButton, new List<Button>(){farmingButton, strategyButton});
                GatheringTech();    
            }
        });
        farmingButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(farmingButton, new List<Button>(){constructButton});
                FarmingTech();    
            }
        });
        constructButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(constructButton);
                ConstructionTech();    
            }
        });
        strategyButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(strategyButton, new List<Button>(){diplomacyButton});
                StrategyTech();
            }
        }));
        diplomacyButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(diplomacyButton);
                DiplomacyTech();
            }
        }));

        #endregion
        #region Mountain
        mountainButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceFirstTier))
            {
                UnlockButtons(mountainButton, new List<Button>(){miningButton, meditationButton});
                MountainTech();
            }
        }));
        miningButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(miningButton, new List<Button>(){forgeButton});
                MiningTech();
            }
        }));
        forgeButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(forgeButton);
                ForgeTech();
            }
        }));
        meditationButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(meditationButton, new List<Button>(){philosophyButton});
                MeditationTech();
            }
        }));
        philosophyButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(philosophyButton);
                PhilosophyTech();
            }
        }));
        #endregion
        #region Fish
        fishingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceFirstTier))
            {
                UnlockButtons(fishingButton, new List<Button>(){sailingButton, whalingButton});
                FishingTech();
            }
        }));
        sailingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(sailingButton, new List<Button>(){navigationButton});
                SailingTech();
            }
        }));
        navigationButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(navigationButton);
                NavigationTech();
            }
        }));
        whalingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(whalingButton, new List<Button>(){aquaButton});
                WhalingTech();
            }
        }));
        aquaButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(aquaButton);
                AquatismTech();
            }
        }));
        #endregion
        #region Hunt
        huntingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceFirstTier))
            {
                UnlockButtons(huntingButton, new List<Button>(){archeryButton, forestryButton});
                HuntingTech();
            }
        }));
        archeryButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(archeryButton, new List<Button>(){spiritualismButton});
                ArcheryTech();
            }
        }));
        spiritualismButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(spiritualismButton);
                SpiritualismTech();
            }
        }));
        forestryButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceTwoTier))
            {
                UnlockButtons(forestryButton, new List<Button>(){mathematicsButton});
                ForestryTech();
            }
        }));
        mathematicsButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priceThreeTier))
            {
                UnlockButtons(mathematicsButton);
                MathematicsTech();
            }
        }));
        #endregion
    }

    private void UnlockButtons(Button thisButton, List<Button> buttons = null)
    {
        thisButton.GetComponent<TechIcon>().BuyTech();
        if (buttons == null)
            return;
        
        foreach (var button in buttons)
        {
            var techIcon = button.GetComponent<TechIcon>();
            techIcon.UnlockTech();
        }
    }

    private bool TryBuyTech(int price)
    {
        if (!EconomicManager.Instance.IsCanBuy(price)) 
            return false;
        EconomicManager.Instance.BuySomething(price);
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
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Aquatism);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(17);
    }
    #endregion
    #region Hunt
    private void HuntingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Hunt);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(1);
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
