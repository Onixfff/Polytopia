using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;

public class TechIcon : MonoBehaviour
{
    [SerializeField] private TechInfo.Technology type;
    [SerializeField] private GameObject lockObject; 
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject buyObject;
    [SerializeField] private Color unlockColor;
    [SerializeField] private Color buyColor;

    private bool _isPastTechUnlock;
    void Start()
    {
        if (lockObject != null) lockObject.SetActive(true);
        lineObject.SetActive(true);
        lineObject.transform.SetParent(transform.parent.parent.GetChild(0).transform);
        var inVal = 0;
        DOTween.To(() => inVal, x => x = inVal, 1, 1).OnComplete(() =>
        {
            if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(type))
            {
                UnlockTech();
                BuyTech();
            }
        });
    }

    public void UnlockTech()
    {
        lineObject.GetComponent<Image>().color = unlockColor;
        if (lockObject != null) lockObject.SetActive(false);
    }

    public void BuyTech()
    {
        lineObject.GetComponent<Image>().color = buyColor;
        buyObject.SetActive(true);
    }
}
