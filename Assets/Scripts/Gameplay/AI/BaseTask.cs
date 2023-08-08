using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTask : MonoBehaviour
{
    public enum TaskType
    {
        Exploring,
        Capture,
        Attack,
        SendTroops,
        Patrol
    }

    public TaskType taskType;
    public Action<BaseTask, List<UnitController>> OnUnitReturn;
    public Action OnTurnEnded;
    public int taskPriority = 2;
    
    protected List<UnitController> UnitsAssignedToTheTask;

    public int GetCountUnit()
    {
        return UnitsAssignedToTheTask.Count;
    }

    public bool UnitIsOnTask(UnitController unit)
    {
        if (UnitsAssignedToTheTask == null)
        {
            return false;
        }
        return UnitsAssignedToTheTask.Contains(unit);
    }
    
    public virtual void AddUnitToTask(UnitController unit)
    {
        UnitsAssignedToTheTask ??= new List<UnitController>();
        UnitsAssignedToTheTask.Add(unit);
    }
    
    public virtual void StartTask()
    {
        Debug.Log("Task: " + name);
        if (UnitsAssignedToTheTask != null)
        {
            UnitsAssignedToTheTask.RemoveAll(unit => unit == null);
            if (UnitsAssignedToTheTask.Count != 0)
            {
                Debug.Log("Task: " + name + " starter." + " Count units - " + UnitsAssignedToTheTask.Count);
                TaskRealisation();
                return;
            }
        }

        EndTurn();
    }
    
    protected virtual void EndTurn()
    { 
        OnTurnEnded?.Invoke();
    }
    
    protected virtual void EndTask()
    {
        if (UnitsAssignedToTheTask != null)
        {
            OnUnitReturn?.Invoke(this, UnitsAssignedToTheTask);
            OnTurnEnded?.Invoke();
            UnitsAssignedToTheTask.Clear();
        }
    }
    
    public abstract int CalculatePriority(List<UnitController> units);
    
    protected abstract void TaskRealisation();

    protected abstract void UnitAction(List<UnitController> units, int i);
}