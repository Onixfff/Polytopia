using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public enum UnitType
    {
        Unit,
        Boat,
        Ship,
        BattleShip,
        Pirate,
        Dinghy
    }

    public enum AttackType
    {
        Melee,
        Range
    }
    
    public Tile occupiedTile;
    public string aiTaskName;
    
    [SerializeField] private UnitInfo unitInfo;
    [SerializeField] private List<Image> unitVisual;
    [SerializeField] private List<Image> unitBackGrounds;
    [SerializeField] private Image headImage;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImage1;
    [SerializeField] private Image battleShipHeadImage;
    [SerializeField] private Image horseImage;
    [SerializeField] private TextMeshProUGUI unitHpTMPro;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<UnitController> shipUnits;
    [SerializeField] private UnitType unitType;
    [SerializeField] private AttackType attackType;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<RectTransform> sweats;
    [SerializeField] private Color disableColor;
    
    private UnitController _unitInTheShip;
    private Home _owner;
    private CivilisationController _civOwner;
    private bool _isCanSelected = true;
    private int _moveThisTurn = 1;
    private int _attackThisTurn = 1;
    private float _hp;
    private Sequence _moveSeq;
    private Sequence _attSeq;
    private Sequence _counterstrikeSeq;
    private int _killCount = 0;
    private int _lvl = 1;
    
    public void Init(Home owner, Tile tile, bool isIndependent)
    {
        SetSweatPos();
        _moveThisTurn = 0;
        _attackThisTurn = 0;
        transform.SetSiblingIndex(transform.parent.childCount);
        OccupyTile(tile);
        var anchorPos = occupiedTile.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorPos.x, anchorPos.y + 30);
        if(!isIndependent)
            SetOwner(owner);
        else
            SetIndependentOwner(owner);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
        LevelManager.Instance.OnTurnEnd += DisableUnit;
        LevelManager.Instance.OnTurnBegin += EnableUnit;

        
        _hp = unitInfo.hp;
        unitHpTMPro.text = _hp.ToString();
        DisableUnit();
        
        if(owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI && !occupiedTile.isOpened) 
            gameObject.SetActive(false);
    }

    public void EnableUnit()
    {
        _isCanSelected = true;
        _moveThisTurn = 1;
        _attackThisTurn = 1;
        foreach (var visual in unitVisual)
        {
            try
            {
                visual.color = Color.white;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
    
    public void DisableUnit()
    {
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI)
            return;
        _isCanSelected = false;
        foreach (var visual in unitVisual)
        {
            visual.color = disableColor;
        }
    }
    
    public bool CheckAbility(UnitInfo.AbilityType abilityType)
    {
        return unitInfo.abilityTypes.Contains(abilityType);
    }
    
    public bool CheckForKill(int dmg)
    {
        var h = _hp;
        h -= dmg;
        if (h <= 0)
            return true;
        return false;
    }
    
    public void TakeDamage(UnitController dealer, int dmg)
    {
        _hp -= dmg;
        unitHpTMPro.text = _hp.ToString();
        if (_hp <= 0)
        {
            DisableUnit();
            foreach (var visual in unitVisual)
            {
                visual.color = new Color(0, 0, 0, 0);
            }
            foreach (var visual in unitBackGrounds)
            {
                visual.color = new Color(0, 0, 0, 0);
            }
        }
        GetDamageAnim(dmg).OnComplete((() =>
        {
            if (_hp <= 0)
            {
                if (dealer != null)
                {
                    dealer.AddKillInCount();
                }

                KillUnit();
            }
        }));
    }

    public float GetHp()
    {
        return _hp;
    }

    public void SelfHeal(int heal)
    {
        Heal(heal);
        _moveThisTurn = 0;
        _attackThisTurn = 0;
        LevelManager.Instance.SelectObject(null);
    }
    
    public void Heal(int heal)
    {
        _hp += heal;
        unitHpTMPro.text = _hp.ToString();
    }
    
    public void LevelUp()
    {
        _hp = unitInfo.hp;
        _hp += 5;
    }
    
    public void ShipLevelUp()
    {
        if (unitType == UnitType.Boat)
        {
            TurnIntoAShip(1);
        }

        if (unitType == UnitType.Ship)
        {
            TurnIntoAShip(2);
        }        
    }
    
    public void DisbandTheSquad()
    {
        LevelManager.Instance.SelectObject(null);
        _owner.owner.AddMoney(Mathf.CeilToInt(Mathf.Round(unitInfo.price/2 + 1)));
        TakeDamage(null, 10000);
    }
    
    public void SelectUnit()
    {
        LevelManager.Instance.SelectObject(gameObject);
        AnimSelect();

        if(!_isCanSelected) return;
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI) 
            return;
        var gameBoard = LevelManager.Instance.gameBoardWindow;
        var allTile = gameBoard.GetAllTile();
        var closeTilesForAttack = gameBoard.GetCloseTile(occupiedTile, unitInfo.rad);
        foreach (var vector2Int in allTile.Keys.ToList())
        {
            var tile = allTile[vector2Int];
            tile.HideTargets();
        }

        var tileForMove = new List<Tile>();
        if (occupiedTile.tileType == Tile.TileType.Ground)
        {
            foreach (var close in gameBoard.GetCloseTile(occupiedTile, 1))
            {
                if(_moveThisTurn <= 0) break;
            
                if(close.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                    continue;
                if(!close.IsTileFree())
                    continue;
                if((close.tileType == Tile.TileType.Water && !close.IsTileHasPort()) || close.tileType == Tile.TileType.DeepWater)
                    continue;
                tileForMove.Add(close);
                if(close.isHasMountain || close.isHasTree)
                    continue;
                if(unitInfo.moveRad == 1)
                    continue;
            
                var closeTile = gameBoard.GetCloseTile(close, 1);
                foreach (var tile in closeTile)
                {
                    if(tile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                        continue;
                    if(!tile.IsTileFree())
                        continue;
                    tileForMove.Add(tile);
                    if(tile.isHasMountain || tile.isHasTree || tile.tileType == Tile.TileType.Water || tile.tileType == Tile.TileType.DeepWater)
                        continue;
                    if(unitInfo.moveRad < 3)
                        continue;
                    var clTile = gameBoard.GetCloseTile(tile, 1);
                    foreach (var cTile in clTile)
                    {
                        if(cTile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                            continue;
                        if(!cTile.IsTileFree())
                            continue;
                        tileForMove.Add(cTile);
                    }
                }
            }
        }
        else
        {
            foreach (var close in gameBoard.GetCloseTile(occupiedTile, 1))
            {
                if(_moveThisTurn <= 0) break;
            
                if(close.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                    continue;
                
                if(!close.IsTileFree())
                    continue;
                
                tileForMove.Add(close);
                if(close.tileType == Tile.TileType.Ground)
                    continue;
                if(unitInfo.moveRad == 1)
                    continue;
            
                var closeTile = gameBoard.GetCloseTile(close, 1);
                foreach (var tile in closeTile)
                {
                    if(tile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                        continue;
                    if(!tile.IsTileFree())
                        continue;
            
                    tileForMove.Add(tile);
                    if(tile.tileType == Tile.TileType.Ground)
                        continue;
                    if(unitInfo.moveRad < 3)
                        continue;
                    var clTile = gameBoard.GetCloseTile(tile, 1);
                    foreach (var cTile in clTile)
                    {
                        if(cTile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                            continue;
                        if(!cTile.IsTileFree())
                            continue;
            
                        tileForMove.Add(cTile);
                    }
                }
            }
        }
        
        
        foreach (var closeTile in tileForMove)
        {
            if (unitType is UnitType.Unit or UnitType.Pirate && (closeTile.tileType == Tile.TileType.Ground || (closeTile.tileType == Tile.TileType.Water && closeTile.IsTileHasPort())))
            {
                closeTile.ShowBlueTarget();
            }            
            if (unitType is UnitType.Boat or UnitType.Ship or UnitType.Dinghy && closeTile.tileType is Tile.TileType.Water or Tile.TileType.Ground)
            {
                closeTile.ShowBlueTarget();
            }

            if (unitType == UnitType.BattleShip)
            {
                closeTile.ShowBlueTarget();
            }
        }

        foreach (var tile in closeTilesForAttack)
        {
            if(_attackThisTurn <= 0) break;
            if(tile.unitOnTile != null && tile.unitOnTile.IsThisUnitAlly(_owner.owner))
                continue;
            tile.ShowRedTarget(this);
        }
    }

    public int GetDmg(UnitController unitToAttack)
    {
        if (unitInfo.dmg == 0)
            return 0;
        var attackForce = unitInfo.dmg * (_hp / unitInfo.hp);
        var defenseForce = unitToAttack.unitInfo.def * (unitToAttack._hp / unitToAttack.unitInfo.hp) * unitToAttack.GetDefenseBonus();
        var totalDamage = attackForce + defenseForce;
        var attackResult = Mathf.RoundToInt((attackForce / totalDamage) * unitInfo.dmg * 4.5f); 
        return attackResult;
    }
    
    public int GetDefDmg(UnitController unitToAttack)
    {
        var attackForce = unitToAttack.unitInfo.dmg * (unitToAttack._hp / unitToAttack.unitInfo.hp);
        var defenseForce = unitInfo.def * (_hp / unitInfo.hp) * GetDefenseBonus();
        var totalDamage = attackForce + defenseForce;
        var defenseResult = Mathf.RoundToInt((defenseForce / totalDamage) * unitInfo.def * 4.5f);
        return defenseResult;
    }
    
    public bool IsThisUnitAlly(CivilisationController controller)
    {
        var isAlly = false;

        if (_owner.owner == controller)
        {
            isAlly = true;
        }
        else if (_owner.owner.CheckAlly(controller))
        {
            isAlly = true;
        }
        return isAlly;
    }
    
    public void SetOwner(Home controller)
    {
        _owner = controller;
        if (headImage != null && controller != null) headImage.sprite = controller.owner.civilisationInfo.HeadSprite;
        if (battleShipHeadImage != null && controller != null) battleShipHeadImage.sprite = controller.owner.civilisationInfo.BattleShipHeadSprite;
        if (horseImage != null && controller != null) horseImage.sprite = controller.owner.civilisationInfo.AnimalSprite;
        foreach (var unitBackGround in unitBackGrounds)
        {
            if(controller != null && controller.owner != null)
                unitBackGround.color = controller.owner.civColor;
        }
    }
    
    public void SetIndependentOwner(Home controller)
    {
        _owner = controller.owner.independentHome;
        if (headImage != null && controller != null) headImage.sprite = controller.owner.civilisationInfo.HeadSprite;
        if (battleShipHeadImage != null && controller != null) battleShipHeadImage.sprite = controller.owner.civilisationInfo.BattleShipHeadSprite;
        if (horseImage != null && controller != null) horseImage.sprite = controller.owner.civilisationInfo.AnimalSprite;
        foreach (var unitBackGround in unitBackGrounds)
        {
            if(controller != null && controller.owner != null)
                unitBackGround.color = controller.owner.civColor;
        }
    }
    
    public Home GetOwner()
    {
        return _owner;
    }
    
    public void OccupyTile(Tile tile)
    {
        tile.unitOnTile = this;
        occupiedTile = tile;
    }

    public UnitInfo GetUnitInfo()
    {
        return unitInfo;
    }
    
    public UnitType GetUnitType()
    {
        return unitType;
    }
    
    public void SetUnitInShip(UnitController unitController)
    {
        _unitInTheShip = unitController;
    }
    
    public Tween MoveToTile(Tile to, float dur = 0.2f)
    {
        if(occupiedTile != null)
        {
            occupiedTile.unitOnTile = null;
            if (occupiedTile.GetHomeOnTile() != null)
            {
                occupiedTile.GetHomeOnTile().HideOccupyButton();
            }
        }
        if (to.isOpened && !gameObject.activeSelf)
        {
            var playerCiv = LevelManager.Instance.gameBoardWindow.playerCiv;
            playerCiv.relationOfCivilisation.AddNewCivilisation(_owner.owner, DiplomacyManager.RelationType.Neutral);
            _owner.owner.relationOfCivilisation.AddNewCivilisation(playerCiv, DiplomacyManager.RelationType.None);
            gameObject.SetActive(true);
        }
        else
        if (!to.isOpened && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        
        _moveSeq = DOTween.Sequence();
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetSiblingIndex(transform.parent.childCount);
        var inValX = rectTransform.anchoredPosition.x;
        OccupyTile(to);
        LevelManager.Instance.SelectObject(null);
        _moveSeq.Append(DOTween.To(() => inValX, x => inValX = x, anchorPos.x, dur).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(inValX, rectTransform.anchoredPosition.y);
        })));
        var inValY = rectTransform.anchoredPosition.y;
        _moveSeq.Join(DOTween.To(() => inValY, x => inValY = x, anchorPos.y + 30, dur).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, inValY);
        })));
        var inVal1 = 0;
        _moveSeq.Join(DOTween.To(() => inVal1, x => inVal1 = x, 1, dur)).OnComplete((() =>
        {
            AfterMoveAction();
        }));

        return _moveSeq;

        void AfterMoveAction()
        {
            if(gameObject.activeSelf)
                BoardController.Instance.TryMoveTo(rectTransform);
            var board = LevelManager.Instance.gameBoardWindow;
            var gameplayWindow = LevelManager.Instance.gameplayWindow;
            var targets = board.GetCloseTile(occupiedTile, Mathf.Max(unitInfo.moveRad, unitInfo.rad) + 1);
            foreach (var tile in targets)
            {
                tile.HideTargets();
            }
            
            var addedRad = 0;
            if (occupiedTile.isHasMountain)
                addedRad = 1;
            var closeTiles = board.GetCloseTile(to, 1 + addedRad);

            switch (GetDefenseBonus())
            {
                case 1:
                    hpImage.gameObject.SetActive(false);
                    hpImage1.gameObject.SetActive(false);
                    break;
                case 1.5f:
                    hpImage.gameObject.SetActive(true);
                    hpImage1.gameObject.SetActive(false);
                    break;
                case 4:
                    hpImage.gameObject.SetActive(false);
                    hpImage1.gameObject.SetActive(true);
                    break;
            }
            
            foreach (var tile in closeTiles)
            {
                tile.UnlockTile(_owner.owner);
            }
            
            if (occupiedTile.GetHomeOnTile() != null && (occupiedTile.GetHomeOnTile().owner == null || occupiedTile.GetHomeOnTile().owner != _owner.owner))
            {
                if (_owner.owner.civilisationInfo.controlType != CivilisationInfo.ControlType.AI)
                {
                    occupiedTile.GetHomeOnTile().ShowOccupyButton();
                    gameplayWindow.ShowCaptureHomeNextTurnAlert();
                }
                else if(occupiedTile.GetHomeOnTile().owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
                {
                    gameplayWindow.ShowYourHomeIsCaptureAlert();
                }
            }
            if (occupiedTile.isHasRuins)
            {
                if(_owner.owner.civilisationInfo.controlType != CivilisationInfo.ControlType.AI)
                    occupiedTile.ShowRuinButton();
            }
            if (unitType == UnitType.Unit && to.IsTileHasPort())
            {
                TurnIntoAShip(0);
            }

            if ((unitType == UnitType.Boat || unitType == UnitType.Ship || unitType == UnitType.BattleShip) && to.tileType == Tile.TileType.Ground)
            {
                TurnIntoAUnit();
            }
            
            
            _moveThisTurn = 0;
            if (_attackThisTurn != 0 && !CheckAbility(UnitInfo.AbilityType.Dash))
            {
                if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
                    DisableUnit();
                _attackThisTurn = 0;
            }
            else if (CheckAbility(UnitInfo.AbilityType.Dash))
            {
                _attackThisTurn = 1;
            }

            if (CheckAbility(UnitInfo.AbilityType.Persist))
            {
                _attackThisTurn = 1;
            }

            if(_owner.owner.civilisationInfo.controlType != CivilisationInfo.ControlType.AI)
            {
                if(_attackThisTurn == 0)
                    return;
                var closeTile = board.GetCloseTile(occupiedTile, unitInfo.rad);
                closeTile.RemoveAll(tile => tile.unitOnTile == null);
                closeTile.RemoveAll(tile => tile.unitOnTile._owner.owner == null);
                closeTile.RemoveAll(tile => tile.unitOnTile._owner.owner == _owner.owner);
                closeTile.RemoveAll(tile => tile.unitOnTile._owner.owner.CheckAlly(_owner.owner)); 
                if(closeTile.Count == 0)
                    DisableUnit();
            }
        }
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
    
    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;

        LevelManager.Instance.OnTurnEnd -= DisableUnit;
        LevelManager.Instance.OnTurnBegin -= EnableUnit;
        _moveSeq.Kill();
        _attSeq.Kill();
        _counterstrikeSeq.Kill();
        _sweatDot.Kill();
        _selectJump.Kill();
        _damagePopupsSeq.Kill();
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (currO == null)
        {
            SweatingAnimationDisable();
        }
        
        if (currO != null && currO == gameObject)
        {
            if (_owner != null && _owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player || _civOwner != null &&
                _civOwner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
            {
                if(_moveThisTurn == 0 || _attackThisTurn == 0)
                {
                    LevelManager.Instance.gameplayWindow.HideUnitButton();
                }
                else
                {
                    var ints = new List<int>();
                    if (_hp < unitInfo.hp)
                        ints.Add(0);
                    if (_owner.owner.technologies.Contains(TechInfo.Technology.FreeSpirit))
                        ints.Add(1);
                    if (unitInfo.abilityTypes.Contains(UnitInfo.AbilityType.Heal))
                        ints.Add(2);
                    if (unitInfo.IsVeteran && _killCount >= 3 && _lvl == 1)
                        ints.Add(3);
                    
                    if (unitType == UnitType.Boat && _owner.owner.technologies.Contains(TechInfo.Technology.Sailing))
                        ints.Add(4);
                    else 
                    if (unitType == UnitType.Ship && _owner.owner.technologies.Contains(TechInfo.Technology.Navigation))
                        ints.Add(4);
                    
                    LevelManager.Instance.gameplayWindow.ShowUnitButton(ints, this);
                }
            }
            else
            {
                LevelManager.Instance.gameplayWindow.HideUnitButton();
            }
        }

        if (pastO == gameObject)
        {
            if (_attackThisTurn > 0)
            {
                if (currO != null && pastO != null && currO.TryGetComponent(out UnitController unit1) && pastO.TryGetComponent(out UnitController unit2) && pastO != currO)
                {
                    if (unit2 == this)
                    {
                        if (!unit1.IsThisUnitAlly(_owner.owner) && unit1.occupiedTile.IsCanAttackTo())
                        {
                            if (unitInfo.abilityTypes.Contains(UnitInfo.AbilityType.Infiltrate) && 
                                unit1.occupiedTile.GetHomeOnTile() != null && 
                                unit1.occupiedTile.GetHomeOnTile().owner != null && 
                                unit1.occupiedTile.GetHomeOnTile().owner != _owner.owner)
                            {
                                Infiltrate(unit1.occupiedTile.GetHomeOnTile());
                                return;
                            }
                            AttackUnitOnTile(unit1);
                        }
                        return;
                    }
                }
            }

            if (_moveThisTurn > 0)
            {
                if (currO != null && (currO.TryGetComponent(out Tile tile) || currO.TryGetComponent(out Home home)))
                {
                    if (unitInfo.abilityTypes.Contains(UnitInfo.AbilityType.Infiltrate) && 
                        tile.GetHomeOnTile() != null && 
                        tile.GetHomeOnTile().owner != null && 
                        tile.GetHomeOnTile().owner != _owner.owner)
                    {
                        Infiltrate(tile.GetHomeOnTile());
                        return;
                    }

                    if (tile.IsTileFree() && tile.IsCanMoveTo())
                    {
                        tile.HideTargets();
                        MoveToTile(tile);
                    }
                }
            }
        }
    }

    public Tween AttackUnitOnTile(UnitController unitToAttack)
    {
        foreach (var tiles in LevelManager.Instance.gameBoardWindow.GetCloseTile(occupiedTile, 5))
        {
            tiles.HideTargets();
        }
        _attSeq = DOTween.Sequence();
        if (unitToAttack._owner.owner == _owner.owner)
            return _attSeq;
        if (transform != null)
        {
            var pos = transform.localPosition;
            var rad = 1;
            if (attackType == AttackType.Range)
                rad = unitInfo.rad;
            var isThisTheNearestTile = LevelManager.Instance.gameBoardWindow.IsThisTheNearestTile(unitToAttack.occupiedTile, occupiedTile, rad);
        
            unitToAttack.GetOwner().owner.ChangeAnotherCivRelationAfterAttack(_owner.owner, true);
            _owner.owner.turnWhenIAttack = LevelManager.Instance.currentTurn;
        
            if (unitToAttack.CheckForKill(GetDmg(unitToAttack)))
            {
                unitToAttack.TakeDamage(this, GetDmg(unitToAttack));
                if (isThisTheNearestTile && attackType == AttackType.Melee)
                {
                    _attSeq.Append(MoveToTile(unitToAttack.occupiedTile, 0.1f).OnComplete((() =>
                    {
                        AfterAttack();
                    })));
                }
                else
                {
                
                    var pr = Instantiate(projectilePrefab, transform.parent);
                    pr.transform.localPosition = transform.localPosition;
                    _attSeq.Append(pr.transform.DOLocalMove(unitToAttack.transform.localPosition, 0.1f).OnComplete((() =>
                    {
                        LevelManager.Instance.SelectObject(null);
                        AfterAttack();
                        Destroy(pr.gameObject);
                    })));
                }
            }
            else
            {
                if (attackType == AttackType.Melee)
                {
                    _attSeq.Append(transform.DOLocalMove(unitToAttack.transform.localPosition, 0.1f).OnComplete((() =>
                    {
                        unitToAttack.TakeDamage(this, GetDmg(unitToAttack));
                        LevelManager.Instance.SelectObject(null);
                        AfterAttack();
                        if (CheckAbility(UnitInfo.AbilityType.Convert))
                        {
                            unitToAttack.GetOwner().RemoveUnit(unitToAttack);
                            unitToAttack.SetOwner(_owner);
                        }
                    })));
                    _attSeq.Append(transform.DOLocalMove(pos, 0.1f));
                }
                else
                {
                    var pr = Instantiate(projectilePrefab, transform.parent);
                    pr.transform.localPosition = transform.localPosition;
                    _attSeq.Append(pr.transform.DOLocalMove(unitToAttack.transform.localPosition, 0.1f).OnComplete((() =>
                    {
                        unitToAttack.TakeDamage(this, GetDmg(unitToAttack));
                        LevelManager.Instance.SelectObject(null);
                        //SelectUnit();
                        Destroy(pr.gameObject);
                    })));
                } 
                if (!CheckAbility(UnitInfo.AbilityType.Convert))
                    _attSeq.Append(unitToAttack.Counterstrike(this));
            }
        }
        
        return _attSeq;

        void AfterAttack()
        {
            _attackThisTurn = 0;
            _moveThisTurn = 0;
            if(CheckAbility(UnitInfo.AbilityType.Persist)) 
                _attackThisTurn = 1;
            else if (CheckAbility(UnitInfo.AbilityType.Escape))
            { 
                _moveThisTurn = 1;
            }
            else
            {
                if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
                    DisableUnit();
            }
        }
    }

    private void AddKillInCount()
    { 
        _killCount++;
        _owner.owner.GetCivilisationStats().AddKill();
    }
    
    private Tween Counterstrike(UnitController unitToAttack)
    {
        _counterstrikeSeq = DOTween.Sequence();
        if (unitToAttack._owner.owner == _owner.owner)
            return _counterstrikeSeq;
        var rad = 1;
        if (attackType == AttackType.Range)
            rad = unitInfo.rad;
        var pos = transform.localPosition;
        var isThisTheNearestTile = LevelManager.Instance.gameBoardWindow.IsThisTheNearestTile(unitToAttack.occupiedTile, occupiedTile, rad);
        if (!isThisTheNearestTile)
            return _counterstrikeSeq;
        var unitToAttackPos = unitToAttack.transform.localPosition;
        
        if (unitToAttack.CheckForKill(GetDefDmg(unitToAttack)))
        {
            if (attackType == AttackType.Melee)
            {
                _counterstrikeSeq.Append(transform.DOLocalMove(unitToAttackPos, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(this, GetDefDmg(unitToAttack));
                    transform.DOLocalMove(pos, 0.1f);
                })));
            }
            else
            {
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.localPosition = transform.localPosition;
                _attSeq.Append(pr.transform.DOLocalMove(unitToAttackPos, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(this, GetDefDmg(unitToAttack));
                    Destroy(pr.gameObject);
                })));
            }
        }
        else
        {
            if (attackType == AttackType.Melee)
            {
                _counterstrikeSeq.Append(transform.DOLocalMove(unitToAttackPos, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(this, GetDefDmg(unitToAttack));
                })));
                _counterstrikeSeq.Append(transform.DOLocalMove(pos, 0.1f));
            }
            else
            {
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.localPosition = transform.localPosition;
                _counterstrikeSeq.Append(pr.transform.DOLocalMove(unitToAttackPos, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(this, GetDefDmg(unitToAttack));
                    Destroy(pr.gameObject);
                })));
            }
        }
        return _counterstrikeSeq;
    }

    public void TurnIntoAShip(int index)
    {
        LevelManager.Instance.SelectObject(null);
      
        var ship = Instantiate(shipUnits[index], transform.parent);

        if (unitType is UnitType.Boat or UnitType.Ship)
        {
            ship.SetUnitInShip(_unitInTheShip);
        }
        else if(unitType is UnitType.Unit)
        {
            ship.SetUnitInShip(this);
        }
        
        if (ship.GetUnitInfo().abilityTypes.Contains(UnitInfo.AbilityType.Independent))
        {
            ship.Init(_owner.owner.independentHome, occupiedTile, true);
        }
        else
        {
            ship.Init(_owner, occupiedTile, false);
        }
        
        gameObject.SetActive(false);
        if(unitType is UnitType.Ship or UnitType.BattleShip)
            Destroy(gameObject);
        
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
            _unitInTheShip.DisableUnit();
    }
    
    private void TurnIntoAUnit()
    {
        LevelManager.Instance.SelectObject(null);
        
        _unitInTheShip.gameObject.SetActive(true);
        if(_unitInTheShip.GetUnitInfo().abilityTypes.Contains(UnitInfo.AbilityType.Independent))
            _unitInTheShip.Init(_owner.owner.independentHome, occupiedTile, true);
        else
            _unitInTheShip.Init(_owner, occupiedTile, false);
        occupiedTile.unitOnTile = _unitInTheShip;
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
            _unitInTheShip.DisableUnit();
        KillUnit();
    }

    private float GetDefenseBonus()
    {
        return occupiedTile.GetGroundDefense();
    }

    private void Infiltrate(Home home)
    {
        _moveSeq = DOTween.Sequence();
        var anchorPos = home.homeTile.GetComponent<RectTransform>().anchoredPosition;
        var inValX = 0f;
        LevelManager.Instance.SelectObject(null);

        _moveSeq.Append(DOTween.To(() => inValX, x => inValX = x, anchorPos.x, 0.2f).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(inValX, rectTransform.anchoredPosition.y);
        })));
        var inValY = rectTransform.anchoredPosition.y;
        _moveSeq.Join(DOTween.To(() => inValY, x => inValY = x, anchorPos.y + 30, 0.2f).OnUpdate((() =>
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, inValY);
        })));
        var inVal1 = 0f;
        _moveSeq.Join(DOTween.To(() => inVal1, x => inVal1 = x, 1, 0.2f)).OnComplete((() =>
        {
            var closeTiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(home.homeTile, unitInfo.rad);
            foreach (var tile in closeTiles)
            { 
                tile.UnlockTile(_owner.owner);
            }
            
            TakeDamage(null, 10000);
            home.Infiltrate(_owner);
        }));

    }

    private void KillUnit()
    {
        if(_unitInTheShip != null)
            _unitInTheShip.KillUnit();
        
        if(occupiedTile.unitOnTile == this)
            occupiedTile.unitOnTile = null;
        if (_owner != null) 
            _owner.RemoveUnit(this);

        SweatingAnimationDisable();
        if (gameObject != null) Destroy(gameObject);
    }
    
    #region UnitAnim
    private bool _isSweaty;
    public float sweatyDur;
    public Vector3 v0;
    public Vector3 v1;
    public Vector3 v2;
    private Vector3 sv0;
    private Vector3 sv1;
    private Vector3 sv2;
    [Button()]
    private void SetSweatPos()
    {
        sv0 = sweats[0].localPosition;
        sv1 = sweats[1].localPosition;
        sv2 = sweats[2].localPosition;
    }

    private Sequence _sweatDot;
    
    [Button()]
    public void SweatingAnimationEnable()
    {
        _isSweaty = true;
        var startPoses = new List<Vector3>()
        {
            sv0, sv1, sv2
        };
        SweatyAnimation();
        void SweatyAnimation()
        {
            if (transform == null)
            {
                _sweatDot.Kill();
                _sweatDot = null;
                return;
            }
            
            if(!_isSweaty)
                return;

            if(_sweatDot != null) 
                return;
            
            for (var i = 0; i < sweats.Count; i++)
            {
                var sweat = sweats[i];
                sweat.gameObject.SetActive(true);
                var normVector = new Vector3(1, 1, 1);
                var posVector = new Vector3(sweat.position.x, sweat.position.y, 0);
                switch (i)
                {
                    case 0:
                        posVector += v0;
                        break;
                    case 1:
                        posVector += v1;
                        break;
                    case 2:
                        posVector += v2;
                        break;
                }

                sweat.localPosition = startPoses[i];
                sweat.transform.localScale = Vector3.zero;
                sweat.DOLocalMove(posVector, sweatyDur);
                sweat.DOScale(normVector, sweatyDur / 2).OnComplete((() =>
                {
                    sweat.DOScale(Vector3.zero, sweatyDur / 2);
                }));
            }
            
            
            if(_sweatDot == null)
                return;
            var inVAl = 0f;
            _sweatDot.Append(DOTween.To(() => inVAl, x => x = inVAl, 1, sweatyDur + 0.1f).OnComplete((() =>
            {
                if (transform == null)
                {
                    _sweatDot.Kill();
                    _sweatDot = null;
                    return;
                }
                _sweatDot.Kill();
                _sweatDot = null;
                    
                foreach (var sweat in sweats)
                {
                    sweat.gameObject.SetActive(false);
                }
                if(_isSweaty)
                    SweatyAnimation();
            })));
        }
    }
    
    [Button()]
    public void SweatingAnimationDisable()
    {
        _isSweaty = false;
        _sweatDot.Kill();
    }
    
    [SerializeField] private AnimationCurve unitSelectAnimationCurve;
    [SerializeField] private float unitSelectAnimHeight = 20f;
    [SerializeField] private float unitSelectAnimTime = 0.2f;
    private Tween _selectJump;
    
    private void AnimSelect()
    {
        if(_selectJump != null) return;
        
        _selectJump = transform.DOLocalMoveY(transform.localPosition.y + unitSelectAnimHeight, unitSelectAnimTime).SetEase(unitSelectAnimationCurve).OnComplete((
            () =>
            {
                _selectJump.Kill();
                _selectJump = null;
            }));
    }
    
    [SerializeField] private AnimationCurve unitDamageAnimationCurve;
    [SerializeField] private TextMeshProUGUI damagePopupsTextPrefab;
    
    [SerializeField] private float unitDamagePopupsAnimHeight = 20f;
    [SerializeField] private float unitDamagePopupsAnimTime = 0.2f;
    private Sequence _damagePopupsSeq;

    private Tween GetDamageAnim(int dmg)
    {
        _damagePopupsSeq = DOTween.Sequence();
        var text = Instantiate(damagePopupsTextPrefab, transform);
        text.text = dmg.ToString();
        text.transform.position = headImage.transform.position;
        _damagePopupsSeq.Append(text.transform.DOLocalMoveY(text.transform.position.y + unitDamagePopupsAnimHeight, unitDamagePopupsAnimTime).SetEase(unitDamageAnimationCurve).OnComplete((
            () =>
            {
                Destroy(text.gameObject);
            })));
        return _damagePopupsSeq;
    }

    #endregion
    
    public string aiName = "unit1";
}
