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
        EconomicManager.Instance.AddMoney(currentIncome);
        LevelManager.Instance.currentTurn++;
    }
    
    private void TurnEnd()
    {
        LevelManager.Instance.SelectObject(null);
    }
}

