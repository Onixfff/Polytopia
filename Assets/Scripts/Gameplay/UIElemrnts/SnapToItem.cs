using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnapToItem : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListItem;
    
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private float snapForce = 1f;

    private bool _isSnapped;

    private void Update()
    {
        var currentItem = Mathf.RoundToInt(0 - contentPanel.localPosition.x /
            (sampleListItem.rect.width + horizontalLayoutGroup.spacing));

        var snapSpeed = 0f;
        if (scrollRect.velocity.magnitude < 200 && !_isSnapped)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(contentPanel.localPosition.x, 
                    0 - currentItem * (sampleListItem.rect.width + horizontalLayoutGroup.spacing), 
                    snapSpeed),
                
                contentPanel.localPosition.y, 
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.x ==
                0 - currentItem * (sampleListItem.rect.width + horizontalLayoutGroup.spacing))
                _isSnapped = true;
        }
        else if(scrollRect.velocity.magnitude > 200)
        {
            _isSnapped = false;
        }
    }
}
