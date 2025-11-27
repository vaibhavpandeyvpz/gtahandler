namespace GTAHandler.Models;

public enum LightType
{
    Long = 0,
    Small = 1,
    Big = 2,
    Tall = 3
}

public static class LightTypeExtensions
{
    public static string GetDisplayName(this LightType lightType) => lightType switch
    {
        LightType.Long => "Long (0)",
        LightType.Small => "Small (1)",
        LightType.Big => "Big (2)",
        LightType.Tall => "Tall (3)",
        _ => lightType.ToString()
    };
}


