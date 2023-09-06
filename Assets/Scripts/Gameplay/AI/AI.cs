using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public int aiNumber;
    public TaskManager taskManager;
    
    
    private CivilisationController _controller;
    private Sequence _unitsSeq;
    private Sequence _unitsActionSeq;
    private List<UnitController> _allUnits;
    private List<int> _openedUnits;
    
    
    private void Start()
    {
        _controller = GetComponentInParent<CivilisationController>();
        _openedUnits ??= new List<int> { 0 };
        UpdateOpenUnitTech();
    }

    public void StartTurn()
    {
        UnitManagement();
        CheckAssignmentForMoney(_controller.Money, 0);
    }

    private void CheckAssignmentForMoney(int money, int count)
    {
        if(count > 10)
            return;
        foreach (var tile in _controller.GetTileInExploreList())
        {
            if (tile.unitOnTile != null)
            {
                _controller.relationOfCivilisation.AddNewCivilisation(tile.unitOnTile.GetOwner().owner, DiplomacyManager.RelationType.Neutral);
                tile.unitOnTile.GetOwner().owner.relationOfCivilisation.AddNewCivilisation(_controller, DiplomacyManager.RelationType.Neutral);
            }
        }
        
        if (money >= 20)
        {
            Building();
            if(count == 0)
                BuyingTech();
            BuyingUnits();
        }
        if (money >= 10)
        {
            Building();
            BuyingUnits();
        }
        if (money >= 5)
        {
            Building();
            BuyingUnits();
        }
        if(money > 8)
            CheckAssignmentForMoney(money, count + 1);
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
            BuildTileTech(home.GetControlledTiles());
        }

        void BuildTileTech(List<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                var intTypes = Check(tile.GetTileTypes(), tile);
                if(intTypes.Count == 0)
                    continue;
                intTypes.Sort();
                intTypes.Reverse();
                var rand = 0;
                if (intTypes[rand] >= 2)
                    rand = Random.Range(0, intTypes.Count);
                var money = _controller.Money;
                for (var i = 0; i <= 100; i++)
                {
                    tile.BuyTileTech(intTypes[rand], CivilisationInfo.ControlType.AI);
                    if (money == _controller.Money) 
                        rand = Random.Range(0, intTypes.Count);
                    else
                        break;
                }
                
                //Debug.Log(intTypes[rand]);
                return;
            }

            List<int> Check(List<GameplayWindow.OpenedTechType> types, Tile tileC)
            {
                var board = LevelManager.Instance.gameBoardWindow;
                var controller = tileC.GetOwner().owner;
                var ints = new List<int>();
                if (controller == null)
                    return ints;

                if (types.Contains(GameplayWindow.OpenedTechType.Monument))
                    return ints;

                if (types.Contains(GameplayWindow.OpenedTechType.Construct))
                {
                    if (controller.technologies.Contains(TechInfo.Technology.Construction))
                    {

                        ints.Add(1);
                    }

                    return ints;
                }

                if (types.Contains(GameplayWindow.OpenedTechType.Water))
                {
                    if (types.Contains(GameplayWindow.OpenedTechType.Fish))
                    {

                        ints.Add(1);
                    }

                    #region Monuments

                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.AltarOfPeace))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.EmperorTomb))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.EyeOfGod))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.GateOfPower))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.GrandBazaar))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.ParkOfFortune))

                        ints.Add(2);
                    if (tileC.GetOwner().owner.GetMonumentBuilder()
                        .IsMonumentAvailable(MonumentBuilder.MonumentType.TowerOfWisdom))

                        ints.Add(2);

                    #endregion

                    if (controller.technologies.Contains(TechInfo.Technology.Sailing))
                    {

                        ints.Add(8);
                    }

                    if (controller.technologies.Contains(TechInfo.Technology.Aqua))
                    {

                        ints.Add(1);
                    }

                    return ints;
                }

                if (types.Contains(GameplayWindow.OpenedTechType.DeepWater))
                {
                    if (controller.technologies.Contains(TechInfo.Technology.Whaling))
                    {

                        ints.Add(9);
                    }

                    return ints;
                }

                if (types.Contains(GameplayWindow.OpenedTechType.Ground))
                {
                    if (tileC.isHasMountain)
                    {
                        if (controller.technologies.Contains(TechInfo.Technology.Mining))
                        {

                            ints.Add(6);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Mining))
                        {

                            ints.Add(7);
                        }

                        return ints;
                    }
                    
                    if (controller.technologies.Contains(TechInfo.Technology.Roads))
                    {
                        if(!types.Contains(GameplayWindow.OpenedTechType.Road))
                            ints.Add(3);
                    }
                    
                    if (types.Contains(GameplayWindow.OpenedTechType.Animal))
                    {

                        ints.Add(2);
                    }

                    if (types.Contains(GameplayWindow.OpenedTechType.Fruit))
                    {

                        ints.Add(0);
                    }

                    if (types.TrueForAll(ty => ty == GameplayWindow.OpenedTechType.Ground))
                    {
                        /*#region Monuments


                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.AltarOfPeace))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.EmperorTomb))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.EyeOfGod))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.GateOfPower))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.GrandBazaar))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.ParkOfFortune))

                            ints.Add(2);
                        if (tileC.GetOwner().owner.GetMonumentBuilder()
                            .IsMonumentAvailable(MonumentBuilder.MonumentType.TowerOfWisdom))

                            ints.Add(2);

                        #endregion*/
                        
                        if (controller.technologies.Contains(TechInfo.Technology.FreeSpirit))
                        {

                            ints.Add(4);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Trade))
                        {
                            if (board.GetCloseTile(tileC, 1).Find(tile =>
                                    tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType ==
                                    BuildingUpgrade.BuildType.Port))

                                ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Construction))
                        {
                            if (board.GetCloseTile(tileC, 1).Find(tile =>
                                    tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType ==
                                    BuildingUpgrade.BuildType.Farm))

                                ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Forge))
                        {
                            if (board.GetCloseTile(tileC, 1).Find(tile =>
                                    tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType ==
                                    BuildingUpgrade.BuildType.Mine))

                                ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Spiritualism))
                        {

                            ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Mathematics))
                        {
                            if (board.GetCloseTile(tileC, 1).Find(tile =>
                                    tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType ==
                                    BuildingUpgrade.BuildType.LumberHut))

                                ints.Add(2);
                        }
                    }

                    if (types.Contains(GameplayWindow.OpenedTechType.Tree))
                    {
                        if (controller.technologies.Contains(TechInfo.Technology.Spiritualism))
                        {

                            ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Forestry))
                        {

                            ints.Add(1);

                            ints.Add(1);
                        }

                        if (controller.technologies.Contains(TechInfo.Technology.Chivalry))
                        {

                            ints.Add(1);
                        }

                    }

                    if (types.Contains(GameplayWindow.OpenedTechType.Crop))
                    {
                        if (controller.technologies.Contains(TechInfo.Technology.Farming))
                        {

                            ints.Add(5);
                        }
                    }
                }

                return ints;
            }
        }
    }

    private void BuyingTech()
    {
        AITechManager.Instance.TryBuyNeededTechnology(_controller);
        UpdateOpenUnitTech();
    }

    private void UpdateOpenUnitTech()
    {
        var technologies = _controller.technologies;
        foreach (var technology in technologies)
        {
            switch (technology)
            {
                case TechInfo.Technology.Rider:
                    if (!_openedUnits.Contains(3))
                        _openedUnits.Add(3);
                    break;
                case TechInfo.Technology.Strategy:
                    if (!_openedUnits.Contains(2))
                        _openedUnits.Add(2);
                    break;
                case TechInfo.Technology.Archery:
                    if (!_openedUnits.Contains(1))
                        _openedUnits.Add(1);
                    break;
                case TechInfo.Technology.Chivalry:
                    if (!_openedUnits.Contains(8))
                        _openedUnits.Add(8);
                    break;
                case TechInfo.Technology.Diplomacy:
                    if (!_openedUnits.Contains(4))
                        _openedUnits.Add(4);
                    break;
                case TechInfo.Technology.Forge:
                    if (!_openedUnits.Contains(6))
                        _openedUnits.Add(6);
                    break;
                case TechInfo.Technology.Philosophy:
                    if (!_openedUnits.Contains(5))
                        _openedUnits.Add(5);
                    break;
                case TechInfo.Technology.Mathematics:
                    if (!_openedUnits.Contains(7))
                        _openedUnits.Add(7);
                    break;
            }
        }

        
    }
    
    private void UnitManagement()
    {
        var homes = _controller.homes;
        homes.RemoveAll(home => home.owner != _controller);
        homes.Add(_controller.independentHome);
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
        var randUnitIndex = Random.Range(0, _openedUnits.Count);
        var randUnit = _openedUnits[randUnitIndex];

        if (_allUnits.Count == 0)
            randUnit = _openedUnits.Last();
        
        var homes = _controller.homes;
        homes.RemoveAll(home => home.owner != _controller);
        foreach (var home in homes)
        { 
            home.BuyUnit(randUnit, CivilisationInfo.ControlType.AI);
        }
    }
}
