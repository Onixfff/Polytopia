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
        _allUnits.Keys.ToList().RemoveAll(unit => unit == null);

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
        tasks = allTasks.OrderBy(or => or.taskPriority).ToList();
        tasks.Reverse();
        var allUnits = _allUnits;
        foreach (var task in tasks)
        {
            allUnits.Keys.ToList().RemoveAll(key => allUnits[key]);
            var units = ChooseTheRightUnit(task, allUnits.Keys.ToList());

            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if(unit == null)
                    continue;
                if (allUnits[unit])
                    continue;
                task.OnUnitReturn += ReturnUnitsToList;
                task.AddUnitToTask(unit);
                RemoveUnitFromList(unit);
                unit.aiTaskName = task.taskType.ToString();
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

    private List<UnitController> ChooseTheRightUnit(BaseTask task, List<UnitController> units)
    {
        var taskType = task.taskType;
        var board = LevelManager.Instance.gameBoardWindow;
        var rightUnits = new List<UnitController>();
        switch (taskType)
        {
            case BaseTask.TaskType.Patrol:
                var homes = new List<Home>();
                foreach (var unit in units)
                {
                    if(homes.Contains(unit.GetOwner()))
                        continue;
                    var homeUnits = unit.GetOwner().GetUnitList();
                    homes.Add(unit.GetOwner());
                    if (homeUnits.Find(task.UnitIsOnTask))
                        continue;
                    var unit1 = units.Find(unit2 => unit2.occupiedTile.homeOnTile != null && unit2.occupiedTile.homeOnTile == unit2.GetOwner()); 
                    rightUnits.Add(unit1);
                }
                
                break;
            case BaseTask.TaskType.SendTroops:
                rightUnits.AddRange(units);
                /*foreach (var unit in units)
                {
                    var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, Mathf.Max(unit.GetUnitInfo().rad, unit.GetUnitInfo().moveRad));
                    if (tiles.Any(tile => tile.unitOnTile == null || tile.homeOnTile == null))
                    {
                        rightUnits.Add(unit);
                    }
                }
                break;*/
                break;
            case BaseTask.TaskType.Attack:
                foreach (var unit in units)
                {
                    var owner = unit.GetOwner().owner;
            
                    var tiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, unit.GetUnitInfo().rad);
                    if (tiles.Any(tile => tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner != owner))
                    {
                        rightUnits.Add(unit);
                    }
                }
                break;
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
