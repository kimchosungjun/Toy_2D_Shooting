using UnityEngine;

public enum GameTags
{
    Player,
    Enemy,
    Item,
    Bullet
}

public class Tool
{
    public static int rankCount = 10;
    public static string rankKey = "RankKey";
    public static bool isStartingMainScene = false;

    public static string GetTag(GameTags _value)
    {
        return _value.ToString();
    }
}

public class cUserData
{
    public string Name;
    public int Score;
}