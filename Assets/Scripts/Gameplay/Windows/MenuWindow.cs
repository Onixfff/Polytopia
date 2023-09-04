
public class MenuWindow : BaseWindow
{
    public void LoadPlayScene()
    {
        WindowsManager.Instance.CloseAllWindows();
        var gameBoardWindow = WindowsManager.Instance.CreateWindow<GameBoardWindow>("GameBoardWindow");
        var gameplayWindow = WindowsManager.Instance.CreateWindow<GameplayWindow>("GameplayWindow");
        
        gameBoardWindow.ShowWindow();
        
        gameplayWindow.OnTop();
        gameplayWindow.ShowWindow();
    }
}
