using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIQuestGiver : MonoBehaviour
{
    public Action OnTaskAreDistributed;
    
    [SerializeField] private List<BaseTask> allTasks;
    private Dictionary<UnitController, bool> _allUnits;

    public void AddUnitsToList(List<UnitController> units)
    {
        _allUnits ??= new Dictionary<UnitController, bool>();
        foreach (var unit in units)
        {
            if(_allUnits.ContainsKey(unit))
                continue;
            _allUnits.Add(unit, false);
        }
    }
    
    public void AssignTasks()
    {
        AssignUnits();
        StartTasksExecution(0);
    }

    private void AssignUnits()
    {
        var tasks = new List<BaseTask>();

        foreach (var task in allTasks)
        {
            task.CalculatePriority(_allUnits.Keys.ToList());
        }
        tasks = allTasks.OrderBy(or => or.TaskPriority).ToList();
        tasks.Reverse();
        foreach (var task in tasks)
        {
            Debug.Log(task.name + " " + task.TaskPriority);
            if(task.TaskPriority is 0 or -1)
                continue;
            
            if (task.TaskPriority == 2)
            {
                for (var i = 0; i < 2; i++)
                {
                    if(_allUnits.Count <= i)
                        continue;
                    
                    var unit = _allUnits.Keys.ToList()[i];
                    if(_allUnits[unit])
                        continue;
                    task.OnUnitReturn += ReturnUnitsToList;
                    task.AddUnitToTask(unit);
                    RemoveUnitFromList(unit);
                }
            }
            if(task.TaskPriority == 1)
            {
                for (var i = 0; i < _allUnits.Count; i++)
                {
                    if(_allUnits.Count <= i)
                        continue;
                    var unit = _allUnits.Keys.ToList()[i];
                    if(_allUnits[unit])
                        continue;
                    task.OnUnitReturn += ReturnUnitsToList;
                    task.AddUnitToTask(unit);
                    RemoveUnitFromList(unit);
                }
            }
        }
    }

    private void StartTasksExecution(int i)
    {
        if (allTasks.Count <= i)
        {
            OnTaskAreDistributed?.Invoke();
            return;
        }
        allTasks[i].OnTurnEnded += RecursionActionStart;

        allTasks[i].StartTask();
        
        void RecursionActionStart()
        {
            allTasks[i].OnTurnEnded -= RecursionActionStart;
            StartTasksExecution(i+1);
        }
    }

    private void ReturnUnitsToList(BaseTask task, List<UnitController> units)
    {
        task.OnUnitReturn -= ReturnUnitsToList;
        foreach (var unit in units)
        {
            _allUnits[unit] = false;
        }
    }

    private void RemoveUnitFromList(UnitController unit)
    {
        _allUnits[unit] = true;
    }
}
