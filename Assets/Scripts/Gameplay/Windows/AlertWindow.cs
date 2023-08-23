using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : MonoBehaviour
{
    [SerializeField] private List<GameObject> homeLevelUp;
    
    public void HomeLevelUp(Home home, int level, int leftovers)
    {
        level--;
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
            if (level == 0)
            {
                home.BuildForge();
            }
            if (level == 1)
            {
                home.CreateHomeWall();
            }
            if (level == 2)
            {
                home.AddFood(3);
            }
            if (level >= 3)
            {
                home.BuildPark();
            }
            home.AddFood(leftovers);
            homeLevelUp[level].SetActive(false);
        }));
        homeLevelUp[level].transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        homeLevelUp[level].transform.GetChild(1).GetComponent<Button>().onClick.AddListener((() =>
        {
            if (level == 0)
            {
                home.CreateScout(home.homeTile);
            }
            if (level == 1)
            {
                home.owner.AddMoney(5);
            }
            if (level == 2)
            {
                home.IncreaseBoarder();
            }
            if (level >= 3)
            {
                home.CreateSuperUnit();
            }
            home.AddFood(leftovers);
            homeLevelUp[level].SetActive(false);
        }));
    }
}
