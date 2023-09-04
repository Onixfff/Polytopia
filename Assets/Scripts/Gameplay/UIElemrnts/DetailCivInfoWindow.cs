using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailCivInfoWindow : MonoBehaviour
{
    [SerializeField] private Image head;
    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI civName;
    [SerializeField] private TextMeshProUGUI civDescription;
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
        civName.text = controller.civilisationInfo.civilisationType.ToString();
        civName.autoSizeTextContainer = false;
        civName.autoSizeTextContainer = true;
        if (controller.civilisationInfo.controlType != CivilisationInfo.ControlType.Player)
        {
            civDescription.text = $"Племя {controller.civilisationInfo.civilisationType} управляется игроком {controller.civilName}({controller.civilisationInfo.controlType.ToString()})";
            civName.transform.GetChild(0).GetComponent<ButtonScale>().SetPosByX((civName.text.Length+civName.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text.Length) * 9);
        }
    }
    
    private void ChangeRelationVisual()
    {
        if (_civilisationController != null)
        {
            if (peaceIcon == null || warIcon == null) return; 
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
