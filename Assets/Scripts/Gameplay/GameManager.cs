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
}
