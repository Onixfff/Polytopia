using System;
using UnityEngine;
using UnityEngine.UI;

public class GameModeButtonsWindow : MonoBehaviour
{
    [SerializeField] private Button classicButton;
    [SerializeField] private Button occupyButton;
    [SerializeField] private Button sandBoxButton;
    
    [SerializeField] private GameOptionWindow gameOptionWindow;

    private void OnEnable()
    {
        classicButton.onClick.RemoveAllListeners();
        occupyButton.onClick.RemoveAllListeners();
        sandBoxButton.onClick.RemoveAllListeners();
        classicButton.onClick.AddListener(ClassicMode);
        occupyButton.onClick.AddListener(OccupyMode);
        sandBoxButton.onClick.AddListener(SandBoxMode);
    }

    private void ClassicMode()
    {
        gameOptionWindow.mode = GameManager.GameMode.Classic;
    }
    
    private void OccupyMode()
    {
        gameOptionWindow.mode = GameManager.GameMode.Occupy;
    }
    
    private void SandBoxMode()
    {
        gameOptionWindow.mode = GameManager.GameMode.Endless;
    }
}
