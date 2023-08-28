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
    private CivilisationController _civilisationController;

    public void Open(CivilisationController controller)
    {
        _civilisationController = controller;
        ChangeRelationVisual();
        DiplomacyManager.Instance.OnRelationChange -= ChangeRelationVisual;
        DiplomacyManager.Instance.OnRelationChange += ChangeRelationVisual;
        head.sprite = controller.civilisationInfo.HeadSprite;
        colorImage.color = controller.civColor;
        civName.text = controller.civilisationInfo.civilisationName;
        civDescription.text = $"Племя {controller.civilisationInfo.civilisationName} управляется игроком {controller.civilName}({controller.civilisationInfo.controlType.ToString()})";
        civRelation.text = $"Ваши отношения: ";
        civRelation1.text = $"Они считают, чтовы обоятельный, мудрый и дипломатичный правитель";
    }
    
    private void ChangeRelationVisual()
    {
        if (_civilisationController != null)
        {
            var player = LevelManager.Instance.gameBoardWindow.playerCiv;
            var relationType = _civilisationController.relationOfCivilisation.GetRelation(player);
            if (relationType == DiplomacyManager.RelationType.Peace)
            {
                warIcon.SetActive(false);
                peaceIcon.SetActive(true);
            }
            else if(relationType == DiplomacyManager.RelationType.War)
            {
                peaceIcon.SetActive(false);
                warIcon.SetActive(true);
            }
            else
            {
                peaceIcon.SetActive(false);
                warIcon.SetActive(false);
            }
        }
    }
}
