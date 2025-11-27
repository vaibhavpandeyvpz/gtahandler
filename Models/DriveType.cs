namespace GTAHandler.Models;

public enum DriveType
{
    Front,  // F
    Rear,   // R
    FourWheel // 4
}

public static class DriveTypeExtensions
{
    public static char ToChar(this DriveType driveType) => driveType switch
    {
        DriveType.Front => 'F',
        DriveType.Rear => 'R',
        DriveType.FourWheel => '4',
        _ => 'R'
    };

    public static DriveType FromChar(char c) => char.ToUpper(c) switch
    {
        'F' => DriveType.Front,
        'R' => DriveType.Rear,
        '4' => DriveType.FourWheel,
        _ => DriveType.Rear
    };

    public static string GetDisplayName(this DriveType driveType) => driveType switch
    {
        DriveType.Front => "Front (F)",
        DriveType.Rear => "Rear (R)",
        DriveType.FourWheel => "4-Wheel (4)",
        _ => driveType.ToString()
    };
}


