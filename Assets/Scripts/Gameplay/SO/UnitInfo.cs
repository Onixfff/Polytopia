using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/CreateUnitData", order = 1)]

public class UnitInfo : ScriptableObject
{
    public enum AbilityType
    {
        Carry,
        Convert,
        Dash,
        Escape,
        Float,
        Fly,
        Fortify,
        Heal,
        Hide,
        Infiltrate,
        Persist,
        Scout,
        Sneak,
        Surprise,
    }
    
    [SerializeField] private List<AbilityType> abilities;
    public List<AbilityType> abilityTypes => abilities;

    [SerializeField] private float unitHp;
    public float hp => unitHp;
    
    [SerializeField] private float unitDmg;
    public float dmg => unitDmg;

    [SerializeField] private float unitDef;
    public float def => unitDef;
    
    [SerializeField] private int unitMoveRad;
    public int moveRad => unitMoveRad;
    
    [SerializeField] private int unitRad;
    public int rad => unitRad;
    
    [SerializeField] private int priceUnit;
    public int price => priceUnit;
}
