using System;
using UnityEngine;

namespace Gameplay
{
    public class Tile : MonoBehaviour
    {
        public Vector2Int pos;
        public bool obstacle;
        public float gCost = float.PositiveInfinity;
        public float hCost = float.PositiveInfinity;
        public float fCost = float.PositiveInfinity;
        public Tile previousTile;
    
        public void CalculateCost(Tile startTile, Tile targetTile)
        {
            gCost = MathF.Abs(pos.x - startTile.pos.x) + Mathf.Abs(pos.y - startTile.pos.y);
            hCost = MathF.Abs(pos.x - targetTile.pos.x) + Mathf.Abs(pos.y - targetTile.pos.y);
            fCost = gCost + hCost;
        }
    }
}