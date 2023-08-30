
using System;
using JetBrains.Annotations;

public class DiplomacyManager : Singleton<DiplomacyManager>
{
    public Action OnRelationChange;
    
    public enum RelationType
    {
        None,
        War,
        Neutral,
        Peace
    }

    public enum OpinionType
    {
        Wise,
        Charming,
        Peaceful,
        Diplomatic,
        Powerful,
        Brave,
        Violent,
        Threatening,
        Weak,
        Annoying,
        Foolish,
        Intrusive,
        Dominating,
        Indifferent
    }
}
