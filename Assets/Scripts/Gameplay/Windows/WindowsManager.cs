using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowsManager : SingletonPersistent<WindowsManager>
{
    [SerializeField] private List<Transform> initWindows;
    
    private List<BaseWindow> _createdWindows;
    private Canvas _canvas;

    public override void Awake()
    {
        base.Awake();
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _canvas.worldCamera = Camera.main;
        CreateWindow<MenuWindow>("MenuWindow");

    }

    public T CreateWindow<T>(string windowName) where T : BaseWindow
    {
        var createdWindow = SearchCreatedWindow<T>(windowName);
        
        if (createdWindow == null)
        {
            var initWindow = SearchInitWindow<T>(windowName);
            if(initWindow == null) Debug.LogError("Window not found");
            createdWindow = Instantiate(initWindow, _canvas.transform);
            createdWindow.OpenWindow();
            _createdWindows.Add(createdWindow);
        }

        return createdWindow;
    }

    public void CloseAllWindows()
    {
        if(_createdWindows == null) return;
        
        foreach (var window in _createdWindows)
        {
            if (window.TryGetComponent(out BaseWindow windowController))
            {
                windowController.CloseWindow();
            }
        }
    }
    
    public void HideAllWindows()
    {
        if(_createdWindows == null) return;
        
        foreach (var window in _createdWindows)
        {
            if (window.TryGetComponent(out BaseWindow windowController))
            {
                windowController.HideWindow();
            }
        }
    }
    
    public T SearchInitWindow<T>(string windowName) where T : BaseWindow
    {
        T targetWindow = null;

        foreach (var initWindow in initWindows)
        {
            if (!initWindow.TryGetComponent(out T windowComponent)) continue;
            var createdWindowName = initWindow.name;
            createdWindowName = createdWindowName.Replace("(Clone)", "");
            if (createdWindowName == windowName)
            {
                targetWindow = windowComponent;
                break;
            }
        }

        return targetWindow;
    }
    
    public T SearchCreatedWindow<T>(string windowName) where T : BaseWindow
    {
        T targetWindow = null;
        _createdWindows ??= new List<BaseWindow>();
        foreach (var createdWindow in _createdWindows)
        {
            var createdWindowName = createdWindow.name;
            createdWindowName = createdWindowName.Replace("(Clone)", "");
            if (createdWindowName == windowName)
            {
                targetWindow = (T)createdWindow;
                break;
            }
        }

        return targetWindow;
    }
}