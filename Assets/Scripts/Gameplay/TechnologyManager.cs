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
    
    [SerializeField] private int priseFirstTier = 5;
    [SerializeField] private int priseTwoTier = 10;
    [SerializeField] private int priseThreeTier = 15;


    private void Awake()
    {
        #region Rider
        ridingButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseFirstTier))
            {
                RidingTech();    
            }
        });
        /*roadsButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                RoadsTech();    
            }
        });*/
        /*tradeButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                RoadsTech();    
            }
        });*/
        freeSpiritButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                FreeSpiritTech();    
            }
        });
        /*chivalryButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                RoadsTech();    
            }
        });*/
        #endregion
        #region Gathering
        gatheringButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseFirstTier))
            {
                GatheringTech();    
            }
        });
        farmingButton.onClick.AddListener(() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                FarmingTech();    
            }
        });
        strategyButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                StrategyTech();
            }
        }));

        #endregion
        #region Mountain
        mountainButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseFirstTier))
            {
                MountainTech();
            }
        }));
        miningButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                MiningTech();
            }
        }));
        #endregion
        #region Fish
        fishingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseFirstTier))
            {
                FishingTech();
            }
        }));
        #endregion
        #region Hunt
        huntingButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseFirstTier))
            {
                HuntingTech();
            }
        }));
        archeryButton.onClick.AddListener((() =>
        {
            if(TryBuyTech(priseTwoTier))
            {
                ArcheryTech();
            }
        }));
        #endregion
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
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(1);
        ridingButton.image.color = Color.green;
        Destroy(ridingButton);
    }
    /*private void RoadsTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Rider);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(1);
        ridingButton.image.color = Color.green;
        Destroy(ridingButton);
    }*/
    private void FreeSpiritTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.FreeSpirit);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(3);
        freeSpiritButton.image.color = Color.green;
        Destroy(freeSpiritButton);
    }
    #endregion
    #region Gathering
    private void GatheringTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Gather);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(0);
        gatheringButton.image.color = Color.green;
        Destroy(gatheringButton);
    }
    private void FarmingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Farming);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(0);
        farmingButton.image.color = Color.green;
        Destroy(farmingButton);
    }
    private void StrategyTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Strategy);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(2);
        strategyButton.image.color = Color.green;
        Destroy(strategyButton);
    }
    #endregion
    #region Mountain
    private void MountainTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mountain);
        mountainButton.image.color = Color.green;
        Destroy(mountainButton);
    }
    private void MiningTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mining);
        miningButton.image.color = Color.green;
        Destroy(miningButton);
    }
    #endregion
    #region Fish
    private void FishingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Fish);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(2);
        fishingButton.image.color = Color.green;
        Destroy(fishingButton);
    }
    #endregion
    #region Hunt
    private void HuntingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Hunt);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(1);
        huntingButton.image.color = Color.green;
        Destroy(huntingButton);
    }
    private void ArcheryTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Archery);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(3);
        archeryButton.image.color = Color.green;
        Destroy(archeryButton);
    }
    #endregion
}
