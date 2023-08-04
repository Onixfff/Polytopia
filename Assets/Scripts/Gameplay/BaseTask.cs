using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTask : MonoBehaviour
{
    public enum TaskType
    {
        Exploring,
        Capture
    }

    public TaskType taskType;
    public Action<BaseTask, List<UnitController>> OnUnitReturn;
    public Action OnTurnEnded;
    public int TaskPriority = 1;
    
    protected List<UnitController> UnitsAssignedToTheTask;

    public virtual int CalculatePriority(List<UnitController> units)
    {
        return TaskPriority;
    }

    public virtual void AddUnitToTask(UnitController unit)
    {
        UnitsAssignedToTheTask ??= new List<UnitController>();
        UnitsAssignedToTheTask.Add(unit);
    }
    
    public virtual void StartTask()
    {
        if (UnitsAssignedToTheTask != null && UnitsAssignedToTheTask.Count != 0)
        {
            Debug.Log("Task: " + name + " starter." + " Count units - " + UnitsAssignedToTheTask.Count);
            TaskRealisation();
        }
        else
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
    
    protected abstract void TaskRealisation();

    protected abstract void UnitAction(List<UnitController> units, int i);
}