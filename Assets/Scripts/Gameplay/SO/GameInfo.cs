using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/CreateGameData", order = 1)]
public class GameInfo : ScriptableObject
{
    [SerializeField] private List<CivilisationInfo> civilisationInfos;
    [SerializeField] private List<CivilisationInfo> playerCivilisationInfos;
    public List<CivilisationInfo> civilisationInfoLists => civilisationInfos;
    public List<CivilisationInfo> playerCivilisationInfoLists => playerCivilisationInfos;
}