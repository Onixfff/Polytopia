using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : BaseWindow
{
    public Action<int> OnUnitSpawn;
    public Action<int> OnTileTech;

    public enum OpenedTechType
    {
        Fruit,
        Animal,
        Tree,
        Fish,
        Whale,
        Ground,
        Water,
        Construct,
        Crop,
        DeepWater,
        Monument
    }
    
    [SerializeField] private TextMeshProUGUI currentTurnUGUI;
    [SerializeField] private TextMeshProUGUI currentMoneyUGUI;
    [SerializeField] private TextMeshProUGUI currentPointUGUI;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button technologyOpenButton;
    [SerializeField] private Button technologyCloseButton;
    [SerializeField] private Button turnEndButton;
    [SerializeField] private GameObject technologyObject;
    [SerializeField] private GameObject inputBlockObject;
    
    [SerializeField] private GameObject turnEnd;
    [SerializeField] private GameObject turnBegin;
    
    [SerializeField] private GameObject downBar;
    [SerializeField] private Button downBarBackButton;
    [SerializeField] private List<Button> unitTechButtons;
    [SerializeField] private GameObject unitTechButtonParent;
    [SerializeField] private List<Button> tileTechButtons;
    [SerializeField] private GameObject unitButtonParent;
    [SerializeField] private List<Button> unitButtons;
    [SerializeField] private GameObject tileTechButtonParent;
    [SerializeField] private TextMeshProUGUI tileNameUGUI;
    private List<Button> _openedTileTechButtons;
    private Tween _tween;
    private CivilisationController _playerCiv;

    public void SetPlayerCiv(CivilisationController civ)
    {
        _playerCiv = civ;
        _playerCiv.OnMoneyChanged += MoneyChanged;
    }
    
    public void UnlockUnitTech(int unitIndex)
    {
        unitTechButtons[unitIndex].gameObject.SetActive(true);
        unitTechButtons[unitIndex].onClick.AddListener((() =>
        {
            BuyUnit(unitIndex);
        }));
    }
    
    public void UnlockTileTech(int techIndex)
    {
        _openedTileTechButtons ??= new List<Button>
        {
            null, null, null, null, null, 
            null, null, null, null, null, 
            null, null, null, null, null, 
            null, null, null, null, null,
            null, null, null, null, null, 
            null, null, null
        };
        _openedTileTechButtons.RemoveAt(techIndex);
        _openedTileTechButtons.Insert(techIndex, tileTechButtons[techIndex]);
        tileTechButtons[techIndex].gameObject.SetActive(true);
        tileTechButtons[techIndex].onClick.AddListener((() =>
        {
            BuyTileTech(techIndex);
        }));
    }
    
    public void SetTileName(string tileName)
    {
        downBar.SetActive(true);
        tileNameUGUI.gameObject.SetActive(true);
        tileNameUGUI.text = tileName;
    }
    
    public void ShowHomeButton()
    {
        downBar.SetActive(true);
        unitTechButtonParent.SetActive(true);
        tileNameUGUI.text = "Home";
    }
    
    public void ShowTileButton(List<OpenedTechType> types, Tile tileC)
    {
        if(_openedTileTechButtons == null) return;
        downBar.SetActive(true);
        tileTechButtonParent.SetActive(true);
        var board = LevelManager.Instance.gameBoardWindow;
        var controller = tileC.GetOwner().owner;
        if(controller == null)
            return;
        foreach (var tile in tileTechButtons)
        {
            tile.gameObject.SetActive(false);            
        }
        if (types.Contains(OpenedTechType.Monument))
            return;
        
        if (types.Contains(OpenedTechType.Construct))
        {
            if (controller.technologies.Contains(TechInfo.Technology.Construction))
            {
                if (_openedTileTechButtons[15] != null)
                    _openedTileTechButtons[15].gameObject.SetActive(true);
            }
            return;
        }
        if(types.Contains(OpenedTechType.Water))
        {
            #region Monuments

            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.AltarOfPeace))
               if (_openedTileTechButtons[21] != null)
                    _openedTileTechButtons[21].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.EmperorTomb))
                if (_openedTileTechButtons[22] != null)
                    _openedTileTechButtons[22].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.EyeOfGod))
                if (_openedTileTechButtons[23] != null)
                    _openedTileTechButtons[23].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.GateOfPower))
                if (_openedTileTechButtons[24] != null)
                    _openedTileTechButtons[24].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.GrandBazaar))
                if (_openedTileTechButtons[25] != null)
                  _openedTileTechButtons[25].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.ParkOfFortune))
                if (_openedTileTechButtons[26] != null)
                    _openedTileTechButtons[26].gameObject.SetActive(true);
            if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.TowerOfWisdom))
                if (_openedTileTechButtons[27] != null)
                    _openedTileTechButtons[27].gameObject.SetActive(true);

            #endregion
            
            if (controller.technologies.Contains(TechInfo.Technology.Sailing))
            {
                if (_openedTileTechButtons[8] != null)
                    _openedTileTechButtons[8].gameObject.SetActive(true);
            }
            if (controller.technologies.Contains(TechInfo.Technology.Aquatism))
            {
                if (_openedTileTechButtons[17] != null)
                    _openedTileTechButtons[17].gameObject.SetActive(true);
            }
            return;
        }

        if (types.Contains(OpenedTechType.DeepWater))
        {
            if (controller.technologies.Contains(TechInfo.Technology.Whaling))
            {
                if (_openedTileTechButtons[9] != null)
                    _openedTileTechButtons[9].gameObject.SetActive(true);
            }
            return;
        }
        if(types.Contains(OpenedTechType.Ground))
        {
            if (tileC.isHasMountain)
            {
                if (controller.technologies.Contains(TechInfo.Technology.Mining))
                {
                    if (_openedTileTechButtons[6] != null)
                        _openedTileTechButtons[6].gameObject.SetActive(true);
                }

                if (controller.technologies.Contains(TechInfo.Technology.Mining))
                {
                    if (_openedTileTechButtons[7] != null)
                        _openedTileTechButtons[7].gameObject.SetActive(true);
                }
                return;
            }

            if (types.Contains(OpenedTechType.Animal))
            {
                if(_openedTileTechButtons[2] != null)
                    _openedTileTechButtons[2]?.gameObject.SetActive(true);
            }
            if (types.Contains(OpenedTechType.Fruit))
            {
                if(_openedTileTechButtons[0] != null)
                    _openedTileTechButtons[0].gameObject.SetActive(true);
            }
            if (types.Contains(OpenedTechType.Fish))
            {
                if (_openedTileTechButtons[1] != null) 
                    _openedTileTechButtons[1].gameObject.SetActive(true);
            }
            if(types.TrueForAll(ty => ty == OpenedTechType.Ground))
            {
                #region Monuments

                
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.AltarOfPeace))
                    if (_openedTileTechButtons[21] != null)
                        _openedTileTechButtons[21].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.EmperorTomb))
                    if (_openedTileTechButtons[22] != null)
                        _openedTileTechButtons[22].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.EyeOfGod))
                    if (_openedTileTechButtons[23] != null)
                        _openedTileTechButtons[23].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.GateOfPower))
                    if (_openedTileTechButtons[24] != null)
                        _openedTileTechButtons[24].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.GrandBazaar))
                    if (_openedTileTechButtons[25] != null)
                        _openedTileTechButtons[25].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.ParkOfFortune))
                    if (_openedTileTechButtons[26] != null)
                        _openedTileTechButtons[26].gameObject.SetActive(true);
                if(tileC.GetOwner().owner.GetMonumentBuilder().IsMonumentAvailable(MonumentBuilder.MonumentType.TowerOfWisdom))
                    if (_openedTileTechButtons[27] != null)
                        _openedTileTechButtons[27].gameObject.SetActive(true);

                #endregion
                
                
                if (controller.technologies.Contains(TechInfo.Technology.FreeSpirit))
                {
                    if (_openedTileTechButtons[4] != null)
                        _openedTileTechButtons[4].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Trade))
                {
                    if(board.GetCloseTile(tileC, 1).Find(tile => tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType == BuildingUpgrade.BuildType.Port))
                        if (_openedTileTechButtons[12] != null)
                            _openedTileTechButtons[12].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Construction))
                {
                    if(board.GetCloseTile(tileC, 1).Find(tile => tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType == BuildingUpgrade.BuildType.Farm))
                        if (_openedTileTechButtons[14] != null)
                            _openedTileTechButtons[14].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Forge))
                {
                    if(board.GetCloseTile(tileC, 1).Find(tile => tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType == BuildingUpgrade.BuildType.Mine))
                        if (_openedTileTechButtons[16] != null)
                            _openedTileTechButtons[16].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Spiritualism))
                {
                    if (_openedTileTechButtons[18] != null)
                        _openedTileTechButtons[18].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Mathematics))
                {
                    if(board.GetCloseTile(tileC, 1).Find(tile => tile.GetBuildingUpgrade() != null && tile.GetBuildingUpgrade().currentType == BuildingUpgrade.BuildType.LumberHut))
                        if (_openedTileTechButtons[20] != null)
                           _openedTileTechButtons[20].gameObject.SetActive(true);
                }
            }
            
            if (types.Contains(OpenedTechType.Tree))
            {
                if (controller.technologies.Contains(TechInfo.Technology.Spiritualism))
                {
                    if (_openedTileTechButtons[19] != null)
                        _openedTileTechButtons[19].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Forestry))
                {
                    if (_openedTileTechButtons[10] != null)
                        _openedTileTechButtons[10].gameObject.SetActive(true);
                    if (_openedTileTechButtons[11] != null)
                        _openedTileTechButtons[11].gameObject.SetActive(true);
                }
                if (controller.technologies.Contains(TechInfo.Technology.Chivalry))
                {
                    if (_openedTileTechButtons[13] != null)
                        _openedTileTechButtons[13].gameObject.SetActive(true);
                }
                
            }

            if (types.Contains(OpenedTechType.Crop))
            {
                if (controller.technologies.Contains(TechInfo.Technology.Farming))
                {
                    if (_openedTileTechButtons[5] != null)
                        _openedTileTechButtons[5].gameObject.SetActive(true);
                }
            }
            
            
            if(controller.technologies.Contains(TechInfo.Technology.Roads))
            {
                if (_openedTileTechButtons[3] != null)
                    _openedTileTechButtons[3].gameObject.SetActive(true);
            }
            return;
        }
        
    }
    
    public void ShowUnitButton(List<int> types, UnitController unit)
    {
        if(types.Count == 0)
            return;
        downBar.SetActive(true);
        unitButtonParent.SetActive(true);
        foreach (var tile in unitButtons)
        {
            tile.gameObject.SetActive(false);            
        }

        foreach (var type in types)
        {
            switch (type)
            {
                case 0:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((() =>
                    {
                        unit.Heal(2);
                        HideDownBar();
                    }));
                    break;
                case 1:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((() =>
                    {
                        unit.DisbandTheSquad();
                        HideDownBar();
                    }));
                    break;
                case 2:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((() =>
                    {
                        var closeTile = LevelManager.Instance.gameBoardWindow.GetCloseTile(unit.occupiedTile, 1);
                        foreach (var tile in closeTile)
                        {
                            if(tile != null && tile.unitOnTile != null && tile.unitOnTile.GetOwner().owner == unit.GetOwner().owner)
                                tile.unitOnTile.Heal(5);
                        }
                        HideDownBar();
                    }));  
                    break;
                case 3:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((() =>
                    {
                        unit.LevelUp();
                        HideDownBar();
                    }));  
                    break;
                case 4:
                    unitButtons[type].gameObject.SetActive(true);
                    if (unit.GetUnitType() == UnitController.UnitType.Boat)
                    {
                        unitButtons[type].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                        unitButtons[type].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    }
                        
                    if (unit.GetUnitType() == UnitController.UnitType.Ship)
                    {
                        unitButtons[type].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                        unitButtons[type].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    }
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((() =>
                    {
                        unit.ShipLevelUp();
                        unitButtons[type].gameObject.SetActive(false);
                        HideDownBar();
                    })); 
                    break;
            }
        }
    }
    
    private void Awake()
    {
        LevelManager.Instance.gameplayWindow = this;
        menuButton.onClick.AddListener(LoadMenuScene);
        technologyOpenButton.onClick.AddListener(OpenTechnologyWindow);
        technologyCloseButton.onClick.AddListener(CloseTechnologyWindow);
        turnEndButton.onClick.AddListener(EndTurn);
        downBarBackButton.onClick.AddListener(HideDownBar);
        UnlockUnitTech(0);
        var inVal = 0f;

        LevelManager.Instance.OnTurnEnd += TurnEnd;

        LevelManager.Instance.OnTurnBegin += TurnBegin;

        LevelManager.Instance.OnObjectSelect += SelectEvent;
        
        technologyObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LevelManager.Instance.gameBoardWindow.playerCiv.OnMoneyChanged -= MoneyChanged;
        LevelManager.Instance.OnTurnEnd -= TurnEnd;
        LevelManager.Instance.OnTurnBegin -= TurnBegin;
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (currO != null && !currO.TryGetComponent(out Home home))
        {
            if (unitTechButtons != null && unitTechButtons.Count != 0)
            {
                if (unitTechButtonParent != null) unitTechButtonParent.SetActive(false);
            }
        }

        if (currO != null && !currO.TryGetComponent(out Tile tile))
        {
            if (tileTechButtons != null && tileTechButtons.Count != 0)
            {
                if (tileTechButtonParent != null)
                {
                    foreach (var tilea in tileTechButtons)
                    {
                        tilea.gameObject.SetActive(false);
                    }
                    tileTechButtonParent.SetActive(false);
                }
            }
        }
        
        if (currO != null && !currO.TryGetComponent(out UnitController unit))
        {
            if (unitButtons != null && unitButtons.Count != 0)
            {
                if (unitButtonParent != null)
                {
                    foreach (var unitButton in unitButtons)
                    {
                        unitButton.gameObject.SetActive(false);
                    }
                    tileTechButtonParent.SetActive(false);
                }
            }
        }

        
        if (currO == null)
        {
            HideDownBar();
        }
    }

    private void TurnBegin()
    {
        Debug.Log("TurnBegin");
        BlockInput(false);
        ShowTurnBegin();
        currentTurnUGUI.text = "Ход: " + LevelManager.Instance.currentTurn;
    }
    
    private void TurnEnd()
    {
        Debug.Log("TurnEnd");
        BlockInput(true);
        ShowTurnEnd();
        HideDownBar();
    }

    private void MoneyChanged()
    {
        currentMoneyUGUI.text = "Звезд: " + LevelManager.Instance.gameBoardWindow.playerCiv.Money;
        currentPointUGUI.text = "Очков: " + LevelManager.Instance.gameBoardWindow.playerCiv.Point;
    }

    private void ShowTurnEnd()
    {
        if (turnEnd != null) turnEnd.SetActive(true);
    }

    private void ShowTurnBegin()
    {
        turnEnd.SetActive(false);
        turnBegin.SetActive(true);
        var inVal = 0;
        DOTween.To(() => inVal, x=> inVal = x, 1, 0.4f).OnComplete(() =>
        {
            turnBegin.SetActive(false);
        });
    }
    
    private void HideDownBar()
    {
        if (downBar != null) downBar.SetActive(false);
        if (unitTechButtonParent != null) unitTechButtonParent.SetActive(false);
        if (tileTechButtonParent != null) tileTechButtonParent.SetActive(false);
        if (tileNameUGUI != null) tileNameUGUI.gameObject.SetActive(false);
    }

    private void LoadMenuScene()
    {
        LevelManager.Instance.DestroyAllCivilisation();
        WindowsManager.Instance.CloseAllWindows();
        WindowsManager.Instance.CreateWindow<MenuWindow>("MenuWindow").ShowWindow();
    }

    private void BuyUnit(int index)
    {
        OnUnitSpawn?.Invoke(index);
        HideDownBar();
    }
    
    private void BuyTileTech(int index)
    {
        OnTileTech?.Invoke(index);
        HideDownBar();
    }
    
    private void OpenTechnologyWindow()
    {
        technologyObject.SetActive(!technologyObject.activeSelf);
    }
    
    private void CloseTechnologyWindow()
    {
        technologyObject.SetActive(false);
    }
    
    private void EndTurn()
    {
        LevelManager.Instance.OnTurnEnd?.Invoke();
    }

    private void BlockInput(bool value)
    {
        inputBlockObject.SetActive(value);
    }
}
