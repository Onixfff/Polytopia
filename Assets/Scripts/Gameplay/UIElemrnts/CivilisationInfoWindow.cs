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
    [SerializeField] private List<TextMeshProUGUI> opinionsText;
    [SerializeField] private Button peaceButton;
    [SerializeField] private Button embassyButton;

    private Dictionary<CivButtonInfo, CivilisationController> _civInfoButtons;

    public void ChangeRelation(string value)
    {
        relationText.text = value;
        relationText.transform.parent.GetComponent<ButtonScale>().AutoSize();
    }
    
    public void ChangeOpinion(List<string> values)
    {
        for (var i = 0; i < opinionsText.Count; i++)
        {
            var opinion = opinionsText[i];
            opinion.text = values[i];
        }
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
        ChangeRelation(relationType.ToString());
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
