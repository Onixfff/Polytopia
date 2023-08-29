using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivilisationInfoWindow : MonoBehaviour
{
    [SerializeField] private GameObject civDetailObject;
    [SerializeField] private CivButtonInfo civInfoButtonPrefab;
    [SerializeField] private Transform civInfoButtonParent;
    [SerializeField] private TextMeshProUGUI relationText;
    [SerializeField] private TextMeshProUGUI civRelation;
    [SerializeField] private TextMeshProUGUI civOpinionsText;
    [SerializeField] private List<TextMeshProUGUI> opinionsText;
    [SerializeField] private Button peaceButton;
    [SerializeField] private Button embassyButton;

    private Dictionary<CivButtonInfo, CivilisationController> _civInfoButtons;

    public void ChangeRelation(DiplomacyManager.RelationType value)
    {
        var text = "";
        switch (value)
        {
            case DiplomacyManager.RelationType.War:
                text = "война";
                break;
            case DiplomacyManager.RelationType.Neutral:
                text = "нейтральные";
                break;
            case DiplomacyManager.RelationType.Peace:
                text = "мир";
                break;
        }
        relationText.text = text;
        relationText.transform.parent.GetComponent<ButtonScale>().AutoSize();
        relationText.transform.parent.GetComponent<ButtonScale>().ChangeColor(value);
    }

    public void ChangeOpinion(List<DiplomacyManager.OpinionType> values, int opinionValue)
    {
        if (values.Count < 3)
        {
            Debug.Log("values.Count < 3");
            return;
        }
        var texts = new List<string>();
        var text = "";
        for (var i = 0; i < 3; i++)
        {
            var opinion = opinionsText[i];
            
            var isGood = false;
            
            switch (values[i])
            {
                case DiplomacyManager.OpinionType.Wise:
                    text = "мудрый";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Charming:
                    text = "обаятельный";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Peaceful:
                    text = "мирный";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Diplomatic:
                    isGood = true;
                    text = "дипломатичный";
                    break;
                case DiplomacyManager.OpinionType.Powerful:
                    isGood = true;
                    text = "мощный";
                    break;
                case DiplomacyManager.OpinionType.Brave:
                    isGood = true;
                    text = "храбрый";
                    break;
                case DiplomacyManager.OpinionType.Violent:
                    text = "жестокий";
                    break;
                case DiplomacyManager.OpinionType.Threatening:
                    text = "угрожающий";
                    break;
                case DiplomacyManager.OpinionType.Weak:
                    text = "слабый";
                    break;
                case DiplomacyManager.OpinionType.Annoying:
                    text = "раздражающий";
                    break;
                case DiplomacyManager.OpinionType.Foolish:
                    text = "глупый";
                    break;
                case DiplomacyManager.OpinionType.Intrusive:
                    text = "навязчивый";
                    break;
                case DiplomacyManager.OpinionType.Dominating:
                    text = "доминирующий";
                    break;
            }
            opinion.text = text;
            var buttonScale = opinion.transform.parent.GetComponent<ButtonScale>();
            
            buttonScale.AutoSize();
            var offset = 0f;
            switch (i)
            {
                case 1:
                    offset = (text.Length-6) * 6;
                    opinionsText[2].transform.parent.GetComponent<ButtonScale>().AddOffsetByX(offset);
                    break;
                case 2:
                    offset = (text.Length-6) * 6;
                    opinionsText[1].transform.parent.GetComponent<ButtonScale>().AddOffsetByX(offset);
                    break;
            }

            if (isGood)
            {
                buttonScale.ChangeColor(DiplomacyManager.RelationType.Peace);
            }
            else
            {
                buttonScale.ChangeColor(DiplomacyManager.RelationType.War);
            }
            texts.Add(text);
        }

        switch (opinionValue)
        {
            case <= -3:
                text = "ужасные";
                break;
            case -2:
                text = "плохие";
                break;
            case -1:
                text = "негативные";
                break;
            case 0:
                text = "нетральные";
                break;
            case 1:
                text = "хорошие";
                break;
            case 2:
                text = "оличные";
                break;
            case >= 3:
                text = "великолепные";
                break;
        }
        
        civRelation.text = $"Ваши отношения: {text}";
        civOpinionsText.text = $"Они считают, что вы {texts[0]}, <br>{texts[1]} и {texts[2]} <br>правитель";
    }
    
    private void Start()
    {
        _civInfoButtons = new Dictionary<CivButtonInfo, CivilisationController>();
        CreateCivilizationInfoButton();
        foreach (var button in _civInfoButtons)
        {
            button.Key.GetComponent<Button>().onClick.RemoveAllListeners();
            button.Key.GetComponent<Button>().onClick.AddListener((() =>
            {
                ShowCivDetailedInformation(button.Value);
            }));
        }
    }

    private void ShowCivDetailedInformation(CivilisationController controller)
    {
        civDetailObject.SetActive(true);
        var relationOfCivilisation = controller.GetComponent<RelationOfCivilisation>();
        var relationType = relationOfCivilisation.GetCivilisationRelation()[LevelManager.Instance.gameBoardWindow.playerCiv];
        ChangeRelation(relationType);
        
        
        var opinionType = relationOfCivilisation.GetCivilisationOpinion()[LevelManager.Instance.gameBoardWindow.playerCiv];
        var opinionTypes = new List<DiplomacyManager.OpinionType>();
        var opinionValue = relationOfCivilisation.CalculateOpinion(controller);
        if (opinionValue < 0)
        {
            opinionTypes.Add(opinionType[0]);
            opinionTypes.Add(opinionType[1]);
            opinionTypes.Add(opinionType[^1]);
        }
        if(opinionValue > 0)
        {
            opinionTypes.Add(opinionType[0]);
            opinionTypes.Add(opinionType[^1]);
            opinionTypes.Add(opinionType[^2]);
        }
        ChangeOpinion(opinionTypes, opinionValue);
        
        
        civDetailObject.GetComponent<DetailCivInfoWindow>().Open(controller);
        
        peaceButton.onClick.RemoveAllListeners();
        peaceButton.onClick.AddListener(() =>
        {
            MakePeace(controller);
        });
        embassyButton.onClick.RemoveAllListeners();
        embassyButton.onClick.AddListener(() =>
        {
            SendAnEmbassy(controller);
        });
        //ChangeOpinion();
    }

    private void MakePeace(CivilisationController controller)
    {
        controller.ProposeAnAlliance(LevelManager.Instance.gameBoardWindow.playerCiv);
    }
    
    private void SendAnEmbassy(CivilisationController controller)
    {
        
    }

    private void CreateCivilizationInfoButton()
    {
        var civilisationControllers = LevelManager.Instance.GetCivilisationControllers();
        foreach (var controller in civilisationControllers)
        {
            var button = Instantiate(civInfoButtonPrefab, civInfoButtonParent);
            button.Init(controller);
            _civInfoButtons.Add(button, controller);
        }
    }
}
