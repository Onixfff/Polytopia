using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivButtonInfo : MonoBehaviour
{
    [SerializeField] private Image head;
    [SerializeField] private Image colorImage;
    [SerializeField] private Image backImage;
    [SerializeField] private TextMeshProUGUI civPoint;
    [SerializeField] private TextMeshProUGUI civInfo;
    [SerializeField] private GameObject peaceIcon;
    [SerializeField] private GameObject warIcon;
    [SerializeField] private Color backColorDef;
    [SerializeField] private Color backColorPlayer;
    private CivilisationController _civilisationController;
    
    public void Init(CivilisationController controller)
    {
        _civilisationController = controller;
        head.sprite = controller.civilisationInfo.HeadSprite;
        colorImage.color = controller.civColor;
        backImage.color = controller.civilisationInfo.controlType == CivilisationInfo.ControlType.Player ? backColorPlayer : backColorDef;

        UpdateInfo();
        LevelManager.Instance.OnTurnBegin += UpdateInfo;
        DiplomacyManager.Instance.OnRelationChange += ChangeRelationVisual;
        void UpdateInfo()
        {
            civPoint.text = controller.civilisationInfo.civilisationName + "......." + controller.Point;
            civInfo.text = "Правитель: " + controller.civilName + $"({controller.civilisationInfo.controlType.ToString()})" + $", городов: {controller.homes.Count}";
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
