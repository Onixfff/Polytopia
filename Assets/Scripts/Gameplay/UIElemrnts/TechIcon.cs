using System;
using DG.Tweening;
using Gameplay.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechIcon : MonoBehaviour
{
    [SerializeField] private Button techButton;
    [SerializeField] private TextMeshProUGUI priceText; 
    [SerializeField] private GameObject lockObject; 
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject buyObject;
    [SerializeField] private Color unlockColor;
    [SerializeField] private Color buyColor;
    [SerializeField] private Color noMoneyColor;

    [SerializeField] private int originalPrice = 5;
    
    public TechInfo.Technology type;
    public bool isTechUnlock = false;
    public bool isInit = false;

    public void OpenTech()
    {
        techButton.onClick?.Invoke();
    }

    private void Start()
    {
        if (!isInit)
        {
            isInit = true;
            var player = LevelManager.Instance.gameBoardWindow.playerCiv;
            if (isTechUnlock == false)
            {
                if (lockObject != null) lockObject.SetActive(true);
                techButton.enabled = false;
            }
            else
            {
                isTechUnlock = true;
            }
            lineObject.SetActive(true);
            lineObject.transform.SetParent(transform.parent.parent.GetChild(0).transform);
        
            if (player.civilisationInfo.technology.startTechnologies == type)
            {
                techButton.enabled = true;
                OpenTech();
            }
            transform.parent.parent.parent.parent.parent.parent.GetComponent<TechnologyManager>().OnTechBuy += UpdateVisual;
        }
    }

    private void OnEnable()
    {
        
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(!techButton.enabled)
            return;
        
        var techPrice = originalPrice;
        
        techPrice = techPrice + LevelManager.Instance.gameBoardWindow.playerCiv.homes.Count - 1;
        if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(TechInfo.Technology.Philosophy))
        {
            techPrice = Mathf.RoundToInt((techPrice / 3) + 1);
        }
        priceText.text = $"{techPrice}";
        
        if (LevelManager.Instance.gameBoardWindow.playerCiv.Money < techPrice)
        {
            priceText.color = noMoneyColor;
            lineObject.GetComponent<Image>().color = noMoneyColor;
            GetComponent<Image>().color = noMoneyColor;
        }
        else
        {
            priceText.color = unlockColor;
            lineObject.GetComponent<Image>().color = unlockColor;
            GetComponent<Image>().color = unlockColor;
        }
    }

    public void UnlockTech()
    {
        isTechUnlock = true;
        techButton.enabled = true;
        lineObject.GetComponent<Image>().color = unlockColor;
        Destroy(lockObject);
    }

    public void BuyTech()
    {
        priceText.transform.parent.gameObject.SetActive(false);
        isTechUnlock = false;
        if(lockObject != null) Destroy(lockObject);
        lineObject.GetComponent<Image>().color = buyColor;
        buyObject.SetActive(true);
        techButton.enabled = false;
    }
}
