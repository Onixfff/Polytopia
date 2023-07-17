using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> generatedTiles;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int mapSize = 16;

#if UNITY_EDITOR
#endif
    [Button()]
    private void GenerateMapInEditor()
    {
        var height = mapSize / Mathf.Sqrt(mapSize);
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < height; j++)
            {
                 var tile = Instantiate(tilePrefab, transform);
                 tile.transform.position = new Vector3(i, 0, j);
                 generatedTiles.Add(tile);
            }
        }
    }
}