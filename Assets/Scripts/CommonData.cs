public enum GameTags
{
    Player,
    Enemy,
    Item,
    Bullet
}

public class Tool
{
    public static string GetTag(GameTags _value)
    {
        return _value.ToString();
    }


}