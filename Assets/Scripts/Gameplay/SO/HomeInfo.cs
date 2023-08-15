using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeData", menuName = "ScriptableObjects/CreateHomeData", order = 0)] 
public class HomeInfo : ScriptableObject
{
    [SerializeField] private Sprite capitalHomeSprite;
    [SerializeField] private Sprite forgeHomeSprite;
    [SerializeField] private Sprite parkHomeSprite;
    [SerializeField] private Sprite home1Sprite;
    [SerializeField] private Sprite home2Sprite;
    
    public Sprite CapitalHomeSprite => capitalHomeSprite;
    public Sprite ForgeHomeSprite => forgeHomeSprite;
    public Sprite ParkHomeSprite => parkHomeSprite;
    public Sprite Home1Sprite => home1Sprite;
    public Sprite Home2Sprite => home2Sprite;
}
