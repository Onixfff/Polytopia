using DG.Tweening;
using UnityEngine;

public class GameCycle : MonoBehaviour
{
    private int currentIncome => LevelManager.Instance.currentIncome;
    
    private void Start()
    {
        LevelManager.Instance.OnTurnBegin += TurnBegin;
        LevelManager.Instance.OnTurnEnd += TurnEnd;
    }
    
    private void TurnBegin()
    {
        Debug.Log("TurnBegin");

        EconomicManager.Instance.AddMoney(currentIncome);
        LevelManager.Instance.currentTurn++;
    }
    
    private void TurnEnd()
    {
        Debug.Log("TurnEnd");

        LevelManager.Instance.SelectObject(null);
        var inVal = 0;
        DOTween.To(() => inVal, x => x = inVal, 5f, 1f).OnComplete(() =>
        {
            LevelManager.Instance.OnTurnBegin?.Invoke();
        });
    }
}

