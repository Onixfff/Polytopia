using System;
using System.Collections.Generic;
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
    private GameObject _selectedObject;
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
}