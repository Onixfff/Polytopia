using System.Collections.Generic;
using Gameplay.SO;
using UnityEngine;

[CreateAssetMenu(fileName = "CivilisationData", menuName = "ScriptableObjects/CreateCivilisationData", order = 1)]
public class CivilisationInfo : BaseCivilisationInfo
{
    public enum ControlType
    {
        Player,
        AI
    }

    [SerializeField] private ControlType control;
    public ControlType controlType => control;
    
    public enum CivilisationType
    {
        China,
        Imperium,
        Bardur,
        Oumaji,
        Kickoo,
        Hoodrick
    }
    
    [SerializeField] private CivilisationType civilType;
    public CivilisationType civilisationType => civilType;
    
    [SerializeField] private Color civilColor;
    public Color CivilisationColor => civilColor;
    
    [SerializeField] private Sprite head;
    public Sprite HeadSprite => head;
    
    [SerializeField] private Sprite battleShipHead;
    public Sprite BattleShipHeadSprite => battleShipHead;
    
    [SerializeField] private Sprite ground;
    public Sprite GroundSprite => ground;
    public string groundName => GroundName;
    
    [SerializeField] private Sprite fruit;
    public Sprite FruitSprite => fruit;
    public string fruitName => FruitName;
    
    [SerializeField] private Sprite tree;
    public Sprite TreeSprite => tree;
    public string treeName => TreeName;

    [SerializeField] private Sprite animal;
    public Sprite AnimalSprite => animal;
    public string animalName => AnimalName;
    
    [SerializeField] private Sprite mountain;
    public Sprite MountainSprite => mountain;
    public string mountainName => MountainName;
    
    [SerializeField] private List<Sprite> buildSprites;
    public List<Sprite> BuildSprites => buildSprites;
    
    [SerializeField] private List<Sprite> monumentSprites;
    public List<Sprite> MonumentSprites => monumentSprites;
    
    [SerializeField] private Sprite port;
    public Sprite PortSprite => port;
    
    [SerializeField] private Sprite farm;
    public Sprite FarmSprite => farm;

    [SerializeField] private HomeInfo homeInfo;
    public HomeInfo Home => homeInfo;
    
    [SerializeField] private List<UnitInfo> unitInfo;
    public List<UnitInfo> Units => unitInfo;
    
    [SerializeField] private List<UnitInfo> shipUnitInfo;
    public List<UnitInfo> Ships => shipUnitInfo;
    
    [SerializeField] private int startUnitIndex;
    public int StartUnitIndex => startUnitIndex;

    [SerializeField] private TechInfo techInfo;
    public TechInfo technology => techInfo;
}
