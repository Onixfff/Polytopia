using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailCivInfoWindow : MonoBehaviour
{
    [SerializeField] private Image head;
    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI civName;
    [SerializeField] private TextMeshProUGUI civDescription;
    [SerializeField] private TextMeshProUGUI civRelation;
    [SerializeField] private TextMeshProUGUI civRelation1;
    [SerializeField] private GameObject peaceIcon;
    [SerializeField] private GameObject warIcon;

    public void Open(CivilisationController controller)
    {
        head.sprite = controller.civilisationInfo.HeadSprite;
        colorImage.color = controller.civColor;
        civName.text = controller.civilisationInfo.civilisationName;
        civDescription.text = $"Племя {controller.civilisationInfo.civilisationName} управляется игроком {controller.civilName}({controller.civilisationInfo.controlType.ToString()})";
        civRelation.text = $"Ваши отношения: ";
        civRelation1.text = $"Они считают, чтовы обоятельный, мудрый и дипломатичный правитель";
    }
}
