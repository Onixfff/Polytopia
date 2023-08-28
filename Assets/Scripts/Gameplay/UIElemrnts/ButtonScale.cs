using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonScale : MonoBehaviour
{
    [SerializeField] private RectTransform button;
    [SerializeField] private RectTransform tmPro;
    [SerializeField] private float time = 1;
    
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
        button.sizeDelta = new Vector2(sizeDelta.x + 15, sizeDelta.y);
    }
}
