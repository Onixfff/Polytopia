using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivButtonInfo : MonoBehaviour
{
    [SerializeField] private Image head;
    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI civPoint;
    [SerializeField] private TextMeshProUGUI civInfo;
    [SerializeField] private GameObject peaceIcon;
    [SerializeField] private GameObject warIcon;
    
    public void Init(CivilisationController controller)
    {
        head.sprite = controller.civilisationInfo.HeadSprite;
        colorImage.color = controller.civColor;
        UpdateInfo();
        LevelManager.Instance.OnTurnBegin += UpdateInfo;
        
        void UpdateInfo()
        {
            civPoint.text = controller.civilisationInfo.civilisationName + "......." + controller.Point;
            civInfo.text = "Правитель: " + controller.civilName + $"({controller.civilisationInfo.controlType.ToString()})" + $", городов: {controller.homes.Count}";
        }
    }
}
