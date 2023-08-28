using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitAbilityButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI tileCoastUGUI;
    [SerializeField] private int coast;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    private void OnEnable()
    {
        /*var curMoney = LevelManager.Instance.gameBoardWindow.playerCiv.Money;
        tileCoastUGUI.text = coast.ToString();
        if(coast == 0)
            tileCoastUGUI.transform.parent.gameObject.SetActive(false);
        if (curMoney < coast)
        {
            button.enabled = false;
            image.color = disableColor;
        }
        else
        {
            button.enabled = true;
            image.color = enableColor;
        }*/
    }
}
