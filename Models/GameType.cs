namespace GTAHandler.Models;

public enum GameType
{
    GTA3,
    GTAVC,
    GTASA
}

public static class GameTypeExtensions
{
    public static string GetDisplayName(this GameType gameType, bool isDefinitiveEdition = false) => gameType switch
    {
        GameType.GTA3 => isDefinitiveEdition ? "GTA:III DE" : "GTA:III",
        GameType.GTAVC => isDefinitiveEdition ? "GTA:VC DE" : "GTA:VC",
        GameType.GTASA => isDefinitiveEdition ? "GTA:SA DE" : "GTA:SA",
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


