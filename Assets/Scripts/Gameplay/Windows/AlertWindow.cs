using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : BaseWindow
{
    [SerializeField] private List<GameObject> homeLevelUp;
    
    private void Awake()
    {
        LevelManager.Instance.OnHomeLevelUp += HomeLevelUp;
    }

    public void HomeLevelUp(Home home, int level)
    {
        if(level < homeLevelUp.Count)
            homeLevelUp[level].SetActive(true);
        else
        {
            homeLevelUp.Last().SetActive(true);
            level = homeLevelUp.Count - 1;
        }

        homeLevelUp[level].transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        homeLevelUp[level].transform.GetChild(0).GetComponent<Button>().onClick.AddListener((() =>
        {
            if (level == 1)
            {
                home.BuildForge();
            }
            if (level == 2)
            {
                home.CreateHomeWall();
            }
            if (level == 3)
            {
                home.AddFood(3);
            }
            if (level >= 4)
            {
                home.BuildPark();
            }
            HideWindow();
        }));
        homeLevelUp[level].transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        homeLevelUp[level].transform.GetChild(1).GetComponent<Button>().onClick.AddListener((() =>
        {
            if (level == 1)
            {
                home.CreateExplorer();
            }
            if (level == 2)
            {
                home.AddStars();
            }
            if (level == 3)
            {
                home.IncreaseBoarder();
            }
            if (level >= 4)
            {
                home.CreateSuperUnit();
            }
            HideWindow();
        }));
    }
}
