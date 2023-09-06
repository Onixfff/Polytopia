using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public abstract class BaseTask : MonoBehaviour
{
    public enum TaskType
    {
        Exploring,
        Capture,
        Attack,
        Patrol,
        MoveToPoint
    }

    public TaskType taskType;
    public Action<BaseTask, List<UnitController>> OnUnitReturn;
    public Action OnTurnEnded;
    public int taskPriority = 2;
    
    protected List<UnitController> UnitsAssignedToTheTask;
    protected TaskManager TaskManager;
    protected Sequence TaskSeq;
    protected Sequence FuseSeq;

    public void AddTaskManager(TaskManager manager)
    {
        if(TaskManager == null)
            TaskManager = manager;
    }
    
    public int GetCountUnit()
    {
        return UnitsAssignedToTheTask.Count;
    }

    public bool UnitIsOnTask(UnitController unit, string taskName)
    {
        if (UnitsAssignedToTheTask == null)
        {
            return false;
        }

        if (unit.aiTaskName == taskName)
            return true;
        
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

        EndTask();
        //EndTurn();
    }
    
    protected virtual void EndTurn()
    { 
        Debug.Log("Task: " + name + " continue.");
        OnTurnEnded?.Invoke();
    }
    
    protected virtual void EndTask()
    {
        if (UnitsAssignedToTheTask != null)
        {
            OnUnitReturn?.Invoke(this, UnitsAssignedToTheTask);
            UnitsAssignedToTheTask.Clear();
        }
        OnTurnEnded?.Invoke();
        Debug.Log("Task: " + name + " ended.");
    }

    protected void TryAddPointOfInteresting(Vector2Int pos)
    {
        TaskManager.TryAddPointOfInteresting(pos);
    }
    
    protected void TryRemovePointOfInteresting(Vector2Int pos)
    {
        TaskManager.TryRemovePointOfInteresting(pos);
    }
    
    public abstract int CalculatePriority(List<UnitController> units);
    
    protected abstract void TaskRealisation();

    protected abstract void UnitAction(List<UnitController> units, int i);
}