namespace GTAHandler.Models;

public enum EngineType
{
    Petrol,   // P
    Diesel,   // D
    Electric  // E
}

public static class EngineTypeExtensions
{
    public static char ToChar(this EngineType engineType) => engineType switch
    {
        EngineType.Petrol => 'P',
        EngineType.Diesel => 'D',
        EngineType.Electric => 'E',
        _ => 'P'
    };

    public static EngineType FromChar(char c) => char.ToUpper(c) switch
    {
        'P' => EngineType.Petrol,
        'D' => EngineType.Diesel,
        'E' => EngineType.Electric,
        _ => EngineType.Petrol
    };

    public static string GetDisplayName(this EngineType engineType) => engineType switch
    {
        EngineType.Petrol => "Petrol (P)",
        EngineType.Diesel => "Diesel (D)",
        EngineType.Electric => "Electric (E)",
        _ => engineType.ToString()
    };
}


