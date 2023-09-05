using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : Singleton<BoardController>
{
    [SerializeField] private RectTransform boardRectTransform;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float duration = 1f;
    private Sequence _moveSeq;
    
    public void TryMoveBoardTo(RectTransform rectTransform)
    {
        if (!rectTransform.IsFullyVisibleFrom(Camera.main))
        {
            if(_moveSeq != null)
                _moveSeq.Kill();
            _moveSeq = DOTween.Sequence();
            
            var inVal = boardRectTransform.anchoredPosition.x;
            scrollRect.velocity = Vector2.zero;

            _moveSeq.Join(DOTween.To(() => inVal, x => inVal = x, -rectTransform.anchoredPosition.x, duration).OnUpdate((() =>
            {
                if (rectTransform.IsFullyVisibleFrom(Camera.main))
                {
                    return;
                }
                boardRectTransform.anchoredPosition = new Vector2(inVal, boardRectTransform.anchoredPosition.y);
            })));
            var inVal1 = boardRectTransform.anchoredPosition.y;
            _moveSeq.Join(DOTween.To(() => inVal1, x => inVal1 = x, -rectTransform.anchoredPosition.y, duration).OnUpdate((() =>
            {
                if (rectTransform.IsFullyVisibleFrom(Camera.main))
                {
                    return;
                }
                boardRectTransform.anchoredPosition = new Vector2(boardRectTransform.anchoredPosition.x, inVal1);
            })));
        }
    }
}
