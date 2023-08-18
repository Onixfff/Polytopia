using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : BaseWindow
{
    [SerializeField] private Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(LoadPlayScene);
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
