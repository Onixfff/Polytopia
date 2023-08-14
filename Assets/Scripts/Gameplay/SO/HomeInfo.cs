using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeData", menuName = "ScriptableObjects/CreateHomeData", order = 0)] 
public class HomeInfo : ScriptableObject
{
    [SerializeField] private List<Sprite> homes;
    public List<Sprite> homeSprites => homes;
    
    [SerializeField] private Sprite capitalHomePrefab;
    [SerializeField] private Sprite home1Prefab;
    [SerializeField] private Sprite home2Prefab;
    
    public Sprite CapitalHomePrefab => capitalHomePrefab;
    public Sprite Home1Prefab => home1Prefab;
    public Sprite Home2Prefab => home2Prefab;
}
