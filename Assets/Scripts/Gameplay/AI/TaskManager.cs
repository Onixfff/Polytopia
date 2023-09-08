using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public Action OnTaskAreDistributed;
    public List<Vector2Int> pointsOfInteresting;

    [SerializeField] private List<BaseTask> allTasks;
    private Dictionary<UnitController, bool> _allUnits;
    public CivilisationController civilisationController;

    public void TryAddPointOfInteresting(Vector2Int pos)
    {
        var boardWindow = LevelManager.Instance.gameBoardWindow;
        foreach (var tile in boardWindow.GetCloseTile(boardWindow.GetTile(pos), 2))
        {
            if (pointsOfInteresting.Contains(tile.pos))
            {
                return;
            }
        }

        if (!pointsOfInteresting.Contains(pos))
        {
            //Debug.Log("added new point - " + pos);
            pointsOfInteresting.Add(pos);
        }
    }
    
    public void TryRemovePointOfInteresting(Vector2Int pos)
    {
        if (pointsOfInteresting.Contains(pos))
        {
            //Debug.Log("removed point - " + pos);
            pointsOfInteresting.Remove(pos);
        }
    }
    
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
    
    public void AssignTasks(CivilisationController controller)
    {
        if (civilisationController == null)
            civilisationController = controller;
        AssignUnits();
        StartTasksExecution(0);
    }

    private void AssignUnits()
    {
        pointsOfInteresting ??= new List<Vector2Int>();

        foreach (var task in allTasks)
        {
            task.AddTaskManager(this);
            task.CalculatePriority(_allUnits.Keys.ToList());
        }
        var tasks = allTasks.OrderBy(or => or.taskPriority).ToList();
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
                    if (unit.occupiedTile.GetHomeOnTile() != null && unit.occupiedTile.GetHomeOnTile().owner != unit.GetOwner().owner)
                    {
                        rightUnits.Add(unit);
                        continue;
                    }
                    var closeTile = board.GetCloseTile(unit.occupiedTile, 1);
                    closeTile.RemoveAll(tile => !tile.IsTileFree());
                    if (closeTile.Any(tile => tile.GetHomeOnTile() != null && tile.GetHomeOnTile().owner != unit.GetOwner().owner))
                    {
                        rightUnits.Add(unit);
                    }
                }
                break;
            case BaseTask.TaskType.Exploring:
                rightUnits.AddRange(units.Where(unit => board.GetCloseTile(unit.occupiedTile, 3).Any(tile => civilisationController.GetTileInExploreList().Contains(tile))));
                break;
        }

        return rightUnits;
    }
}
