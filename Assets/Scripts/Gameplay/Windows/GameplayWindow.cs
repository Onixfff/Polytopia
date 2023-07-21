using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : BaseWindow
{
    public Action OnUnitSpawn;

    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI tileNameUGUI;
    [SerializeField] private Button civilisationButton;
    private void Awake()
    {
        LevelManager.Instance.gameplayWindow = this;
        menuButton.onClick.AddListener(LoadMenuScene);
        civilisationButton.onClick.AddListener(SpawnUnit);

        LevelManager.Instance.OnObjectSelect += (o, o1) =>
        {
            if (o1 != null && o1.TryGetComponent(out CivilisationController civ))
            {
                return;
            }
            
            civilisationButton.gameObject.SetActive(false);
        };
    }

    private void LoadMenuScene()
    {
        WindowsManager.Instance.HideAllWindows();
        WindowsManager.Instance.CreateWindow<MenuWindow>("MenuWindow").ShowWindow();
    }

    public void SetTileName(string tileName)
    {
        tileNameUGUI.text = tileName;
    }
    
    public void ShowCivilisationButton()
    {
        civilisationButton.gameObject.SetActive(true);
    }

    private void SpawnUnit()
    {
        OnUnitSpawn?.Invoke();
    }
}
