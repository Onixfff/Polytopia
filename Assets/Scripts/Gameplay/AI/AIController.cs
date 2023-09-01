using System;
using System.Collections.Generic;

public class AIController : Singleton<AIController>
{
    public Action<int> OnAITurnEnded;
    public int countAi;
    public AI aiPrefab;
    private List<AI> _aiList;

    public void AddAI(AI ai)
    {
        _aiList ??= new List<AI>();
        _aiList.Add(ai);
    }
    
    private void Start()
    {
        LevelManager.Instance.OnTurnEnd += TurnEnd;
        OnAITurnEnded += AIStartTurn;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnTurnEnd -= TurnEnd;
        OnAITurnEnded -= AIStartTurn;
    }

    private void TurnEnd()
    {
        OnAITurnEnded?.Invoke(0);
    }

    private void AIStartTurn(int i)
    {
        if (i >= countAi)
        {
            LevelManager.Instance.OnTurnBegin?.Invoke();
            return;
        }
        _aiList[i].StartTurn();
    }

    public void ClearAI()
    {
        _aiList.Clear();
    }
}