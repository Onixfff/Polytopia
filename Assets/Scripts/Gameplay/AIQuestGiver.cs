using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        var allUnits = _allUnits;
        foreach (var task in tasks)
        {
            if(task.TaskPriority is 0 or -1)
                continue;
            
            var units = ChooseTheRightUnit(task.taskType, allUnits.Keys.ToList());

            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (allUnits[unit])
                    continue;
                task.OnUnitReturn += ReturnUnitsToList;
                task.AddUnitToTask(unit);
                RemoveUnitFromList(unit);
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

    private List<UnitController> ChooseTheRightUnit(BaseTask.TaskType taskType, List<UnitController> units)
    {
        var board = LevelManager.Instance.gameBoardWindow;
        var rightUnits = new List<UnitController>();
        switch (taskType)
        {
            case BaseTask.TaskType.Capture:
                foreach (var unit in units)
                {
                    if (unit.occupiedTile.homeOnTile != null && unit.occupiedTile.homeOnTile.owner != unit.GetOwner().owner)
                    {
                        rightUnits.Add(unit);
                        continue;
                    }
                    var closeTile = board.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().moveRad);
                    if (closeTile.Any(tile => tile.homeOnTile != null && tile.homeOnTile.owner != unit.GetOwner().owner))
                    {
                        rightUnits.Add(unit);
                        continue;
                    }
                }
                break;
            case BaseTask.TaskType.Exploring:
                rightUnits.AddRange(units);
                break;
        }

        return rightUnits;
    }
}
