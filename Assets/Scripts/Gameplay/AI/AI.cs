using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int aiNumber;
    public TaskManager taskManager;

    private CivilisationController _controller;
    private Sequence _unitsSeq;
    private Sequence _unitsActionSeq;
    private List<UnitController> _allUnits;
    
    private void Start()
    {
        _controller = GetComponentInParent<CivilisationController>();
    }

    public void StartTurn()
    {
        foreach (var tile in _controller.GetTileInExploreList())
        {
            if (tile.unitOnTile != null)
            {
                _controller.relationOfCivilisation.AddNewCivilisation(tile.unitOnTile.GetOwner().owner, DiplomacyManager.RelationType.Neutral);
                tile.unitOnTile.GetOwner().owner.relationOfCivilisation.AddNewCivilisation(_controller, DiplomacyManager.RelationType.Neutral);
            }
        }
        
        Building();
        BuyingTech();
        UnitManagement();
        BuyingUnits();
    }

    private void EndTurn()
    {
        taskManager.OnTaskAreDistributed -= EndTurn;
        AIController.Instance.OnAITurnEnded?.Invoke(aiNumber+1);
    }

    private void Building()
    {
        var homes = _controller.homes;
        foreach (var home in homes)
        {
            
        }
    }
    
    private void BuyingTech()
    {
        AITechManager.Instance.TryBuyNeededTechnology(_controller);
    }
    
    private void UnitManagement()
    {
        var homes = _controller.homes;
        homes.RemoveAll(home => home.owner != _controller);
        _allUnits ??= new List<UnitController>();
        foreach (var home in homes)
        {
            foreach (var unit in home.GetUnitList())
            {
                if(!_allUnits.Contains(unit) && unit.GetOwner().owner == _controller)
                    _allUnits.Add(unit);
            }
        }

        _allUnits.RemoveAll(unit => unit == null);
        if (_allUnits.Count != 0) 
            UnitAction(_allUnits);
        else
        {
            var val = 0;
            DOTween.To(() => val, x => x = val, 1, 0.01f).OnComplete(EndTurn);
        }
    }
    
    private void UnitAction(List<UnitController> units)
    {
        taskManager.AddUnitsToList(units);
        taskManager.AssignTasks(_controller);
        taskManager.OnTaskAreDistributed += EndTurn;
    }
    
    private void BuyingUnits()
    {
        var homes = _controller.homes;
        homes.RemoveAll(home => home.owner != _controller);
        foreach (var home in homes)
        {
            if(LevelManager.Instance.currentTurn % 2 == 0)
                home.BuyUnit(0);
        }
    }
}
