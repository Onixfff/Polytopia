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
    
    [SerializeField] private string civilName;
    public string civilisationName => civilName;
    
    [SerializeField] private Color civilColor;
    public Color civilisationColor => civilColor;
    
    [SerializeField] private Sprite head;
    public Sprite headSprite => head;
    
    [SerializeField] private Sprite ground;
    public Sprite groundSprite => ground;
    public string groundName => GroundName;
    
    [SerializeField] private Sprite fruit;
    public Sprite fruitSprite => fruit;
    public string fruitName => FruitName;
    
    [SerializeField] private Sprite tree;
    public Sprite treeSprite => tree;
    public string treeName => TreeName;

    [SerializeField] private Sprite animal;
    public Sprite animalSprite => animal;
    public string animalName => AnimalName;
    
    [SerializeField] private Sprite mountain;
    public Sprite mountainSprite => mountain;
    public string mountainName => MountainName;
    
    [SerializeField] private Sprite mining;
    public Sprite miningSprite => mining;
    
    [SerializeField] private Sprite church;
    public Sprite churchSprite => church;
    
    
    [SerializeField] private Sprite port;
    public Sprite portSprite => port;
    
    [SerializeField] private Sprite farm;
    public Sprite farmSprite => farm;

    [SerializeField] private HomeInfo homeInfo;
    public HomeInfo home => homeInfo;
    
    
    [SerializeField] private List<UnitInfo> unitInfo;
    public List<UnitInfo> units => unitInfo;
    
    [SerializeField] private List<UnitInfo> shipUnitInfo;
    public List<UnitInfo> ships => shipUnitInfo;
    
    [SerializeField] private TechInfo techInfo;
    public TechInfo technology => techInfo;
}
