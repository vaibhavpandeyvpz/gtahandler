namespace GTAHandler.Models;

public enum GameType
{
    GTA3,
    GTAVC,
    GTASA
}

public static class GameTypeExtensions
{
    public static string GetDisplayName(this GameType gameType) => gameType switch
    {
        GameType.GTA3 => "GTA:III",
        GameType.GTAVC => "GTA:VC",
        GameType.GTASA => "GTA:SA",
        _ => gameType.ToString()
    };

    public static string GetFullName(this GameType gameType) => gameType switch
    {
        GameType.GTA3 => "GTA III",
        GameType.GTAVC => "GTA: Vice City",
        GameType.GTASA => "GTA: San Andreas",
        _ => gameType.ToString()
    };
}


