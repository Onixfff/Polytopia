using System.Collections.Generic;
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
        homeLevelUp[level].SetActive(true);

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
        }));
    }
}
