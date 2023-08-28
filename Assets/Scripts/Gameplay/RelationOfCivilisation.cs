using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class RelationOfCivilisation : MonoBehaviour
{
    [SerializeField] private CivilisationController civilisationController;
    
    private Dictionary<CivilisationController, List<DiplomacyManager.OpinionType>> _civilisationOpinions;
    private Dictionary<CivilisationController, DiplomacyManager.RelationType> _civilisationRelation;

    public void AddNewCivilisation(CivilisationController civ)
    {
        _civilisationOpinions.Add(civ, new List<DiplomacyManager.OpinionType>());
        _civilisationRelation.Add(civ, DiplomacyManager.RelationType.Neutral);
    }

    public Dictionary<CivilisationController, List<DiplomacyManager.OpinionType>> GetCivilisationOpinion()
    {
        return _civilisationOpinions;
    }
    
    public Dictionary<CivilisationController, DiplomacyManager.RelationType> GetCivilisationRelation()
    {
        return _civilisationRelation;
    }

    private void Start()
    {
        _civilisationOpinions = new Dictionary<CivilisationController, List<DiplomacyManager.OpinionType>>();
        _civilisationRelation = new Dictionary<CivilisationController, DiplomacyManager.RelationType>();
        foreach (var controller in LevelManager.Instance.GetCivilisationControllers())
        { 
            AddNewCivilisation(controller);
        }
        LevelManager.Instance.OnTurnEnd += UpdateOpinions;
    }

    #region Opinion
    
    private void UpdateOpinions()
    {
        foreach (var civilisationOpinion in _civilisationOpinions)
        {
            civilisationOpinion.Value.Clear();
            civilisationOpinion.Value.AddRange(CheckOpinions(civilisationOpinion.Key));
        }
    }
    
    private List<DiplomacyManager.OpinionType> CheckOpinions(CivilisationController civ)
    {
        var opinions = new List<DiplomacyManager.OpinionType>();
        opinions.AddRange(PositiveOpinions(civ).Where(opinion => !opinions.Contains(opinion)));
        opinions.AddRange(NegativeOpinions(civ).Where(opinion => !opinions.Contains(opinion)));
        return opinions;
    }
    
    private List<DiplomacyManager.OpinionType> PositiveOpinions(CivilisationController controller)
    {
        var positiveOpinions = new List<DiplomacyManager.OpinionType>();

        if (WiseOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Wise);
        }
        if (CharmingOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Charming);
        }
        if (PeacefulOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Peaceful);
        }
        if (DiplomaticOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Diplomatic);
        }
        if (PowerfulOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Powerful);
        }
        if (BraveOpinion())
        {
            positiveOpinions.Add(DiplomacyManager.OpinionType.Brave);
        }
        
        return positiveOpinions;
        
        #region func
        
        bool WiseOpinion()
        {
            if (_civilisationRelation.ContainsValue(DiplomacyManager.RelationType.Peace))
            {
                foreach (var keyValuePair in _civilisationRelation)
                {
                    if (keyValuePair.Value == DiplomacyManager.RelationType.Peace)
                    {
                        if (keyValuePair.Key.relationOfCivilisation.CheckRelation(controller) == DiplomacyManager.RelationType.Peace)
                        {
                            return true;
                        }
                    }
                }
            }
            
            if (_civilisationRelation.ContainsValue(DiplomacyManager.RelationType.War))
            {
                foreach (var keyValuePair in _civilisationRelation)
                {
                    if (keyValuePair.Value == DiplomacyManager.RelationType.War)
                    {
                        if (keyValuePair.Key.relationOfCivilisation.CheckRelation(controller) == DiplomacyManager.RelationType.War)
                        {
                            return true;
                        }
                    }
                }
            }
            
            
            return false;
        } //

        bool CharmingOpinion()
        {
            if(GameManager.Instance.difficult == GameManager.Difficult.Easy)
                return true;
            return false;
        } //

        bool PeacefulOpinion()
        {
            if (civilisationController.TurnWhenIWasAttack.ContainsKey(controller))
            {
                if (LevelManager.Instance.currentTurn - civilisationController.TurnWhenIWasAttack[controller] >= 2)
                {
                    return true;
                }
                return false;
            }

            return true;
        } //

        bool DiplomaticOpinion()
        {
            return false;
        }

        bool PowerfulOpinion()
        {
            if (civilisationController.GetAllUnit().Count < controller.GetAllUnit().Count)
                return true;
            return false;
        } //

        bool BraveOpinion()
        {
            var powerFoolCiv = LevelManager.Instance.GetCivilisationControllers().OrderBy(civ => civ.Point).Last();
            foreach (var keyValuePair in controller.relationOfCivilisation._civilisationRelation)
            {
                if (keyValuePair.Value == DiplomacyManager.RelationType.War && keyValuePair.Key == powerFoolCiv)
                {
                    return true;
                }
            }
            return false;
        } //
      
        #endregion
    }
    
    private List<DiplomacyManager.OpinionType> NegativeOpinions(CivilisationController controller)
    {
        var negativeOpinions = new List<DiplomacyManager.OpinionType>();
        
        if (ViolentOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Violent);
        }
        if (ThreateningOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Threatening);
        }
        if (WeakOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Weak);
        }
        if (AnnoyingOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Annoying);
        }
        if (FoolishOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Foolish);
        }
        if (IntrusiveOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Intrusive);
        }
        if (DominatingOpinion())
        {
            negativeOpinions.Add(DiplomacyManager.OpinionType.Dominating);
        }
        
        return negativeOpinions;

        #region MyRegion

        bool ViolentOpinion()
        {
            if (civilisationController.TurnWhenIWasAttack.ContainsKey(controller))
            {
                if (LevelManager.Instance.currentTurn - civilisationController.TurnWhenIWasAttack[controller] < 2)
                {
                    return true;
                }
                return false;
            }

            return false;
        } //

        bool ThreateningOpinion()
        {
            var board = LevelManager.Instance.gameBoardWindow;
            var allTile = board.GetAllTile().Values.ToList();
            var controlledTiles = allTile.FindAll(tile => tile.GetOwner() != null && tile.GetOwner().owner == civilisationController);
            foreach (var tile in controlledTiles)
            {
                var close = board.GetCloseTile(tile, 1);
                if (close.Any(til => til.unitOnTile != null && til.unitOnTile.GetOwner().owner == controller))
                {
                    return true;
                }
            }
            
            return false;
        } //

        bool WeakOpinion()
        {
            if (civilisationController.GetAllUnit().Count >= controller.GetAllUnit().Count)
                return true;
            return false;
        } //

        bool AnnoyingOpinion()
        {
            if(GameManager.Instance.difficult == GameManager.Difficult.Hard)
                return true;
            return false;
        } //

        bool FoolishOpinion()
        {
            if (_civilisationRelation.ContainsValue(DiplomacyManager.RelationType.Peace))
            {
                foreach (var keyValuePair in _civilisationRelation)
                {
                    if (keyValuePair.Value == DiplomacyManager.RelationType.Peace)
                    {
                        if (keyValuePair.Key.relationOfCivilisation.CheckRelation(controller) == DiplomacyManager.RelationType.War)
                        {
                            return true;
                        }
                    }
                }
            }
            
            if (_civilisationRelation.ContainsValue(DiplomacyManager.RelationType.War))
            {
                foreach (var keyValuePair in _civilisationRelation)
                {
                    if (keyValuePair.Value == DiplomacyManager.RelationType.War)
                    {
                        if (keyValuePair.Key.relationOfCivilisation.CheckRelation(controller) == DiplomacyManager.RelationType.Peace)
                        {
                            return true;
                        }
                    }
                }
            }
            
            
            return false;
        } //

        bool IntrusiveOpinion()
        {
            var board = LevelManager.Instance.gameBoardWindow;
            var allTile = board.GetAllTile().Values.ToList();
            var controlledTiles = allTile.FindAll(tile => tile.GetOwner() != null && tile.GetOwner().owner == civilisationController);
            foreach (var tile in controlledTiles)
            {
                var close = board.GetCloseTile(tile, 1);
                if (close.Any(til => til.GetOwner() != null && til.GetOwner().owner == controller))
                {
                    return true;
                }
            }
            
            return false;
        } //
        
        bool DominatingOpinion()
        {
            if (LevelManager.Instance.GetCivilisationControllers().Count == 2 && controller.GetAllUnit().Count > civilisationController.GetAllUnit().Count)
            {
                return true;
            }

            return false;
        } //

        #endregion
    }
    
    private int CalculateOpinion(CivilisationController civ)
    {
        var opinionValue = 0;
        var opinionsValues = _civilisationOpinions[civ];
        foreach (var value in opinionsValues)
        {
            switch (value)
            {
                case DiplomacyManager.OpinionType.Wise:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Charming:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Peaceful:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Diplomatic:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Powerful:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Brave:
                    opinionValue++;
                    break;
                case DiplomacyManager.OpinionType.Violent:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Threatening:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Weak:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Annoying:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Foolish:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Intrusive:
                    opinionValue--;
                    break;
                case DiplomacyManager.OpinionType.Dominating:
                    opinionValue--;
                    break;
            }
        }
        return opinionValue;
    }
    
    #endregion

    #region Relation

    public void SetRelation(CivilisationController controller, DiplomacyManager.RelationType type)
    {
        _civilisationRelation[controller] = type;
        DiplomacyManager.Instance.OnRelationChange?.Invoke();
    }
    
    public DiplomacyManager.RelationType GetRelation(CivilisationController controller)
    {
        return _civilisationRelation[controller];
    }
    
    private DiplomacyManager.RelationType CheckRelation(CivilisationController civ)
    {
        var opinions = DiplomacyManager.RelationType.Neutral;
        return opinions;
    }
    
    #endregion


    [Button()]
    private void ShowAllRelationAndOpinion()
    {
        foreach (var controller in LevelManager.Instance.GetCivilisationControllers())
        {
            if(controller != civilisationController)
                Debug.Log("Civ: " + controller.civilName + " Relation: " + _civilisationRelation[controller] + " Count opinion: " + _civilisationOpinions[controller].Count);
        }
    }
}
