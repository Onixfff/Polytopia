using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivilisationChooserInfoWindow : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Image headImage;

    [SerializeField] private TextMeshProUGUI civNameText;
    [SerializeField] private TextMeshProUGUI civDescriptionText;
    [SerializeField] private TextMeshProUGUI civStartTechText;
    private void OnEnable()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(CloseInfoWindow);
        ShowInfo();
    }

    private void CloseInfoWindow()
    {
        gameObject.SetActive(false);
        GameManager.Instance.playerCivInfo = null;
    }

    private void ShowInfo()
    {
        var playerCivInfo = GameManager.Instance.playerCivInfo;
        var civilisationType = playerCivInfo.civilisationType;

        headImage.sprite = playerCivInfo.HeadSprite;
        civNameText.text = civilisationType.ToString();
        civStartTechText.text = $"Племя {civilisationType} начинает игру с технологией {playerCivInfo.technology.startTechnologies}";
        switch (civilisationType)
        {
            case CivilisationInfo.CivilisationType.China:
                civDescriptionText.text = "Это племя начинает свой путь в высоких горах среди опадающих лепестков цветущей вишни.";
                break;
            case CivilisationInfo.CivilisationType.Imperium:
                civDescriptionText.text = "Огромные горы и зелёные долины. Земли племени Рим как нельзя лучше подходят для выращивания фруктов.";
                break;
            case CivilisationInfo.CivilisationType.Bardur:
                civDescriptionText.text = "Выжить посреди вечно покрытых снегом лесов племени Викингов нелегко, но они процветают даже в таких тяжёлых условиях.";
                break;
            case CivilisationInfo.CivilisationType.Oumaji:
                civDescriptionText.text = "Домом племени Арабов служит бесконечная выжженная солнцем пустыня.";
                break;
            case CivilisationInfo.CivilisationType.Kickoo:
                civDescriptionText.text = "Белые песчаные пляжи и кокосовые пальмы, изобилие рыбы и фруктов. Добро пожаловать в земли племени Индейцев.";
                break;
            case CivilisationInfo.CivilisationType.Hoodrick:
                civDescriptionText.text = "Жёлтые листья лесов племени Англичан укрывают его жителей от постороннего взгляда, когда те собирают грибы.";
                break;
        }        
    }
}