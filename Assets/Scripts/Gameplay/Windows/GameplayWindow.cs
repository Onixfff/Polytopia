using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : BaseWindow
{
    [SerializeField] private Button menuButton;
    
    private void Start()
    {
        menuButton.onClick.AddListener(LoadMenuScene);
    }

    private void LoadMenuScene()
    {
        WindowsManager.Instance.HideAllWindows();
        WindowsManager.Instance.CreateWindow<MenuWindow>("MenuWindow").ShowWindow();
    }
}
