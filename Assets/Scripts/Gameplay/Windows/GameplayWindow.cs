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
        Ground,
        Water,
        Construct
    }
    
    [SerializeField] private TextMeshProUGUI currentTurnUGUI;
    [SerializeField] private TextMeshProUGUI currentMoneyUGUI;
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
        _openedTileTechButtons ??= new List<Button>(){null, null, null, null, null, null, null, null, null, null};
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
        var controller = tileC.GetOwner().owner;
        if(controller == null)
            return;
        foreach (var tile in tileTechButtons)
        {
            tile.gameObject.SetActive(false);            
        }
        
        if(types.Contains(OpenedTechType.Construct))
            return;
        foreach (var type in types)
        {
            if (type == OpenedTechType.Animal)
            {
                if(_openedTileTechButtons[1] != null)
                    _openedTileTechButtons[1]?.gameObject.SetActive(true);
            }
            if (type == OpenedTechType.Fruit)
            {
                if(_openedTileTechButtons[0] != null)
                    _openedTileTechButtons[0].gameObject.SetActive(true);
            }
            if (type == OpenedTechType.Fish)
            {
                if (_openedTileTechButtons[2] != null) 
                    _openedTileTechButtons[2].gameObject.SetActive(true);
            }
            if(type == OpenedTechType.Ground)
            {
                if (!tileC.isHasMountain)
                {
                    if(controller.technologies.Contains(TechInfo.Technology.FreeSpirit))
                    {
                        if(_openedTileTechButtons[3] != null)
                            _openedTileTechButtons[3].gameObject.SetActive(true);
                    }
                    if(controller.technologies.Contains(TechInfo.Technology.Farming))
                    {
                        if (_openedTileTechButtons[5] != null) 
                            _openedTileTechButtons[5].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if(controller.technologies.Contains(TechInfo.Technology.Mining))
                    {
                        if(_openedTileTechButtons[6] != null)
                            _openedTileTechButtons[6].gameObject.SetActive(true);
                    }
                }
            }
            if(type == OpenedTechType.Ground)
            {
                if(controller.technologies.Contains(TechInfo.Technology.FreeSpirit) && _openedTileTechButtons[3] != null)
                {
                    _openedTileTechButtons[3].gameObject.SetActive(true);
                }
            }
            if(controller.technologies.Contains(TechInfo.Technology.Sailing) && type == OpenedTechType.Water)
            {
                if (_openedTileTechButtons[4] != null)
                {
                    _openedTileTechButtons[4].gameObject.SetActive(true);
                }
            }
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
                    }));
                    break;
                case 1:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((unit.DisbandTheSquad)); break;
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
                    }));  
                    break;
                case 3:
                    unitButtons[type].gameObject.SetActive(true);
                    unitButtons[type].onClick.RemoveAllListeners();
                    unitButtons[type].onClick.AddListener((unit.LevelUp));  
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

        EconomicManager.Instance.OnMoneyChanged += MoneyChanged;

        LevelManager.Instance.OnTurnEnd += TurnEnd;

        LevelManager.Instance.OnTurnBegin += TurnBegin;

        LevelManager.Instance.OnObjectSelect += SelectEvent;
    }

    private void OnDestroy()
    {
        EconomicManager.Instance.OnMoneyChanged -= MoneyChanged;
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
        BlockInput(false);
        ShowTurnBegin();
        currentTurnUGUI.text = "Ход: " + LevelManager.Instance.currentTurn;
    }
    
    private void TurnEnd()
    {
        BlockInput(true);
        ShowTurnEnd();
        HideDownBar();
    }

    private void MoneyChanged()
    {
        currentMoneyUGUI.text = "Звезд: " + EconomicManager.Instance.Money;

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
