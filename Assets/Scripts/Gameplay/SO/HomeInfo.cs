using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeData", menuName = "ScriptableObjects/CreateHomeData", order = 0)] 
public class HomeInfo : ScriptableObject
{
    [SerializeField] private List<Sprite> homes;
    public List<Sprite> homeSprites => homes;
}
