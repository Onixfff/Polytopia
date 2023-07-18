using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/CreateTile", order = 1)]
public class TileSO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private string name;
    [SerializeField] private string description;

    public Sprite tileSprite => sprite;
    public string tileName => name;
    public string tileDescription => description;
}
