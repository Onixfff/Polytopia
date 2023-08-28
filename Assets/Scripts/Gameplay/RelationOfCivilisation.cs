using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RelationOfCivilisation : MonoBehaviour
{
    [SerializeField] private CivilisationController civilisationController;
    
    private Dictionary<CivilisationController, List<DiplomacyManager.OpinionType>> _civilisationOpinions;
    private Dictionary<CivilisationController, DiplomacyManager.RelationType> _civilisationRelation;
    
    public void AddNewCivilisation(CivilisationController civ)
    {
        if (!_civilisationOpinions.ContainsKey(civ))
        {
            _civilisationOpinions.Add(civ, CheckOpinions(civ));
            _civilisationRelation.Add(civ, CheckRelation(civ));
        }
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
        AddNewCivilisation(civilisationController);
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
            return true;
        }

        bool CharmingOpinion()
        {
            return true;
    
        }

        bool PeacefulOpinion()
        {
            return true;

        }

        bool DiplomaticOpinion()
        {
            return true;

        }

        bool PowerfulOpinion()
        {
            return true;

        }

        bool BraveOpinion()
        {
            return true;

        }
      
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
            return true;
   
        }

        bool ThreateningOpinion()
        {
            return true;
        
        }

        bool WeakOpinion()
        {
            return true;
        
        }

        bool AnnoyingOpinion()
        {
            return true;
        
        }

        bool FoolishOpinion()
        {
            return true;
        
        }

        bool IntrusiveOpinion()
        {
            return true;
        
        }
        
        bool DominatingOpinion()
        {
            return true;
        
        }

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
}
