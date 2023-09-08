using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class TechIconButtonInfo : MonoBehaviour
{
    [SerializeField] private List<Button> iconInfoButtons; 
    [SerializeField] private List<GameObject> iconInfoObject; 

    private void OnEnable()
    {
        for (var i = 0; i < iconInfoButtons.Count; i++)
        {
            iconInfoButtons[i].onClick.RemoveAllListeners();
            var j = i;
            iconInfoButtons[i].onClick.AddListener((() =>
            {
                ShowInfo(j);
            }));
        }
    }

    private void ShowInfo(int index)
    {
        foreach (var icon in iconInfoObject)
        {
            icon.SetActive(false);
        }
        iconInfoObject[index].transform.parent.gameObject.SetActive(true);
        iconInfoObject[index].SetActive(true);
    }
}
