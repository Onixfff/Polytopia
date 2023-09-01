using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.SO
{
    [CreateAssetMenu(fileName = "TechData", menuName = "ScriptableObjects/CreateTechData", order = 0)]
    public class TechInfo : ScriptableObject
    {
        public enum Technology
        {
            Rider,
            Gather,
            Mountain,
            Fish,
            Hunt,
            Roads,
            FreeSpirit,
            Farming,
            Strategy,
            Mining,
            Meditation,
            Sailing,
            Whaling,
            Archery,
            Forestry,
            Trade,
            Chivalry,
            Construction,
            Diplomacy,
            Forge,
            Philosophy,
            Navigation,
            Aqua,
            Spiritualism,
            Mathematics,
        }

        [SerializeField] private List<Technology> technologiesList;
        public List<Technology> startTechnologies => technologiesList;
    }
}