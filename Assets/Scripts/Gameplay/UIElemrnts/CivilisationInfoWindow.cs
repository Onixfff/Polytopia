using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivilisationInfoWindow : MonoBehaviour
{
    [SerializeField] private GameObject playerCivDetailObject;
    [SerializeField] private GameObject civDetailObject;
    [SerializeField] private CivButtonInfo civInfoButtonPrefab;
    [SerializeField] private Transform civInfoButtonParent;
    [SerializeField] private TextMeshProUGUI relationText;
    [SerializeField] private TextMeshProUGUI civRelation;
    [SerializeField] private TextMeshProUGUI civOpinionsText;
    [SerializeField] private List<TextMeshProUGUI> opinionsText;
    [SerializeField] private Button peaceButton;
    [SerializeField] private Button embassyButton;
    [SerializeField] private GameObject questCountObject;
    [SerializeField] private List<GameObject> quests;

    private Dictionary<CivButtonInfo, CivilisationController> _civInfoButtons;
    private List<GameObject> _civInfoObjects;

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
            case DiplomacyManager.RelationType.None:
                text = "нейтральные";
                break;
        }
        relationText.text = text;
        var parent = relationText.transform.parent.GetComponent<ButtonScale>();
        parent.AutoSize();
        parent.ChangeColor(value);
    }

    public void ChangeOpinion(DiplomacyManager.RelationType relationType, List<DiplomacyManager.OpinionType> values, int opinionValue)
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
                    text = "дипломатичный";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Powerful:
                    text = "мощный";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Brave:
                    text = "храбрый";
                    isGood = true;
                    break;
                case DiplomacyManager.OpinionType.Violent:
                    text = "жестокий";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Threatening:
                    text = "угрожающий";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Weak:
                    text = "слабый";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Annoying:
                    text = "раздражающий";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Foolish:
                    text = "глупый";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Intrusive:
                    text = "навязчивый";
                    isGood = false;
                    break;
                case DiplomacyManager.OpinionType.Dominating:
                    text = "доминирующий";
                    isGood = false;
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

        if (relationType == DiplomacyManager.RelationType.None)
        {
            foreach (var op in opinionsText)
            {
                op.transform.parent.gameObject.SetActive(false);
            }
            civRelation.text = "Они пока что о вас не знают";
            civOpinionsText.text = "";
        }
        else
        {
            foreach (var op in opinionsText)
            {
                op.transform.parent.gameObject.SetActive(true);
            }
            civRelation.text = $"Ваши отношения: {text}";
            civOpinionsText.text = $"Они считают, что вы {texts[0]}, <br>{texts[1]} и {texts[2]} <br>правитель";
        }
    }
    
    private void Start()
    {
        _civInfoButtons ??= new Dictionary<CivButtonInfo, CivilisationController>();
        _civInfoObjects ??= new List<GameObject>();
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

    private void OnEnable()
    {
        CheckQuests();
        _civInfoButtons ??= new Dictionary<CivButtonInfo, CivilisationController>();
        _civInfoObjects ??= new List<GameObject>();
        foreach (var keyValue in _civInfoButtons)
        {
            keyValue.Key.UpdateInfoWindow(keyValue.Value);
        }
    }

    private void ShowCivDetailedInformation(CivilisationController controller)
    {
        if (controller.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            playerCivDetailObject.SetActive(true);
            playerCivDetailObject.GetComponent<DetailCivInfoWindow>().Open(controller);
        }
        else
        {
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
            ChangeOpinion(relationType, opinionTypes, opinionValue);
            
            civDetailObject.SetActive(true);
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
        }
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
            button.transform.SetSiblingIndex(5);
            button.Init(controller);
            _civInfoButtons.Add(button, controller);
            _civInfoObjects.Add(button.gameObject);
        }
    }

    private void CheckQuests()
    {
        var player = LevelManager.Instance.gameBoardWindow.playerCiv;
        var playerStats = player.GetComponent<CivilisationStats>();
        
        for (var i = 0; i < quests.Count; i++)
        {
            var quest = quests[i];

            switch (i)
            {
                case 0:
                    if (playerStats.GetCountMaxCityLevel() != 0)
                    {
                        if (playerStats.GetCountMaxCityLevel() == 5)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountMaxCityLevel()}/5";
                    }
                    break;
                case 1:
                    if (playerStats.GetCountKilledUnit() != 0)
                    {
                        if (playerStats.GetCountKilledUnit() == 10)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountKilledUnit()}/10";
                    }
                    break;
                case 2:
                    if(!player.technologies.Contains(TechInfo.Technology.Meditation))
                        continue;
                    if (playerStats.GetCountTurnWithoutAttack() >= 6)
                        return;
                    if (playerStats.GetCountTurnWithoutAttack() >= 5)
                    {
                        quest.SetActive(true);
                        quest.transform.GetChild(0).gameObject.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                        continue;
                    }
                    quest.SetActive(true);
                    quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountTurnWithoutAttack()}/5";
                    break;
                case 3:
                    if(!player.technologies.Contains(TechInfo.Technology.Philosophy))
                        continue;
                    if (playerStats.GetCountUnlockedTech() != 0)
                    {
                        if (playerStats.GetCountUnlockedTech() == 26)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountUnlockedTech()}/26";
                    }
                    break;
                case 4:
                    if(!player.technologies.Contains(TechInfo.Technology.Roads))
                        continue;
                    if (playerStats.GetCountConnectedCity() != 0)
                    {
                        if (playerStats.GetCountConnectedCity() == 5)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountConnectedCity()}/5";
                    }
                    break;
                case 5:
                    if(!player.technologies.Contains(TechInfo.Technology.Navigation))
                        continue;
                    if (playerStats.GetCountExploredTile() != 0)
                    {
                        var count = LevelManager.Instance.gameBoardWindow.GetAllTile().Count;
                        if (playerStats.GetCountExploredTile() == count)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetCountExploredTile()}/{count}";
                    }
                    break;
                case 6:
                    if(!player.technologies.Contains(TechInfo.Technology.Trade))
                        continue;
                    if (playerStats.GetAllStars() != 0)
                    {
                        if (playerStats.GetAllStars() == 100)
                        {
                            quest.SetActive(true);
                            quest.transform.GetChild(0).gameObject.SetActive(true);
                            quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Выполнено";
                            continue;
                        }
                        quest.SetActive(true);
                        quest.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerStats.GetAllStars()}/100";
                    }
                    break;
            }
        }
        questCountObject.GetComponent<TextMeshProUGUI>().text = $"Задание: {quests.FindAll(qu => qu.activeSelf).Count}/7";
    }
}
