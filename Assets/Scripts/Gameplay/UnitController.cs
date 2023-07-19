using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    [SerializeField] private Button selectUnit;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float moveDuration = 2f;
    private GameObject _occupiedTile;
    private bool _isSelected;
    void Start()
    {
        unitImage.SetNativeSize();
        selectUnit.onClick.AddListener(SelectUnit);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (pastO == gameObject)
        {
            if (currO.TryGetComponent(out Tile tile))
            {
                MoveToTile(tile.transform);
            }
            DeselectUnit();
        }
    }

    private void MoveToTile(Transform to)
    {
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetParent(to.parent);
        transform.SetSiblingIndex(transform.parent.childCount);
        var inValX = rectTransform.anchoredPosition.x;
        DOTween.To(() => inValX, x => inValX = x, anchorPos.x, moveDuration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(inValX, rectTransform.anchoredPosition.y);
        }));
        var inValY = rectTransform.anchoredPosition.y;
        DOTween.To(() => inValY, x => inValY = x, anchorPos.y + 30, moveDuration).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, inValY);
        }));
    }
    
    private void SelectUnit()
    {
        if (_isSelected)
        {
            SelectOccupiedTile();
            return;
        }
        
        _isSelected = true;
        LevelManager.Instance.SelectObject(gameObject);
        LevelManager.Instance.gameBoardWindow.GetAllTile();
    }

    private void DeselectUnit()
    {
        _isSelected = false;
    }

    private void SelectOccupiedTile()
    {
        Debug.Log("tileInfo");
    }
}
