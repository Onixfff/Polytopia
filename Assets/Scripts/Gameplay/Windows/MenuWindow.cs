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
        WindowsManager.Instance.HideAllWindows();
        WindowsManager.Instance.CreateWindow<GameplayWindow>("GameplayWindow").ShowWindow();
    }
}
