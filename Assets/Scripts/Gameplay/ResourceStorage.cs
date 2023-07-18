using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : SingletonPersistent<ResourceStorage>
{
    [SerializeField] private List<GameObject> tileEnvironments;

    public GameObject GetRandomEnvironments()
    {
        var a = Random.Range(0, tileEnvironments.Count);
        Debug.Log(a);
        return tileEnvironments[a];
    }
}
