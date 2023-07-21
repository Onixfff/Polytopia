using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CivilisationController : MonoBehaviour
{
    [SerializeField] private CivilisationInfo civilisationInfo;
    [SerializeField] private Button civilisationButton;
    [SerializeField] private List<GameObject> unitPrefabs;
    public Tile homeTile;
    private GameBoardWindow _gameBoardWindow;
    private void Start()
    {
        _gameBoardWindow = LevelManager.Instance.gameBoardWindow;
        civilisationButton.onClick.AddListener(CivilisationOnClick);
        homeTile = transform.parent.GetComponent<Tile>();
        SetupCivilisation();
        SpawnUnit(unitPrefabs[0]);
        LevelManager.Instance.gameplayWindow.OnUnitSpawn += () =>
        {
            SpawnUnit(unitPrefabs[0]);
        };
    }

    private void CivilisationOnClick()
    {
        homeTile.SelectTile();
        LevelManager.Instance.gameplayWindow.ShowCivilisationButton();
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
        if(!homeTile.IsTileFree()) 
            return;
        
        var unitObject = Instantiate(unitPrefab, homeTile.transform.parent);
        unitObject.transform.SetSiblingIndex(unitObject.transform.parent.childCount);
        var anchorPos = homeTile.GetComponent<RectTransform>().anchoredPosition;
        unitObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorPos.x, anchorPos.y + 30);
        
        var unit = unitObject.GetComponent<UnitController>();
        unit.OccupyTile(homeTile);
    }
}
