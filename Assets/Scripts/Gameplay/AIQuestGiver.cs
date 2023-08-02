using System;
using System.Collections.Generic;
using UnityEngine;

public class AIQuestGiver : MonoBehaviour
{
    public Action OnTaskAreDistributed;
    
    [SerializeField] private List<BaseTask> allTasks;
    private List<UnitController> _allUnits;

    public void AddUnitsToList(List<UnitController> units)
    {
        _allUnits ??= new List<UnitController>();
        foreach (var unit in units)
        {
            if(_allUnits.Contains(unit))
                continue;
            _allUnits.Add(unit);
        }        
    }
    
    public void AssignTasks()
    {
        AssignUnits();
        StartTasksExecution();
    }

    private void AssignUnits()
    {
        var tasks = allTasks;
        for (var i = _allUnits.Count - 1; i >= 0; i--)
        {
            var unit = _allUnits[i];
            var maxPriority = 0;
            BaseTask maxTask = null;

            foreach (var task in tasks)
            {
                if (task.TaskPriority >= maxPriority)
                {
                    maxPriority = task.CalculatePriority();
                    maxTask = task;
                }
            }

            if (maxTask != null)
            {
                maxTask.OnTaskEnded += ReturnUnitsToList;
                maxTask.AddUnitToTask(unit);
                RemoveUnitFromList(unit);
            }
        }
    }

    private void StartTasksExecution()
    {
        foreach (var task in allTasks)
        {
            task.StartTask();
        }
    }

    private void ReturnUnitsToList(BaseTask task, List<UnitController> units)
    {
        task.OnTaskEnded -= ReturnUnitsToList;
        _allUnits.AddRange(units);
    }

    private void RemoveUnitFromList(UnitController unit)
    {
        _allUnits.Remove(unit);
    }
}
