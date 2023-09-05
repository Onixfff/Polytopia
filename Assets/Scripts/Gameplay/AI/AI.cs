using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
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
            BuyingTech();
            BuyingUnits(0);
        }
        else
        if (money >= 10)
        {
            Building();
            BuyingUnits(0);
        }
        else
        if (money >= 5)
        {
            Building();
            BuyingUnits(0);
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
                var rand = 0;
                if (intTypes[rand] > 2)
                    rand = Random.Range(0, intTypes.Count);
                //Debug.Log(intTypes[rand]);
                tile.BuyTileTech(intTypes[rand], CivilisationInfo.ControlType.AI);
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
    
    private void BuyingUnits(int index)
    {
        var homes = _controller.homes;
        homes.RemoveAll(home => home.owner != _controller);
        foreach (var home in homes)
        { 
            home.BuyUnit(index, CivilisationInfo.ControlType.AI);
        }
    }
}
