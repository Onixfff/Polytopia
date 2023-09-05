using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : Singleton<BoardController>
{

    [SerializeField] private RectTransform boardRectTransform;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float duration = 1f;
    private Sequence _moveSeq;

    public void TryMoveTo(RectTransform rectTransform)
    {
        if (!rectTransform.IsFullyVisibleFrom(Camera.main))
        {
            if (_moveSeq != null)
                _moveSeq.Kill();
            _moveSeq = DOTween.Sequence();

            scrollRect.velocity = Vector2.zero;

            var inVal = boardRectTransform.anchoredPosition.x;
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

    public void MoveToCenter(RectTransform rectTransform)
    {
        _moveSeq = DOTween.Sequence();
        scrollRect.velocity = Vector2.zero;

        var inVal = boardRectTransform.anchoredPosition.x;
        _moveSeq.Join(DOTween.To(() => inVal, x => inVal = x, -rectTransform.anchoredPosition.x, duration).OnUpdate((() =>
        {
            boardRectTransform.anchoredPosition = new Vector2(inVal, boardRectTransform.anchoredPosition.y);
        })));
        var inVal1 = boardRectTransform.anchoredPosition.y;
        _moveSeq.Join(DOTween.To(() => inVal1, x => inVal1 = x, -rectTransform.anchoredPosition.y, duration).OnUpdate((() =>
        {
            boardRectTransform.anchoredPosition = new Vector2(boardRectTransform.anchoredPosition.x, inVal1);
        })));
    }

    private void MoveTo(RectTransform rectTransform)
    {
        
    }
}
