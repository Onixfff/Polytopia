using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour
{
    [SerializeField] private RectTransform button;
    [SerializeField] private RectTransform tmPro;
    [SerializeField] private Image image;
    [SerializeField] private float time = 1;

    
    
    public void ChangeColor(DiplomacyManager.RelationType type)
    {
        switch (type)
        {
            case DiplomacyManager.RelationType.War:
                image.color = Color.red;
                break;
            case DiplomacyManager.RelationType.Neutral:
                image.color = Color.yellow;
                break;
            case DiplomacyManager.RelationType.Peace:
                image.color = Color.green;
                break;
        }
    }
    
    [Button()]
    public void AutoSize()
    {
        AutoSizeText();
        Invoke(nameof(AutoSizeButton), time);
    }

    private void AutoSizeText()
    {
        var proUGUI = tmPro.GetComponent<TextMeshProUGUI>();
        proUGUI.autoSizeTextContainer = false;
        proUGUI.autoSizeTextContainer = true;
    }

    private void AutoSizeButton()
    {
        var sizeDelta = tmPro.sizeDelta;
        button.sizeDelta = new Vector2(sizeDelta.x + 13, sizeDelta.y);
    }

    public void AddOffsetByX(float offset)
    {
        var anchoredPosition = button.anchoredPosition;
        if(anchoredPosition.x > 0)
            anchoredPosition = new Vector2(50 + offset, anchoredPosition.y);
        if(anchoredPosition.x < 0)
            anchoredPosition = new Vector2(-50 - offset, anchoredPosition.y);

        button.anchoredPosition = anchoredPosition;
    }
}
