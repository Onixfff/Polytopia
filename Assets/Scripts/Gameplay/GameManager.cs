using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum Difficult
    {
        Easy,
        Normal,
        Hard
    }

    public Difficult difficult;
    
    public enum GameMode
    {
        Classic,
        Occupy,
        Endless
    }

    public GameMode gameMode;

    public int mapSize;
    public int countEnemy;
    
    public bool isHasTurnLimit;

    public CivilisationInfo playerCivInfo;
}
