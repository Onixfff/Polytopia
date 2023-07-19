using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileEnvironmentData", menuName = "ScriptableObjects/CreateTileEnvironment", order = 1)]
public class CivilisationInfo : BaseCivilisationInfo
{
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
    
    [SerializeField] private List<Sprite> homes;
    public List<Sprite> homeSprites => homes;
}
