using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechInfoWindow : MonoBehaviour
{
    public Button openTechButton;
    public List<Button> techInfoBackButtons;
    public TextMeshProUGUI techInfoNameText;
    public TextMeshProUGUI price;
    public GameObject noMoneyObject;
    public GameObject closeObject;
    public GameObject completeObject;
    public Color openButtonColor;
    public GameObject backBuyObject;
    public GameObject backObject;
}
