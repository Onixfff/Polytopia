using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseWindow : MonoBehaviour
{
    public bool IsVisible { get; private set; }

    protected CanvasGroup _canvasGroup;

    public void OpenWindow()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        IsVisible = true; 
    }
    
    public void ShowWindow(bool withAnimation = true)
    {
        if (IsVisible) return;

        IsVisible = true;

        _canvasGroup.alpha = 1;
        SetInteractable(true);
    }

    public void HideWindow(bool withAnimation = true)
    {
        if (IsVisible == false) return;

        IsVisible = false;

        _canvasGroup.alpha = 0;
        SetInteractable(false);
    }

    public void CloseWindow()
    {
        Destroy(gameObject);
    }

    public void SetInteractable(bool state)
    {
        _canvasGroup.blocksRaycasts = state;
    }
    
    public void OnTop()
    {
        transform.SetSiblingIndex(transform.parent.childCount);
    }

    private void AddToWindowManager()
    {
    }
}
