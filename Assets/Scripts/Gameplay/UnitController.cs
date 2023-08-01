using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public enum UnitType
    {
        Unit,
        Ship
    }
    
    public Tile occupiedTile;

    [SerializeField] private UnitInfo unitInfo;
    [SerializeField] private List<Image> unitBackGrounds;
    [SerializeField] private Image headImage;
    [SerializeField] private Image horseImage;
    [SerializeField] private TextMeshProUGUI unitHpTMPro;
    [SerializeField] private Button selectUnitButton;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private UnitController shipUnit;
    [SerializeField] private UnitType unitType;
    
    private UnitController _unitInTheShip;
    private Home _owner;
    private bool _isCanSelected = true;
    private bool _isSelected;
    private int _moveThisTurn = 1;
    private int _attackThisTurn = 1;
    private int _hp;
    private int _unitIndex;
    private Sequence _moveSeq;
    private Sequence _attSeq;
    private Sequence _counterstrikeSeq;

    public void Init(Home owner, Tile tile, int index)
    {
        transform.SetSiblingIndex(transform.parent.childCount);
        OccupyTile(tile);
        var anchorPos = occupiedTile.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorPos.x, anchorPos.y + 30);
        SetOwner(owner);
        selectUnitButton.onClick.RemoveAllListeners();
        selectUnitButton.onClick.AddListener(SelectUnit);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
        LevelManager.Instance.OnTurnEnd += () =>
        {
            _isCanSelected = false;
        };
        LevelManager.Instance.OnTurnBegin += () =>
        {
            _isCanSelected = true;
            _moveThisTurn = 1;
            _attackThisTurn = 1;
        };

        _unitIndex = index;
        if(unitType == UnitType.Unit)
            unitInfo = owner.owner.civilisationInfo.units[index];
        
        _hp = unitInfo.hp;
        unitHpTMPro.text = _hp.ToString();
    }

    public bool TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0)
        {
            KillUnit();
            return true;
        }
        
        unitHpTMPro.text = _hp.ToString();
        return false;
    }
    
    public bool IsThisUnitAlly(CivilisationController controller)
    {
        return _owner.owner == controller;
    }
    
    public void SetOwner(Home controller)
    {
        _owner = controller;
        headImage.sprite = controller.owner.civilisationInfo.headSprite;
        if (horseImage != null) horseImage.sprite = controller.owner.civilisationInfo.animalSprite;
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

    public void SetUnitInShip(UnitController unitController)
    {
        _unitInTheShip = unitController;
    }
    
    public Tween MoveToTile(Tile to, float dur = 0.2f)
    {
        if(occupiedTile != null)
        {
            occupiedTile.unitOnTile = null;
        }
        
        _moveSeq = DOTween.Sequence();
        aiFromTile = occupiedTile;
        _moveThisTurn = 0;
        var anchorPos = to.GetComponent<RectTransform>().anchoredPosition;
        transform.SetParent(to.transform.parent);
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
        _moveSeq.Join(DOTween.To(() => inValY, x => inValY = x, 1, dur)).OnComplete((() =>
        {
            if (occupiedTile.homeOnTile != null && (occupiedTile.homeOnTile.owner == null || occupiedTile.homeOnTile.owner != _owner.owner))
            {
                occupiedTile.homeOnTile.ShowOccupyButton();
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
                TurnIntoAShip();
            }

            if (unitType == UnitType.Ship && to.tileType == Tile.TileType.Ground)
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
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if (pastO == gameObject)
        {
            DeselectUnit();
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
                if (currO != null && currO.TryGetComponent(out Tile tile))
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
        
        var pos = transform.position;
        DeselectUnit();
        var isThisTheNearestTile = LevelManager.Instance.gameBoardWindow.IsThisTheNearestTile(unitToAttack.occupiedTile, occupiedTile);
        if (unitToAttack.TakeDamage(unitInfo.dmg) && isThisTheNearestTile)
        {
            _attSeq.Append(MoveToTile(unitToAttack.occupiedTile, 0.1f));
            _attackThisTurn = 0; 
            _moveThisTurn = 0;
        }
        else
        {
            _attSeq.Append(transform.DOMove(unitToAttack.transform.position, 0.1f).OnComplete((() =>
            {
                _attackThisTurn = 0; 
                _moveThisTurn = 0;

                LevelManager.Instance.SelectObject(null);
                SelectUnit();

            })));
            _attSeq.Append(transform.DOMove(pos, 0.1f));
            if (_attackThisTurn != 0)
                _attSeq.Append(unitToAttack.Counterstrike(this));
        }
        
        return _attSeq;
    }

    private Tween Counterstrike(UnitController enemyWhoAttack)
    {
        _counterstrikeSeq = DOTween.Sequence();
        
        _counterstrikeSeq.Append(AttackUnitOnTile(enemyWhoAttack));
        return _counterstrikeSeq;
    }

    private void TurnIntoAShip()
    {
        LevelManager.Instance.SelectObject(null);
      
        var ship = Instantiate(shipUnit, transform.parent);
        ship.Init(_owner, occupiedTile, 0);
        ship.SetUnitInShip(this);
        gameObject.SetActive(false);
    }
    
    private void TurnIntoAUnit()
    {
        LevelManager.Instance.SelectObject(null);
        
        _unitInTheShip.gameObject.SetActive(true);
        _unitInTheShip.Init(_owner, occupiedTile, _unitInTheShip._unitIndex);
        occupiedTile.unitOnTile = _unitInTheShip;
        KillUnit();
    }
    
    private void SelectUnit()
    {
        if(!_isCanSelected) return;
        if (_isSelected)
        {
            occupiedTile.SelectTile();
            return;
        }
        
        _isSelected = true;
        LevelManager.Instance.SelectObject(gameObject);
        if(_owner.owner.civilisationInfo.controlType == CivilisationInfo.ControlType.AI) 
            return;

        var gameBoard = LevelManager.Instance.gameBoardWindow;
        var allTile = gameBoard.GetAllTile();
        var closeTilesForMove = gameBoard.GetCloseTile(occupiedTile, unitInfo.moveRad);
        var closeTilesForAttack = gameBoard.GetCloseTile(occupiedTile, unitInfo.rad);
        foreach (var tile in allTile.Select(tile => tile.GetComponent<Tile>()))
        {
            tile.HideTargets();
        }
        foreach (var closeTile in closeTilesForMove)
        {
            if(_moveThisTurn <= 0) break;
            if(closeTile.isHasMountain && !_owner.owner.technologies.Contains(TechInfo.Technology.Mountain))
                continue;
            
            if(unitType != UnitType.Ship && closeTile.tileType == Tile.TileType.Water && !closeTile.IsTileHasPort())
                continue;
            closeTile.ShowBlueTarget();
        }

        foreach (var tile in closeTilesForAttack)
        {
            if(_attackThisTurn <= 0) break;
            if(tile.unitOnTile != null && tile.unitOnTile.IsThisUnitAlly(_owner.owner))
                continue;
            tile.ShowRedTarget();
        }
    }

    private void DeselectUnit()
    {
        _isSelected = false;
    }

    private void KillUnit()
    {
        occupiedTile.unitOnTile = null;
        _owner.RemoveUnit(this);
        Destroy(gameObject);
    }

    public string aiName = "unit1";
    public Home aiHomeExploring;
    public Tile aiFromTile;
}
