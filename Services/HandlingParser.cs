using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GTAHandler.Models;

namespace GTAHandler.Services;

public class HandlingParser
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public (List<VehicleHandling> Vehicles, List<string> RawLines) ParseFile(string filePath, GameType gameType)
    {
        var vehicles = new List<VehicleHandling>();
        var rawLines = new List<string>();

        // Check if file exists
        if (!File.Exists(filePath))
        {
            throw HandlingParseException.FileNotFound(filePath);
        }

        // Try to read the file
        string[] lines;
        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw HandlingParseException.FileUnreadable(filePath, ex);
        }
        catch (IOException ex)
        {
            throw HandlingParseException.FileUnreadable(filePath, ex);
        }

        // Check if file is empty
        if (lines.Length == 0)
        {
            throw HandlingParseException.FileEmpty(filePath);
        }

        int parseErrors = 0;
        int? firstDataLineColumns = null;
        string? wrongFormatDetails = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            rawLines.Add(line);

            // Skip empty lines, comments, and special sections (boats, bikes, planes, anim groups)
            if (string.IsNullOrWhiteSpace(line) ||
                line.TrimStart().StartsWith(';') ||
                line.TrimStart().StartsWith('%') ||
                line.TrimStart().StartsWith('!') ||
                line.TrimStart().StartsWith('$') ||
                line.TrimStart().StartsWith('^'))
            {
                continue;
            }

            try
            {
                var vehicle = ParseVehicleLine(line, gameType, i + 1, out int columnCount, out string? formatError);

                // Track first data line column count for format validation
                firstDataLineColumns ??= columnCount;

                if (formatError != null)
                {
                    wrongFormatDetails = formatError;
                }

                if (vehicle != null)
                {
                    vehicles.Add(vehicle);
                }
                else
                {
                    parseErrors++;
                }
            }
            catch (Exception ex)
            {
                parseErrors++;
                System.Diagnostics.Debug.WriteLine($"Error parsing line {i + 1}: {ex.Message}");
            }
        }

        // Check if we found any vehicles
        if (vehicles.Count == 0)
        {
            // Check if it might be a wrong game format
            if (wrongFormatDetails != null)
            {
                throw HandlingParseException.InvalidFormat(wrongFormatDetails);
            }

            // Check if there were only comments
            bool hasOnlyComments = rawLines.All(l =>
                string.IsNullOrWhiteSpace(l) ||
                l.TrimStart().StartsWith(';') ||
                l.TrimStart().StartsWith('%'));

            if (hasOnlyComments)
            {
                throw HandlingParseException.FileEmpty(filePath);
            }

            throw HandlingParseException.NoVehiclesFound();
        }

        return (vehicles, rawLines);
    }

    private int GetExpectedColumnCount(GameType gameType) => gameType switch
    {
        GameType.GTA3 => 32,
        GameType.GTAVC => 33,
        GameType.GTASA => 36,
        _ => 30
    };

    private string GetGameName(GameType gameType) => gameType switch
    {
        GameType.GTA3 => "GTA III",
        GameType.GTAVC => "GTA: Vice City",
        GameType.GTASA => "GTA: San Andreas",
        _ => "GTA"
    };

    private VehicleHandling? ParseVehicleLine(string line, GameType gameType, int lineNumber,
        out int columnCount, out string? formatError)
    {
        formatError = null;

        // Split by whitespace and filter empty entries
        var parts = WhitespaceRegex.Split(line.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToArray();

        columnCount = parts.Length;

        if (parts.Length < 20) // Minimum fields required
            return null;

        // Check if column count matches expected for game type
        int expectedMin = gameType switch
        {
            GameType.GTA3 => 31,
            GameType.GTAVC => 32,
            GameType.GTASA => 35,
            _ => 30
        };

        int expectedMax = gameType switch
        {
            GameType.GTA3 => 32,
            GameType.GTAVC => 33,
            GameType.GTASA => 38,
            _ => 40
        };

        if (parts.Length < expectedMin || parts.Length > expectedMax)
        {
            // Try to detect which game this file is actually for
            string? detectedGame = parts.Length switch
            {
                >= 31 and <= 32 => "GTA III",
                >= 32 and <= 33 => "GTA: Vice City",
                >= 35 and <= 38 => "GTA: San Andreas",
                _ => null
            };

            if (detectedGame != null && detectedGame != GetGameName(gameType))
            {
                formatError = $"This appears to be a {detectedGame} file (has {parts.Length} columns), " +
                             $"but you selected {GetGameName(gameType)}.";
            }
            else
            {
                formatError = $"Line has {parts.Length} columns, expected {expectedMin}-{expectedMax} for {GetGameName(gameType)}.";
            }
        }

        var vehicle = new VehicleHandling
        {
            GameType = gameType,
            RawLine = line,
            LineNumber = lineNumber
        };

        try
        {
            switch (gameType)
            {
                case GameType.GTA3:
                    ParseGTA3Line(parts, vehicle);
                    break;
                case GameType.GTAVC:
                    ParseGTAVCLine(parts, vehicle);
                    break;
                case GameType.GTASA:
                    ParseGTASALine(parts, vehicle);
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Parse error: {ex.Message}");
            formatError ??= $"Error parsing data: {ex.Message}";
            return null;
        }

        return vehicle;
    }

    private void ParseGTA3Line(string[] parts, VehicleHandling v)
    {
        // GTA3: 31 fields
        // A B C D E F G H I J K L M N O P Q R S T U V W X Y Z aa ab ac ad ae af
        int i = 0;
        v.Identifier = parts[i++];                          // A
        v.Mass = ParseFloat(parts[i++]);                    // B
        v.TurnMassOrDimensionX = ParseFloat(parts[i++]);    // C - Dimensions.x
        v.DragMultOrDimensionY = ParseFloat(parts[i++]);    // D - Dimensions.y
        v.DimensionZ = ParseFloat(parts[i++]);              // E - Dimensions.z
        v.CentreOfMassX = ParseFloat(parts[i++]);           // F
        v.CentreOfMassY = ParseFloat(parts[i++]);           // G
        v.CentreOfMassZ = ParseFloat(parts[i++]);           // H
        v.PercentSubmerged = ParseInt(parts[i++]);          // I
        v.TractionMultiplier = ParseFloat(parts[i++]);      // J
        v.TractionLoss = ParseFloat(parts[i++]);            // K
        v.TractionBias = ParseFloat(parts[i++]);            // L
        v.NumberOfGears = ParseInt(parts[i++]);             // M
        v.MaxVelocity = ParseFloat(parts[i++]);             // N
        v.EngineAcceleration = ParseFloat(parts[i++]);      // O
        v.DriveType = DriveTypeExtensions.FromChar(parts[i++][0]); // P
        v.EngineType = EngineTypeExtensions.FromChar(parts[i++][0]); // Q
        v.BrakeDeceleration = ParseFloat(parts[i++]);       // R
        v.BrakeBias = ParseFloat(parts[i++]);               // S
        v.HasABS = parts[i++] == "1";                       // T
        v.SteeringLock = ParseFloat(parts[i++]);            // U
        v.SuspensionForceLevel = ParseFloat(parts[i++]);    // V
        v.SuspensionDampingLevel = ParseFloat(parts[i++]);  // W
        v.SeatOffsetDistance = ParseFloat(parts[i++]);      // X
        v.CollisionDamageMultiplier = ParseFloat(parts[i++]); // Y
        v.MonetaryValue = ParseInt(parts[i++]);             // Z
        v.SuspensionUpperLimit = ParseFloat(parts[i++]);    // aa
        v.SuspensionLowerLimit = ParseFloat(parts[i++]);    // ab
        v.SuspensionBias = ParseFloat(parts[i++]);          // ac
        v.ModelFlags = parts[i++];                          // ad (flags)
        v.FrontLights = (LightType)ParseInt(parts[i++]);    // ae
        v.RearLights = (LightType)ParseInt(parts[i++]);     // af
    }

    private void ParseGTAVCLine(string[] parts, VehicleHandling v)
    {
        // GTAVC: 32 fields (added suspension anti-dive multiplier)
        // A B C D E F G H I J K L M N O P Q R S T U V W X Y Z aa ab ac ad ae af ag
        int i = 0;
        v.Identifier = parts[i++];                          // A
        v.Mass = ParseFloat(parts[i++]);                    // B
        v.TurnMassOrDimensionX = ParseFloat(parts[i++]);    // C - Dimensions.x
        v.DragMultOrDimensionY = ParseFloat(parts[i++]);    // D - Dimensions.y
        v.DimensionZ = ParseFloat(parts[i++]);              // E - Dimensions.z
        v.CentreOfMassX = ParseFloat(parts[i++]);           // F
        v.CentreOfMassY = ParseFloat(parts[i++]);           // G
        v.CentreOfMassZ = ParseFloat(parts[i++]);           // H
        v.PercentSubmerged = ParseInt(parts[i++]);          // I
        v.TractionMultiplier = ParseFloat(parts[i++]);      // J
        v.TractionLoss = ParseFloat(parts[i++]);            // K
        v.TractionBias = ParseFloat(parts[i++]);            // L
        v.NumberOfGears = ParseInt(parts[i++]);             // M
        v.MaxVelocity = ParseFloat(parts[i++]);             // N
        v.EngineAcceleration = ParseFloat(parts[i++]);      // O
        v.DriveType = DriveTypeExtensions.FromChar(parts[i++][0]); // P
        v.EngineType = EngineTypeExtensions.FromChar(parts[i++][0]); // Q
        v.BrakeDeceleration = ParseFloat(parts[i++]);       // R
        v.BrakeBias = ParseFloat(parts[i++]);               // S
        v.HasABS = parts[i++] == "1";                       // T
        v.SteeringLock = ParseFloat(parts[i++]);            // U
        v.SuspensionForceLevel = ParseFloat(parts[i++]);    // V
        v.SuspensionDampingLevel = ParseFloat(parts[i++]);  // W
        v.SeatOffsetDistance = ParseFloat(parts[i++]);      // X
        v.CollisionDamageMultiplier = ParseFloat(parts[i++]); // Y
        v.MonetaryValue = ParseInt(parts[i++]);             // Z
        v.SuspensionUpperLimit = ParseFloat(parts[i++]);    // aa
        v.SuspensionLowerLimit = ParseFloat(parts[i++]);    // ab
        v.SuspensionBias = ParseFloat(parts[i++]);          // ac
        v.SuspensionAntiDiveMultiplier = ParseFloat(parts[i++]); // ad
        v.ModelFlags = parts[i++];                          // ae (flags)
        v.FrontLights = (LightType)ParseInt(parts[i++]);    // af
        v.RearLights = (LightType)ParseInt(parts[i++]);     // ag
    }

    private void ParseGTASALine(string[] parts, VehicleHandling v)
    {
        // GTASA: 36+ fields
        // A B C D F G H I J K L M N O P Q R S T U V a b c d e f g aa ab ac af ag ah ai aj
        // Note: Column E is skipped (not used) but we need to handle varying column counts
        int i = 0;
        v.Identifier = parts[i++];                          // A
        v.Mass = ParseFloat(parts[i++]);                    // B
        v.TurnMassOrDimensionX = ParseFloat(parts[i++]);    // C - fTurnMass
        v.DragMultOrDimensionY = ParseFloat(parts[i++]);    // D - fDragMult
        // E is not used in SA (skip if exists, but actually it's not in the file)
        v.CentreOfMassX = ParseFloat(parts[i++]);           // F
        v.CentreOfMassY = ParseFloat(parts[i++]);           // G
        v.CentreOfMassZ = ParseFloat(parts[i++]);           // H
        v.PercentSubmerged = ParseInt(parts[i++]);          // I
        v.TractionMultiplier = ParseFloat(parts[i++]);      // J
        v.TractionLoss = ParseFloat(parts[i++]);            // K
        v.TractionBias = ParseFloat(parts[i++]);            // L
        v.NumberOfGears = ParseInt(parts[i++]);             // M
        v.MaxVelocity = ParseFloat(parts[i++]);             // N
        v.EngineAcceleration = ParseFloat(parts[i++]);      // O
        v.EngineInertia = ParseFloat(parts[i++]);           // P (SA-specific)
        v.DriveType = DriveTypeExtensions.FromChar(parts[i++][0]); // Q
        v.EngineType = EngineTypeExtensions.FromChar(parts[i++][0]); // R
        v.BrakeDeceleration = ParseFloat(parts[i++]);       // S
        v.BrakeBias = ParseFloat(parts[i++]);               // T
        v.HasABS = parts[i++] == "1";                       // U
        v.SteeringLock = ParseFloat(parts[i++]);            // V
        v.SuspensionForceLevel = ParseFloat(parts[i++]);    // a
        v.SuspensionDampingLevel = ParseFloat(parts[i++]);  // b
        v.SuspensionHighSpeedComDamp = ParseFloat(parts[i++]); // c (SA-specific)
        v.SuspensionUpperLimit = ParseFloat(parts[i++]);    // d
        v.SuspensionLowerLimit = ParseFloat(parts[i++]);    // e
        v.SuspensionBias = ParseFloat(parts[i++]);          // f
        v.SuspensionAntiDiveMultiplier = ParseFloat(parts[i++]); // g
        v.SeatOffsetDistance = ParseFloat(parts[i++]);      // aa
        v.CollisionDamageMultiplier = ParseFloat(parts[i++]); // ab
        v.MonetaryValue = ParseInt(parts[i++]);             // ac
        v.ModelFlags = parts[i++];                          // af
        v.HandlingFlags = parts[i++];                       // ag (SA-specific)
        v.FrontLights = (LightType)ParseInt(parts[i++]);    // ah
        v.RearLights = (LightType)ParseInt(parts[i++]);     // ai

        if (i < parts.Length)
        {
            v.AnimGroup = ParseInt(parts[i]);               // aj (SA-specific)
        }
    }

    private static float ParseFloat(string value)
    {
        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;
        return 0f;
    }

    private static int ParseInt(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        return 0;
    }
}

