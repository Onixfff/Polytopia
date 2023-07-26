using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/CreateUnitData", order = 1)]

public class UnitInfo : ScriptableObject
{
    [SerializeField] private int unitHp;
    public int hp => unitHp;
    
    [SerializeField] private int unitDmg;
    public int dmg => unitDmg;

    [SerializeField] private int unitDef;
    public int def => unitDef;
    
    [SerializeField] private int unitMoveRad;
    public int moveRad => unitMoveRad;
    
    [SerializeField] private int unitRad;
    public int rad => unitRad;
    
    [SerializeField] private int priceUnit;
    public int price => priceUnit;
}
