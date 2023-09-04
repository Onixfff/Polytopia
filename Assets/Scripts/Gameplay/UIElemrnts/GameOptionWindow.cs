using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionWindow : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private MenuWindow menuWindow;
    
    [SerializeField] private List<GameObject> objectGorHide;
    
    [SerializeField] private List<GameObject> gameMods;
    [SerializeField] private List<GameObject> gameEnemyCount;
    [SerializeField] private List<GameObject> gameDifficult;
    [SerializeField] private List<GameObject> gameMapSize;
    [SerializeField] private TextMeshProUGUI gameInfoText;

    public GameManager.GameMode mode;
    private GameManager.GameMode _gameMode;
    private GameManager.Difficult _gameDifficult;
    private int _countEnemy;
    private int _mapSize;
    
    private bool _isOption;
    private bool _isMode;

    private void OnEnable()
    {
        if (mode is GameManager.GameMode.Classic or GameManager.GameMode.Occupy)
        {
            _isMode = true;
            _gameMode = mode;
            _mapSize = 11;
            foreach (var hide in objectGorHide)
            {
                hide.SetActive(false);
            }
        }
        else
        {
            _isMode = false;
            foreach (var hide in objectGorHide)
            {
                hide.SetActive(true);
            }
        }
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartGame);
        _isOption = true;
    }
    
    private void OnDisable()
    {
        _isOption = false;
        
    }

    private void StartGame()
    {
        menuWindow.LoadPlayScene();
    }

    private void Update()
    {
        if(!_isOption)
            return;
        
        CheckDifficult();
        CheckCountEnemy();
        if (_isMode)
        {
            UpdateInfo();
            return;
        }
        CheckGameMod();
        CheckMapSize();
        UpdateInfo();
    }

    private void CheckGameMod()
    {
        for (var i = 0; i < gameMods.Count; i++)
        {
            if (gameMods[i].transform.position.x is <= 1 and >= -1)
            {
                switch (i)
                {
                    case 0:
                        _gameMode = GameManager.GameMode.Classic;
                        break;
                    case 1:
                        _gameMode = GameManager.GameMode.Occupy;
                        break;
                    case 2:
                        _gameMode = GameManager.GameMode.Endless;
                        break;
                }
                break;
            }            
        }
    }
    
    private void CheckDifficult()
    {
        for (var i = 0; i < gameDifficult.Count; i++)
        {
            if (gameDifficult[i].transform.position.x is <= 1 and >= -1)
            {
                switch (i)
                {
                    case 0:
                        _gameDifficult = GameManager.Difficult.Easy;
                        break;
                    case 1:
                        _gameDifficult = GameManager.Difficult.Normal;
                        break;
                    case 2:
                        _gameDifficult = GameManager.Difficult.Hard;
                        break;
                }
                break;
            }            
        }
    }
    
    private void CheckCountEnemy()
    {
        for (var i = 0; i < gameEnemyCount.Count; i++)
        {
            if (gameEnemyCount[i].transform.position.x is <= 0.5f and >= -0.5f)
            {
                _countEnemy = i + 1;
                break;
            }            
        }
    }
    
    private void CheckMapSize()
    {
        for (var i = 0; i < gameMapSize.Count; i++)
        {
            if (gameMapSize[i].transform.position.x is <= 1 and >= -1)
            {
                switch (i)
                {
                    case 0:
                        _mapSize = 11;
                        break;
                    case 1:
                        _mapSize = 13;
                        break;
                    case 2:
                        _mapSize = 16;
                        break;
                    case 3:
                        _mapSize = 19;
                        break;
                }

                break;
            }            
        }
    }
    
    private void UpdateInfo()
    {
        var gameManager = GameManager.Instance;

        gameManager.gameMode = _gameMode;
        gameManager.difficult = _gameDifficult;
        gameManager.countEnemy = _countEnemy;
        gameManager.mapSize = _mapSize;
        
        gameInfoText.text = $"Противников: {_countEnemy}. Клеток на карте: {_mapSize*_mapSize}";
    }
}
