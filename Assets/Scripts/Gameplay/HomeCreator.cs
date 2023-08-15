using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HomeCreator : MonoBehaviour
{
    [SerializeField] private List<RectTransform> homePositions;
    [SerializeField] private GameObject capitalHomePrefab;
    [SerializeField] private GameObject forgeHomePrefab;
    [SerializeField] private GameObject parkHomePrefab;
    [SerializeField] private GameObject home1Prefab;
    [SerializeField] private GameObject home2Prefab;

    private Image _capitalBlock;
    private List<Image> _home1Blocks;
    private List<Image> _home2Blocks;
    private void Awake()
    {
        _home1Blocks ??= new List<Image>();
        _home2Blocks ??= new List<Image>();
    }

    public void UpdateVisual(HomeInfo homeInfo)
    {
        capitalHomePrefab.GetComponent<Image>().sprite = homeInfo.CapitalHomeSprite;
        forgeHomePrefab.GetComponent<Image>().sprite = homeInfo.ForgeHomeSprite;
        parkHomePrefab.GetComponent<Image>().sprite = homeInfo.ParkHomeSprite;
        home1Prefab.GetComponent<Image>().sprite = homeInfo.Home1Sprite;
        home2Prefab.GetComponent<Image>().sprite = homeInfo.Home2Sprite;

        if (_capitalBlock != null) _capitalBlock.sprite = homeInfo.CapitalHomeSprite;
        foreach (var image in _home1Blocks)
        {
            image.sprite = homeInfo.Home1Sprite;
        }
        foreach (var image in _home2Blocks)
        {
            image.sprite = homeInfo.Home2Sprite;
        }
    }
    
    public void CreateCapital()
    {
        var capital = Instantiate(capitalHomePrefab, homePositions.Last()).GetComponent<RectTransform>();
        capital.anchoredPosition = Vector2.zero;
        _capitalBlock = capital.GetComponent<Image>();
    }
    
    public void CreateForge()
    {
        var randParent = Random.Range(0, homePositions.Count);
        var parent = homePositions[randParent].GetComponent<RectTransform>();
        var forge = Instantiate(forgeHomePrefab, parent).GetComponent<RectTransform>();
        forge.anchoredPosition = Vector2.zero;
        if (parent.childCount > 1)
        {
            var rectTransform = parent.GetChild(0).GetComponent<RectTransform>(); 
            rectTransform.SetParent(forge);
            rectTransform.anchoredPosition = new Vector2(0, 13);
        }
    }
    
    public void CreatePark()
    {
        var randParent = Random.Range(0, homePositions.Count);
        var parent = homePositions[randParent].GetComponent<RectTransform>();
        var park = Instantiate(parkHomePrefab, parent).GetComponent<RectTransform>();
        park.anchoredPosition = Vector2.zero;
        if (parent.childCount > 1)
        {
            var rectTransform = parent.GetChild(0).GetComponent<RectTransform>(); 
            rectTransform.SetParent(park);
            rectTransform.anchoredPosition = new Vector2(0, 13);
        }
    }
    
    public void CreateHome()
    {
        for (var i = 0; i < 3; i++)
        {
            var randPrefab = Random.Range(0, 2);
            var randParent = Random.Range(0, homePositions.Count);
            var parent = homePositions[randParent].GetComponent<RectTransform>();
            RectTransform home = null;
            switch (randPrefab)
            {
                case 0:
                    home = Instantiate(home1Prefab, parent).GetComponent<RectTransform>();
                    _home1Blocks.Add(home.GetComponent<Image>());
                    home.anchoredPosition = Vector2.zero;
                    break;
                case 1:
                    home = Instantiate(home2Prefab, parent).GetComponent<RectTransform>();
                    _home2Blocks.Add(home.GetComponent<Image>());
                    home.anchoredPosition = Vector2.zero;
                    break;
            }
            if (parent.childCount > 1)
            {
                var rectTransform = parent.GetChild(0).GetComponent<RectTransform>(); 
                rectTransform.SetParent(home);
                rectTransform.anchoredPosition = new Vector2(0, 13);
            }
        }
    }

    public void LevelUpHome()
    {
        for (var i = 0; i < 4; i++)
        {
            var randPrefab = Random.Range(0, 2);
            var randParent = Random.Range(0, homePositions.Count);
            var parent = homePositions[randParent];
            RectTransform home = null;
            switch (randPrefab)
            {
                case 0:
                    home = Instantiate(home1Prefab, parent).GetComponent<RectTransform>();
                    _home1Blocks.Add(home.GetComponent<Image>());
                    break;
                case 1:
                    home = Instantiate(home2Prefab, parent).GetComponent<RectTransform>();
                    _home2Blocks.Add(home.GetComponent<Image>());
                    break;
            }
            
            if (parent.childCount > 1)
            {
                var c = parent.GetChild(0).GetComponent<RectTransform>(); 
                c.SetParent(home);
                c.anchoredPosition = new Vector2(c.anchoredPosition.x,c.anchoredPosition.y + 13);
            }
        }
    }
}
