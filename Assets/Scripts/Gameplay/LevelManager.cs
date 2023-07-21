using System;
using UnityEngine;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public Action<GameObject, GameObject> OnObjectSelect;

    public GameBoardWindow gameBoardWindow;
    public GameplayWindow gameplayWindow;
    public string currentName;
    private GameObject _selectedObject;

    public void SelectObject(GameObject objectForSelect)
    {
        if(objectForSelect != null) 
            gameplayWindow.SetTileName(currentName);
        if (objectForSelect == null)
            gameplayWindow.SetTileName("");
        OnObjectSelect?.Invoke(_selectedObject, objectForSelect);
        _selectedObject = objectForSelect;
    }
}