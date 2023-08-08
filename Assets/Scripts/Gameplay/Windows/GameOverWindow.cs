using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : BaseWindow
{
    [SerializeField] private Button menuButton;
    
    private void Start()
    {
        menuButton.onClick.AddListener(LoadMenuScene);
    }
    
    private void LoadMenuScene()
    {
        LevelManager.Instance.DestroyAllCivilisation();
        WindowsManager.Instance.CloseAllWindows();
        WindowsManager.Instance.CreateWindow<MenuWindow>("MenuWindow").ShowWindow();
    }
}