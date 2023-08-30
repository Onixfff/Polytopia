using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour
{
    [SerializeField] private RectTransform button;
    [SerializeField] private RectTransform tmPro;
    [SerializeField] private Image image;

    
    
    public void ChangeColor(DiplomacyManager.RelationType type)
    {
        switch (type)
        {
            case DiplomacyManager.RelationType.War:
                image.color = Color.red;
                break;
            case DiplomacyManager.RelationType.Neutral:
                image.color = Color.yellow;
                break;
            case DiplomacyManager.RelationType.Peace:
                image.color = Color.green;
                break;
        }
    }
    
    [Button()]
    public void AutoSize()
    {
        AutoSizeText();
    }

    private void AutoSizeText()
    {
        var proUGUI = tmPro.GetComponent<TextMeshProUGUI>();
        proUGUI.autoSizeTextContainer = false;
        proUGUI.autoSizeTextContainer = true;
    }

    public void AddOffsetByX(float offset)
    {
        var anchoredPosition = button.anchoredPosition;
        if(anchoredPosition.x > 0)
            anchoredPosition = new Vector2(50 + offset, anchoredPosition.y);
        if(anchoredPosition.x < 0)
            anchoredPosition = new Vector2(-50 - offset, anchoredPosition.y);

        button.anchoredPosition = anchoredPosition;
    }
    
    public void SetPosByX(float pos)
    {
        var anchoredPosition = button.anchoredPosition; 
        anchoredPosition = new Vector2(pos, anchoredPosition.y);
        button.anchoredPosition = anchoredPosition;
    }
}
