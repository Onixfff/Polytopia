using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUnitButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI unitCoastUGUI;
    [SerializeField] private int coast;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    private void OnEnable()
    {
        var curMoney = LevelManager.Instance.gameBoardWindow.playerCiv.Money;
        unitCoastUGUI.text = coast.ToString();
        if (curMoney < coast)
        {
            button.enabled = false;
            image.color = disableColor;
        }
        else
        {
            button.enabled = true;
            image.color = enableColor;
        }
    }
}
