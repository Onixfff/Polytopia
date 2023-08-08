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
    public int currentIncome = 2;
    
    public GameBoardWindow gameBoardWindow;
    public GameplayWindow gameplayWindow;
    [SerializeField]private GameObject _selectedObject;
    public string currentName;
    
    public List<Sprite> waterSprites;

    public void SelectObject(GameObject objectForSelect)
    {
        if(objectForSelect != null) 
            gameplayWindow.SetTileName(currentName);
        if (objectForSelect == null)
            gameplayWindow.SetTileName("");
        OnObjectSelect?.Invoke(_selectedObject, objectForSelect);
        _selectedObject = objectForSelect;
    }

    public GameObject GetSelectedObject()
    {
        return _selectedObject;
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

    public void CheckWin()
    {
        var civs = transform.GetComponentsInChildren<CivilisationController>();
        if(civs.Length == 0)
            return;
        var winCiv = civs.First();
        if (civs.Length == 1)
        {
            if (winCiv.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
            {
                var a = WindowsManager.Instance.CreateWindow<GameOverWindow>("GameOverWindow");
                a.ShowWindow();
                a.OnTop();
            }
            else
            {
                var a = WindowsManager.Instance.CreateWindow<VictoryWindow>("VictoryWindow");
                a.ShowWindow();
                a.OnTop();
            }
        }
    }
}