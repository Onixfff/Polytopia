using System.Collections.Generic;
using Gameplay.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechIcon : MonoBehaviour
{
    public TechInfo.Technology type;
    public bool isTechUnlock = false;
    public bool isTechBuy = false;
    public bool isInit = false;
    
    [SerializeField] private Button techButton;
    [SerializeField] private List<TechIcon> techIcons; 
    [SerializeField] private TextMeshProUGUI priceText; 
    [SerializeField] private GameObject lockObject; 
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject buyObject;
    [SerializeField] private Color unlockColor;
    [SerializeField] private Color buyColor;
    [SerializeField] private Color noMoneyColor;
    [SerializeField] private int originalPrice = 5;

    [SerializeField] private TechnologyManager technologyManager;
    [SerializeField] private TechInfoWindow techInfoWindow;

    private int _techPrice;
    
    public void OpenTech()
    {
        techInfoWindow.gameObject.SetActive(false);
        technologyManager.OpenTech(this, _techPrice);
        UpdateVisual();
        BuyTech();
        foreach (var techIcon in techIcons)
        {
            techIcon.UnlockTech();
        }
    }
    
    private void Start()
    {
        techIcons ??= new List<TechIcon>();
        if (!isInit)
        {
            isInit = true;
            var player = LevelManager.Instance.gameBoardWindow.playerCiv;
            if (isTechUnlock == false)
            {
                if (lockObject != null) lockObject.SetActive(true);
            }
            else
            {
                isTechUnlock = true;
            }
            lineObject.SetActive(true);
            lineObject.transform.SetParent(transform.parent.parent.GetChild(0).transform);
        
            if (player.civilisationInfo.technology.startTechnologies == type)
            {
                OpenTech();
            }
        }
        technologyManager.OnTechBuy += UpdateVisual;
        techButton.onClick.AddListener(OpenTechInfoWindow);
    }
    
    private void UnlockButtons(TechIcon thisTechIcon, int price)
    {
        BuyTech();
        if (techIcons == null || techIcons.Count == 0)
            return;
        
        
        foreach (var techIcon in techIcons)
        {
            techIcon.UnlockTech();
        }
    }
    
    private void OpenTechInfoWindow()
    {
        techInfoWindow.gameObject.SetActive(true);
        
        techInfoWindow.noMoneyObject.SetActive(false);
        techInfoWindow.closeObject.SetActive(false);
        techInfoWindow.completeObject.SetActive(false);
        techInfoWindow.backBuyObject.SetActive(false);
        techInfoWindow.backObject.SetActive(false);
        
        //if (techInfoWindow.techInfoNameText.text.Contains("(закрыто)"))
            techInfoWindow.techInfoNameText.text.Replace("(закрыто)", "");
        
        //if (techInfoWindow.techInfoNameText.text.Contains("(завершено)"))
            techInfoWindow.techInfoNameText.text.Replace("(завершено)", "");
        
        foreach (var techInfoBackButton in techInfoWindow.techInfoBackButtons)
        {
            techInfoBackButton.gameObject.SetActive(false);
        }
        if (!isTechUnlock)
        {
            techInfoWindow.backObject.SetActive(true);
            techInfoWindow.closeObject.SetActive(true);
            techInfoWindow.techInfoBackButtons[1].gameObject.SetActive(true);
            //techInfoWindow.techInfoNameText.text = techInfoWindow.techInfoNameText.text + "(закрыто)";
        }
        else
        if (isTechUnlock && !isTechBuy)
        {
            _techPrice = originalPrice;
            _techPrice = _techPrice + LevelManager.Instance.gameBoardWindow.playerCiv.homes.Count - 1;
            if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(TechInfo.Technology.Philosophy))
            {
                _techPrice = Mathf.RoundToInt((_techPrice / 3) + 1);
            }
            
            if (LevelManager.Instance.gameBoardWindow.playerCiv.Money < _techPrice)
            {
                techInfoWindow.backBuyObject.SetActive(true);
                techInfoWindow.techInfoBackButtons[0].gameObject.SetActive(true);
                techInfoWindow.noMoneyObject.SetActive(true);
                techInfoWindow.openTechButton.image.color = noMoneyColor;
                techInfoWindow.openTechButton.enabled = false;
            }
            else
            {
                techInfoWindow.backBuyObject.SetActive(true);
                techInfoWindow.techInfoBackButtons[0].gameObject.SetActive(true);
                techInfoWindow.openTechButton.image.color = techInfoWindow.openButtonColor;
                techInfoWindow.openTechButton.enabled = true;
            }
        }
        else        
        if (isTechBuy)
        {
            techInfoWindow.backObject.SetActive(true);
            techInfoWindow.completeObject.SetActive(true);
            techInfoWindow.techInfoBackButtons[1].gameObject.SetActive(true);
            //techInfoWindow.techInfoNameText.text = techInfoWindow.techInfoNameText.text + "(завершено)";
        }
        
        techInfoWindow.openTechButton.onClick.RemoveAllListeners();
        techInfoWindow.openTechButton.onClick.AddListener(OpenTech);
    }

    private void OnEnable()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(isTechBuy)
            return;
        var techPrice = originalPrice;
        
        techPrice = techPrice + LevelManager.Instance.gameBoardWindow.playerCiv.homes.Count - 1;
        if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(TechInfo.Technology.Philosophy))
        {
            techPrice = Mathf.RoundToInt((techPrice / 3) + 1);
        }

        techInfoWindow.price.text = $"{techPrice}";
        priceText.text = $"{techPrice}";
        if (LevelManager.Instance.gameBoardWindow.playerCiv.Money < techPrice)
        {
            techInfoWindow.price.color = noMoneyColor;
            priceText.color = noMoneyColor;
        }
        else
        {
            techInfoWindow.price.color = unlockColor;
            priceText.color = unlockColor;
        }
    
        if (LevelManager.Instance.gameBoardWindow.playerCiv.Money < techPrice)
        {
            lineObject.GetComponent<Image>().color = noMoneyColor;
            GetComponent<Image>().color = noMoneyColor;
        }
        else
        {
            lineObject.GetComponent<Image>().color = unlockColor;
            GetComponent<Image>().color = unlockColor;
        }
    }

    private void UnlockTech()
    {
        isTechUnlock = true;
        lineObject.GetComponent<Image>().color = unlockColor;
        Destroy(lockObject);
    }

    private void BuyTech()
    {
        isTechBuy = true;
        if(lockObject != null) Destroy(lockObject);
        lineObject.GetComponent<Image>().color = buyColor;
        buyObject.SetActive(true);
    }
}
