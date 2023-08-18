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
        BattleShip
    }

    public enum AttackType
    {
        Melee,
        Range
    }
    
    public Tile occupiedTile;
    public string aiTaskName;
    
    [SerializeField] private UnitInfo unitInfo;
    [SerializeField] private List<Image> unitBackGrounds;
    [SerializeField] private Image headImage;
    [SerializeField] private Image battleShipHeadImage;
    [SerializeField] private Image horseImage;
    [SerializeField] private TextMeshProUGUI unitHpTMPro;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<UnitController> shipUnits;
    [SerializeField] private UnitType unitType;
    [SerializeField] private AttackType attackType;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<RectTransform> sweats;
    
    private UnitController _unitInTheShip;
    private Home _owner;
    private bool _isCanSelected = true;
    private int _moveThisTurn = 1;
    private int _attackThisTurn = 1;
    private float _hp;
    private int _unitIndex;
    private Sequence _moveSeq;
    private Sequence _attSeq;
    private Sequence _counterstrikeSeq;
    private int _killCount = 0;
    private int _lvl = 1;

    public void Init(Home owner, Tile tile, int index)
    {
        SetSweatPos();
        _moveThisTurn = 0;
        _attackThisTurn = 0;
        transform.SetSiblingIndex(transform.parent.childCount);
        OccupyTile(tile);
        var anchorPos = occupiedTile.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorPos.x, anchorPos.y + 30);
        SetOwner(owner);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
        LevelManager.Instance.OnTurnEnd += () =>
        {
            _isCanSelected = false;
        };
        LevelManager.Instance.OnTurnBegin += () =>
        {
            _isCanSelected = true;
            EnableUnit();
        };

        _unitIndex = index;
        
        _hp = unitInfo.hp;
        unitHpTMPro.text = _hp.ToString();
        
        if(owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI && !occupiedTile.isOpened)
            gameObject.SetActive(false);
    }

    public void EnableUnit()
    {
        _moveThisTurn = 1;
        _attackThisTurn = 1;
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
    
    public void TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0)
        {
            KillUnit();
        }
        
        unitHpTMPro.text = _hp.ToString();
    }

    public float GetHp()
    {
        return _hp;
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
        _hp = _unitInTheShip.GetUnitInfo().hp;
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
        _owner.owner.AddMoney(Mathf.CeilToInt(Mathf.Round(unitInfo.price/2 + 1)));
        KillUnit();
    }
    
    public void SelectUnit()
    {
        if(!_isCanSelected) return;

        LevelManager.Instance.SelectObject(gameObject);
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI) 
            return;

        var gameBoard = LevelManager.Instance.gameBoardWindow;
        var allTile = gameBoard.GetAllTile();
        var closeTilesForMove = gameBoard.GetCloseTile(occupiedTile, unitInfo.moveRad);
        var closeTilesForAttack = gameBoard.GetCloseTile(occupiedTile, unitInfo.rad);
        foreach (var vector2Int in allTile.Keys.ToList())
        {
            var tile = allTile[vector2Int];
            tile.HideTargets();
        }
        foreach (var closeTile in closeTilesForMove)
        {
            if(_moveThisTurn <= 0) break;
            if(closeTile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                continue;
            
            if((unitType != UnitType.Boat || unitType != UnitType.Ship || unitType != UnitType.BattleShip) && closeTile.tileType == Tile.TileType.Water && !closeTile.IsTileHasPort())
                continue;
            closeTile.ShowBlueTarget();
        }

        foreach (var tile in closeTilesForAttack)
        {
            if(_attackThisTurn <= 0) break;
            if(tile.unitOnTile != null && tile.unitOnTile.IsThisUnitAlly(_owner.owner))
                continue;
            tile.ShowRedTarget(this);
        }
    }

    public int GetDmg()
    {
        var attackForce = unitInfo.dmg * (_hp / unitInfo.hp);
        var totalDamage = unitInfo.dmg + unitInfo.def;
        var a = attackForce / totalDamage;
        var attackResult = Mathf.Round(a * unitInfo.dmg * 4.5f); 

        return (int)attackResult;
    }
    
    public int GetDefDmg()
    {
        var defenseForce = unitInfo.def * (_hp / unitInfo.hp) * GetDefenseBonus();
        var totalDamage = unitInfo.dmg + unitInfo.def;
        var a = defenseForce / totalDamage;

        var defenseResult = Mathf.Round(a * unitInfo.def * 4.5f);
        return (int)defenseResult;
    }
    
    public bool IsThisUnitAlly(CivilisationController controller)
    {
        return _owner.owner == controller;
    }
    
    public void SetOwner(Home controller)
    {
        _owner = controller;
        if (headImage != null && controller != null) headImage.sprite = controller.owner.civilisationInfo.HeadSprite;
        if (battleShipHeadImage != null && controller != null) battleShipHeadImage.sprite = controller.owner.civilisationInfo.BattleShipHeadSprite;
        if (horseImage != null && controller != null) horseImage.sprite = controller.owner.civilisationInfo.AnimalSprite;
        foreach (var unitBackGround in unitBackGrounds)
        {
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
        if(to.isOpened)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
        _moveSeq = DOTween.Sequence();
        aiFromTile = occupiedTile;
        _moveThisTurn = 0;
        if (_attackThisTurn != 0 && !CheckAbility(UnitInfo.AbilityType.Dash))
            _attackThisTurn = 0;
        else if(CheckAbility(UnitInfo.AbilityType.Dash))
            _attackThisTurn = 1;
        if(CheckAbility(UnitInfo.AbilityType.Persist)) 
            _attackThisTurn = 1;
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetSiblingIndex(transform.parent.childCount);
        var inValX = rectTransform.anchoredPosition.x;
        OccupyTile(to);
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
            if (occupiedTile.GetHomeOnTile() != null && (occupiedTile.GetHomeOnTile().owner == null || occupiedTile.GetHomeOnTile().owner != _owner.owner))
            {
                if(_owner.owner.civilisationInfo.controlType != CivilisationInfo.ControlType.AI)
                    occupiedTile.GetHomeOnTile().ShowOccupyButton();
            }
            var addedRad = 0;
            if (occupiedTile.isHasMountain)
                addedRad = 1;
            var closeTiles = LevelManager.Instance.gameBoardWindow.GetCloseTile(to, unitInfo.rad + addedRad);
            LevelManager.Instance.SelectObject(null);
            foreach (var tile in closeTiles)
            {
                if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
                    tile.UnlockTile();
            }

            if (unitType == UnitType.Unit && to.IsTileHasPort())
            {
                TurnIntoAShip(0);
            }

            if ((unitType == UnitType.Boat || unitType == UnitType.Ship || unitType == UnitType.BattleShip) && to.tileType == Tile.TileType.Ground)
            {
                TurnIntoAUnit();
            }
        }));

        return _moveSeq;
    }
    
    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;

        _moveSeq.Kill();
        _attSeq.Kill();
        _counterstrikeSeq.Kill();
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (currO != null && currO == gameObject && _owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.Player)
        {
            var ints = new List<int>();
            if(_hp < unitInfo.hp)
                ints.Add(0);
            if(_owner.owner.technologies.Contains(TechInfo.Technology.FreeSpirit))
                ints.Add(1);
            if(unitInfo.abilityTypes.Contains(UnitInfo.AbilityType.Heal))
                ints.Add(2);
            if(_killCount == 3 && _lvl == 1)
                ints.Add(3);
            if(unitType == UnitType.Boat || unitType == UnitType.Ship)
                ints.Add(4);
            LevelManager.Instance.gameplayWindow.ShowUnitButton(ints, this);
        }
        
        if (pastO == gameObject)
        {
            if (_attackThisTurn > 0)
            {
                if (currO != null && pastO != null && currO.TryGetComponent(out UnitController unit1) && pastO.TryGetComponent(out UnitController unit2) && pastO != currO)
                {
                    if (unit2 == this)
                    {
                        if(!unit1.IsThisUnitAlly(_owner.owner) && unit1.occupiedTile.IsCanAttackTo())
                            AttackUnitOnTile(unit1);
                        return;
                    }
                }
            }

            if (_moveThisTurn > 0)
            {
                if (currO != null && (currO.TryGetComponent(out Tile tile) || currO.TryGetComponent(out Home home)))
                {
                    if (tile.IsTileFree() && tile.IsCanMoveTo())
                        MoveToTile(tile);
                }
            }
        }
    }

    public Tween AttackUnitOnTile(UnitController unitToAttack)
    {
        _attSeq = DOTween.Sequence();
        if (unitToAttack._owner.owner == _owner.owner)
            return _attSeq;
        var pos = transform.position;
        var rad = 1;
        if (attackType == AttackType.Range)
            rad = unitInfo.rad;
        var isThisTheNearestTile = LevelManager.Instance.gameBoardWindow.IsThisTheNearestTile(unitToAttack.occupiedTile, occupiedTile, rad);
        if (unitToAttack.CheckForKill(GetDmg()))
        {
            unitToAttack.TakeDamage(GetDmg());
            AddKillInCount();
            if (isThisTheNearestTile && attackType == AttackType.Melee)
            {
                _attSeq.Append(MoveToTile(unitToAttack.occupiedTile, 0.1f));
                _attackThisTurn = 0; 
                if(CheckAbility(UnitInfo.AbilityType.Persist)) 
                    _attackThisTurn = 1;
                if(!CheckAbility(UnitInfo.AbilityType.Escape))
                    _moveThisTurn = 0;
            }
            else
            {
                _attackThisTurn = 0; 
                if(CheckAbility(UnitInfo.AbilityType.Persist)) 
                    _attackThisTurn = 1;
                if(!CheckAbility(UnitInfo.AbilityType.Escape))
                   _moveThisTurn = 0;
                
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.position = transform.position;
                _attSeq.Append(pr.transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    LevelManager.Instance.SelectObject(null);
                    SelectUnit();
                    Destroy(pr.gameObject);
                })));
            }
        }
        else
        {
            _attackThisTurn = 0;
            if(CheckAbility(UnitInfo.AbilityType.Persist)) 
                _attackThisTurn = 1;
            if(!CheckAbility(UnitInfo.AbilityType.Escape))
                _moveThisTurn = 0;
            if (attackType == AttackType.Melee)
            {
                _attSeq.Append(transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDmg());
                    LevelManager.Instance.SelectObject(null);
                    SelectUnit();

                })));
                _attSeq.Append(transform.DOMove(pos, 0.1f));
                if (CheckAbility(UnitInfo.AbilityType.Convert))
                {
                    unitToAttack.GetOwner().RemoveUnit(unitToAttack);
                    unitToAttack.SetOwner(_owner);
                }
            }
            else
            {
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.position = transform.position;
                _attSeq.Append(pr.transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDmg());
                    LevelManager.Instance.SelectObject(null);
                    SelectUnit();
                    Destroy(pr.gameObject);
                })));
            } 
            _attSeq.Append(unitToAttack.Counterstrike(this));
        }
        
        return _attSeq;
    }

    private void AddKillInCount()
    { 
        _killCount++;
    }
    
    private Tween Counterstrike(UnitController unitToAttack)
    {
        _counterstrikeSeq = DOTween.Sequence();
        if (unitToAttack._owner.owner == _owner.owner)
            return _counterstrikeSeq;
        var rad = 1;
        if (attackType == AttackType.Range)
            rad = unitInfo.rad;
        var pos = transform.position;
        var isThisTheNearestTile = LevelManager.Instance.gameBoardWindow.IsThisTheNearestTile(unitToAttack.occupiedTile, occupiedTile, rad);
        if (!isThisTheNearestTile)
            return _counterstrikeSeq;
        
        if (unitToAttack.CheckForKill(GetDefDmg()))
        {
            if (attackType == AttackType.Melee)
            {
                _counterstrikeSeq.Append(transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDefDmg());
                })));
                _counterstrikeSeq.Append(transform.DOMove(pos, 0.1f));
            }
            else
            {
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.position = transform.position;
                _attSeq.Append(pr.transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDefDmg());
                    Destroy(pr.gameObject);
                })));
            }
        }
        else
        {
            if (attackType == AttackType.Melee)
            {
                _counterstrikeSeq.Append(transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDefDmg());
                })));
                _counterstrikeSeq.Append(transform.DOMove(pos, 0.1f));
            }
            else
            {
                var pr = Instantiate(projectilePrefab, transform.parent);
                pr.transform.position = transform.position;
                _attSeq.Append(pr.transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
                {
                    unitToAttack.TakeDamage(GetDefDmg());
                    Destroy(pr.gameObject);
                })));
            }
        }
        return _counterstrikeSeq;
    }

    private void TurnIntoAShip(int index)
    {
        LevelManager.Instance.SelectObject(null);
      
        var ship = Instantiate(shipUnits[index], transform.parent);
        ship.Init(_owner, occupiedTile, 0);
        if(_unitInTheShip == null)
            ship.SetUnitInShip(this);
        else
            ship.SetUnitInShip(_unitInTheShip);
        
        
        gameObject.SetActive(false);
        if(unitType is UnitType.Ship or UnitType.BattleShip)
            Destroy(gameObject);
    }
    
    private void TurnIntoAUnit()
    {
        LevelManager.Instance.SelectObject(null);
        
        _unitInTheShip.gameObject.SetActive(true);
        _unitInTheShip.Init(_owner, occupiedTile, _unitInTheShip._unitIndex);
        occupiedTile.unitOnTile = _unitInTheShip;
        KillUnit();
    }

    private int GetDefenseBonus()
    {
        var bonus = 1;
        if (occupiedTile.isHasMountain)
        {
            bonus++;
        }

        if (occupiedTile.GetHomeOnTile() != null)
        {
            bonus++;
        }
        return bonus;
    }

    private void KillUnit()
    {
        if(occupiedTile.unitOnTile == this)
            occupiedTile.unitOnTile = null;
        if (_owner != null) 
            _owner.RemoveUnit(this);
        Destroy(gameObject);
    }
    
    #region SweatAnim
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

    private Tween _sweatDot;
    
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
            if(!_isSweaty)
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

            var inVAl = 0f;
            _sweatDot = DOTween.To(() => inVAl, x => x = inVAl, 1, sweatyDur + 0.1f).OnComplete((() =>
            {
                if(_sweatDot == null)
                    _sweatDot.Kill();
                foreach (var sweat in sweats)
                {
                    sweat.gameObject.SetActive(false);
                }
                if(_isSweaty)
                    SweatyAnimation();
            }));
        }
    }
    
    [Button()]
    public void SweatingAnimationDisable()
    {
        _isSweaty = false;
    }

    #endregion
    
    public string aiName = "unit1";
    public Home aiHomeExploring;
    public Tile aiFromTile;
}
