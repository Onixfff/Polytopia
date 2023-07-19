using System;
using UnityEngine;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public Action<GameObject, GameObject> OnObjectSelect;

    public GameBoardWindow gameBoardWindow;
    private GameObject _selectedObject;

    public void SelectObject(GameObject objectForSelect)
    {
        if(_selectedObject != null && objectForSelect != null) 
            Debug.Log(_selectedObject.name + " - " + objectForSelect.name);
        OnObjectSelect?.Invoke(_selectedObject, objectForSelect);
        _selectedObject = objectForSelect;
    }
}