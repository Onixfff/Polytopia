using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyManager : MonoBehaviour
{
    [SerializeField] private Button horsebackRidingButton;
    [SerializeField] private Button huntingButton;
    [SerializeField] private Button fishingButton;
    [SerializeField] private Button gatheringButton;
    [SerializeField] private Button mountaineeringButton;
    [SerializeField] private int priseFirstTier = 5;
    [SerializeField] private int priseTwoTier = 10;
    [SerializeField] private int priseThreeTier = 15;
    
    private void Awake()
    {
        horsebackRidingButton.onClick.AddListener(() =>
        {
            BuyTechnology(1);
        });
        gatheringButton.onClick.AddListener(() =>
        {
            BuyTechnology(4);
        });
        huntingButton.onClick.AddListener(() =>
        {
            BuyTechnology(2);
        });
        fishingButton.onClick.AddListener(() =>
        {
            BuyTechnology(3);
        });
        mountaineeringButton.onClick.AddListener(() =>
        {
            BuyTechnology(5);
        });
    }

    private void BuyTechnology(int techIndex)
    {
        switch (techIndex)
        {
            case 1:
                if (!EconomicManager.Instance.IsCanBuy(priseFirstTier)) 
                    return;
                EconomicManager.Instance.BuySomething(priseFirstTier);
                RidersTech();
                break;
            case 2:
                if (!EconomicManager.Instance.IsCanBuy(priseFirstTier)) 
                    return;
                EconomicManager.Instance.BuySomething(priseFirstTier);
                HuntingTech();
                break;
            case 3:
                if (!EconomicManager.Instance.IsCanBuy(priseFirstTier)) 
                    return;
                EconomicManager.Instance.BuySomething(priseFirstTier);
                FishingTech();
                break;
            case 4:
                if (!EconomicManager.Instance.IsCanBuy(priseFirstTier)) 
                    return;
                EconomicManager.Instance.BuySomething(priseFirstTier);
                GatheringTech();
                break;
            case 5:
                if (!EconomicManager.Instance.IsCanBuy(priseFirstTier)) 
                    return;
                EconomicManager.Instance.BuySomething(priseFirstTier);
                MountainTech();
                break;
        }
    }
    
    private void HuntingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Hunt);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(2);
    }
    
    private void RidersTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Rider);
        LevelManager.Instance.gameplayWindow.UnlockUnitTech(2);
    }
    
    private void GatheringTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Gather);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(1);
    }
    
    private void MountainTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Mountain);
        //LevelManager.Instance.gameplayWindow.UnlockUnitTech(4);
    }
    
    private void FishingTech()
    {
        LevelManager.Instance.OnUnlockTechnology?.Invoke(TechInfo.Technology.Fish);
        LevelManager.Instance.gameplayWindow.UnlockTileTech(3);
    }
}
