using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTask : MonoBehaviour
{
    public Action<BaseTask, List<UnitController>> OnTaskEnded;
    public int TaskPriority = 1;
    
    protected List<UnitController> UnitsAssignedToTheTask;
    
    public virtual int CalculatePriority()
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
        if(UnitsAssignedToTheTask != null && UnitsAssignedToTheTask.Count != 0)
            TaskRealisation();
    }
    
    protected virtual void EndTask()
    {
        if (UnitsAssignedToTheTask != null) 
            OnTaskEnded?.Invoke(this, UnitsAssignedToTheTask);
    }
    
    protected abstract void TaskRealisation();

    protected abstract void UnitAction(List<UnitController> units, int i);
}