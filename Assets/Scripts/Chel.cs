using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Chel : MonoBehaviour
{
    public Tile tile;
    
    [Button()]
    private void Go()
    {
        var allTiles = AstarTest.Instance.GeneratedTiles;
        var allPos = allTiles.Keys.ToList();
        foreach (var pos in allPos)
        {
            if (allTiles[pos].obstacle)
            {
                allTiles[pos].GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }
            else
            {
                allTiles[pos].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
        }

        allTiles.Values.ToList().RemoveAll(tile => tile.obstacle);
        var randPos = Random.Range(0, allPos.Count);
        var a = allTiles[allPos[randPos]];
        
        allPos.Remove(allPos[randPos]);
        randPos = Random.Range(0, allPos.Count);
        var b = AstarTest.Instance.GeneratedTiles[allPos[randPos]];
        while (b.obstacle)
        {
            randPos = Random.Range(0, allPos.Count);
            b = AstarTest.Instance.GeneratedTiles[allPos[randPos]];
        }
        a.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        b.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        Debug.Log("From - " + a.pos + "To - " + b.pos);
        var path = AStarAlgorithm.FindPath(a.pos, b.pos);
        if(path == null)
            Debug.Log("lox");
        foreach (var pat in path)
        {
            allTiles[pat].GetComponent<Renderer>().material.SetColor("_Color", Color.blue);

            Debug.Log(pat);
        }
    }
}
