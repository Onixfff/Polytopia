using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.SO
{
    [CreateAssetMenu(fileName = "TechData", menuName = "ScriptableObjects/CreateTechData", order = 0)]
    public class TechInfo : ScriptableObject
    {
        public enum Technology
        {
            Mountain,
            Hunt,
            Rider,
            Fish,
            Gather,
            FreeSpirit,
            Farming,
            Strategy,
            Mining,
            Archery,
            Sailing,
            Chivalry,
            Diplomacy,
            Forge,
            Philosophy,
            Mathematics,
            Meditation
        }

        [SerializeField] private List<Technology> technologiesList;
        public List<Technology> startTechnologies => technologiesList;
    }
}