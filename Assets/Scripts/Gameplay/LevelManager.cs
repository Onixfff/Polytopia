using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SO;
using UnityEngine;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public Action<GameObject, GameObject> OnObjectSelect;
    public Action<TechInfo.Technology> OnUnlockTechnology;
    public Action OnTurnBegin;
    public Action OnTurnEnd;
    public int currentTurn = 0;
    
    public GameBoardWindow gameBoardWindow;
    public GameplayWindow gameplayWindow;
    public string currentName;
    
    public List<Sprite> waterSprites;
    
    [SerializeField] private GameObject selectedObject;
    
    private List<CivilisationController> _civilisationControllers;

    public void SelectObject(GameObject objectForSelect)
    {
        if(objectForSelect != null) 
            gameplayWindow.SetTileName(currentName);
        if (objectForSelect == null)
            gameplayWindow.SetTileName("");
        OnObjectSelect?.Invoke(selectedObject, objectForSelect);
        selectedObject = objectForSelect;
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }

    public void DestroyAllCivilisation()
    {
        AIController.Instance.ClearAI();
        var civs = transform.GetComponentsInChildren<CivilisationController>();
        for (var i = civs.Length - 1; i >= 0; i--)
        {
            civs[i].DestroyAllUnits();
            Destroy(civs[i].gameObject);
        }
    }

    public List<CivilisationController> GetCivilisationControllers()
    {
        return _civilisationControllers;
    }

    public void AddCiv(CivilisationController civ)
    {
        _civilisationControllers ??= new List<CivilisationController>();
        _civilisationControllers.Add(civ);
    }
    
    public void RemoveCiv(CivilisationController civ)
    {
        if (civ.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            ShowLoseWindow();
            return;
        }
        _civilisationControllers?.Remove(civ);
        
        if(_civilisationControllers?.Count == 1)
            ShowWinWindow();
    }

    public void ClearCivList()
    {
        _civilisationControllers.Clear();
    }
    
    public void ShowWinWindow()
    {
        var a = WindowsManager.Instance.CreateWindow<VictoryWindow>("VictoryWindow");
        a.ShowWindow();
        a.OnTop();
    }
    
    public void ShowLoseWindow()
    {
        if(_civilisationControllers.Count == 0)
            return;
        
        var a = WindowsManager.Instance.CreateWindow<GameOverWindow>("GameOverWindow");
        a.ShowWindow();
        a.OnTop();
    }

    private void Start()
    {
        OnTurnEnd += CheckWin;
    }

    private void CheckWin()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.Classic)
        {
            if (currentTurn >= 30)
            {
                var maxPoint = 0;
                var winner = _civilisationControllers[0];
                foreach (var civilisationController in _civilisationControllers)
                {
                    if (civilisationController.Point > maxPoint)
                    {
                        maxPoint = civilisationController.Point;
                        winner = civilisationController;
                    }
                }

                foreach (var civilisationController in _civilisationControllers)
                {
                    if(civilisationController == winner)
                        continue;
                    RemoveCiv(civilisationController);
                }
            }
        }
    }
}