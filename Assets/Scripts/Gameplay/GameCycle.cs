using DG.Tweening;
using UnityEngine;

public class GameCycle : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance.OnTurnBegin += TurnBegin;
        LevelManager.Instance.OnTurnEnd += TurnEnd;
    }
    
    private void TurnBegin()
    {
        LevelManager.Instance.currentTurn++;
    }
    
    private void TurnEnd()
    {
        LevelManager.Instance.SelectObject(null);
    }
}

