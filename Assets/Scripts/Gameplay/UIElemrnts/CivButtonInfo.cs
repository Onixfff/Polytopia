using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivButtonInfo : MonoBehaviour
{
    [SerializeField] private Image head;
    [SerializeField] private Image colorImage;
    [SerializeField] private Image backImage;
    [SerializeField] private TextMeshProUGUI civName;
    [SerializeField] private TextMeshProUGUI civPoint;
    [SerializeField] private List<TextMeshProUGUI> civInfos;
    [SerializeField] private GameObject peaceIcon;
    [SerializeField] private GameObject warIcon;
    [SerializeField] private Color backColorDef;
    [SerializeField] private Color backColorPlayer;
    private CivilisationController _civilisationController;
    
    public void Init(CivilisationController controller)
    {
        colorImage.gameObject.SetActive(false);
        
        _civilisationController = controller;
        head.sprite = controller.civilisationInfo.HeadSprite;
        colorImage.color = controller.civColor;
        backImage.color = controller.civilisationInfo.controlType == CivilisationInfo.ControlType.Player ? backColorPlayer : backColorDef;

        UpdateInfoWindow(controller);
        DiplomacyManager.Instance.OnRelationChange += ChangeRelationVisual;
    }
    
    public void UpdateInfoWindow(CivilisationController controller)
    {
        var playerCiv = LevelManager.Instance.gameBoardWindow.playerCiv;
        if (playerCiv.relationOfCivilisation.CheckCivForContain(controller) || controller.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            GetComponent<Button>().enabled = true;
            colorImage.gameObject.SetActive(true);
            head.transform.parent.gameObject.SetActive(true);
            civName.text = controller.civilisationInfo.civilisationType.ToString();
            civPoint.text = controller.Point.ToString();
            foreach (var civInfo in civInfos)
            {
                civInfo.text = "Правитель: " + controller.civilName + $"({controller.civilisationInfo.controlType.ToString()}), " + $"Город: {controller.homes.Count}";
            }
        }
        else
        {
            GetComponent<Button>().enabled = false;
            head.transform.parent.gameObject.SetActive(false);
            colorImage.gameObject.SetActive(false);
            civName.text = "Неизвестное племя";
            civPoint.text = controller.Point.ToString();
            foreach (var civInfo in civInfos)
            {
                civInfo.text = $"Неизвестный правитель, Город: {controller.homes.Count}";
            }
        }
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
