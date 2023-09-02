using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : BaseWindow
{
    [SerializeField] private Button classicButton;
    [SerializeField] private Button occupyButton;
    [SerializeField] private Button sandBoxButton;

    private void Start()
    {
        classicButton.onClick.AddListener(LoadPlayScene);
        occupyButton.onClick.AddListener(LoadPlayScene);
        sandBoxButton.onClick.AddListener(LoadPlayScene);
    }

    private void LoadPlayScene()
    {
        WindowsManager.Instance.CloseAllWindows();
        var a = WindowsManager.Instance.CreateWindow<GameplayWindow>("GameplayWindow");
        WindowsManager.Instance.CreateWindow<GameBoardWindow>("GameBoardWindow").ShowWindow();
        a.OnTop();
        a.ShowWindow();
    }
}
