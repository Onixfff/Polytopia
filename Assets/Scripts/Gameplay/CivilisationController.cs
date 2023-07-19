using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilisationController : MonoBehaviour
{
    [SerializeField] private CivilisationInfo civilisationInfo;
    [SerializeField] private List<GameObject> unitPrefabs;
    public Tile _homeTile;
    private GameBoardWindow _gameBoardWindow;
    private void Awake()
    {
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        _homeTile = transform.parent.GetComponent<Tile>();
        SetupCivilisation();
        SpawnUnit(unitPrefabs[0]);
    }

    private void SetupCivilisation()
    {
        var tiles = _gameBoardWindow.GetAllTile().Select(tile => tile.GetComponent<Tile>()).ToList();
        
        foreach (var tile in tiles)
        {
            tile.SetGroundSprite(civilisationInfo.groundSprite);
            var a = Random.Range(0, 13);
            switch (a)
            {
                case 0 or 6: 
                    tile.SetEnvironmentSprite(civilisationInfo.fruitSprite, civilisationInfo.fruitName);
                    break;
                case 1 or 2:
                    tile.SetEnvironmentSprite(civilisationInfo.treeSprite, civilisationInfo.treeName);
                    break;
                case 3:
                    tile.SetEnvironmentSprite(civilisationInfo.treeSprite, civilisationInfo.treeName);
                    tile.SetAnimalSprite(civilisationInfo.animalSprite, civilisationInfo.animalName);
                    break;
                case 4 or 5:
                    tile.SetAnimalSprite(civilisationInfo.animalSprite, civilisationInfo.animalName);
                    break;
                    break;
                default:
                    tile.SetEnvironmentSprite(null, civilisationInfo.groundName);
                    tile.SetAnimalSprite(null, civilisationInfo.groundName);
                    break;
            }
        }
    }

    private void SpawnUnit(GameObject unitPrefab)
    {
        var unit = Instantiate(unitPrefab, _homeTile.transform.parent);
        unit.transform.SetSiblingIndex(unit.transform.parent.childCount);
        var anchorPos = _homeTile.GetComponent<RectTransform>().anchoredPosition;
        unit.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorPos.x, anchorPos.y + 30);
    }
}
