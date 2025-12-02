using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GTAHandler.Models;

namespace GTAHandler.Services;

public class HandlingWriter
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public void SaveFile(string filePath, List<VehicleHandling> vehicles, List<string> originalLines, GameType gameType)
    {
        // Create a dictionary for quick lookup of modified vehicles
        var vehicleMap = vehicles.ToDictionary(v => v.LineNumber, v => v);

        var output = new StringBuilder();

        for (int i = 0; i < originalLines.Count; i++)
        {
            int lineNumber = i + 1;

            if (vehicleMap.TryGetValue(lineNumber, out var vehicle))
            {
                // Only regenerate line if vehicle was modified, otherwise preserve original formatting
                if (vehicle.IsModified)
                {
                    // Write the modified vehicle line
                    output.AppendLine(GenerateVehicleLine(vehicle, gameType));
                }
                else
                {
                    // Preserve original line formatting (tabs, spacing, etc.)
                    output.AppendLine(originalLines[i]);
                }
            }
            else
            {
                // Keep the original line
                output.AppendLine(originalLines[i]);
            }
        }

        File.WriteAllText(filePath, output.ToString(), Encoding.ASCII);
    }

    public string GenerateVehicleLine(VehicleHandling v, GameType gameType)
    {
        return gameType switch
        {
            GameType.GTA3 => GenerateGTA3Line(v),
            GameType.GTAVC => GenerateGTAVCLine(v),
            GameType.GTASA => GenerateGTASALine(v),
            _ => v.RawLine
        };
    }

    private string GenerateGTA3Line(VehicleHandling v)
    {
        // GTA3: 31 fields
        var parts = new List<string>
        {
            v.Identifier.PadRight(14),
            FormatFloat(v.Mass),
            FormatFloat(v.TurnMassOrDimensionX),
            FormatFloat(v.DragMultOrDimensionY),
            FormatFloat(v.DimensionZ),
            FormatFloat(v.CentreOfMassX),
            FormatFloat(v.CentreOfMassY),
            FormatFloat(v.CentreOfMassZ),
            v.PercentSubmerged.ToString(),
            FormatFloat(v.TractionMultiplier),
            FormatFloat(v.TractionLoss),
            FormatFloat(v.TractionBias),
            v.NumberOfGears.ToString(),
            FormatFloat(v.MaxVelocity),
            FormatFloat(v.EngineAcceleration),
            v.DriveType.ToChar().ToString(),
            v.EngineType.ToChar().ToString(),
            FormatFloat(v.BrakeDeceleration),
            FormatFloat(v.BrakeBias),
            v.HasABS ? "1" : "0",
            FormatFloat(v.SteeringLock),
            FormatFloat(v.SuspensionForceLevel),
            FormatFloat(v.SuspensionDampingLevel),
            FormatFloat(v.SeatOffsetDistance),
            FormatFloat(v.CollisionDamageMultiplier),
            v.MonetaryValue.ToString(),
            FormatFloat(v.SuspensionUpperLimit),
            FormatFloat(v.SuspensionLowerLimit),
            FormatFloat(v.SuspensionBias),
            v.ModelFlags,
            ((int)v.FrontLights).ToString(),
            ((int)v.RearLights).ToString()
        };

        return string.Join(" ", parts);
    }

    private string GenerateGTAVCLine(VehicleHandling v)
    {
        // GTAVC: 33 fields
        var parts = new List<string>
        {
            v.Identifier.PadRight(14),
            FormatFloat(v.Mass),
            FormatFloat(v.TurnMassOrDimensionX),
            FormatFloat(v.DragMultOrDimensionY),
            FormatFloat(v.DimensionZ),
            FormatFloat(v.CentreOfMassX),
            FormatFloat(v.CentreOfMassY),
            FormatFloat(v.CentreOfMassZ),
            v.PercentSubmerged.ToString(),
            FormatFloat(v.TractionMultiplier),
            FormatFloat(v.TractionLoss),
            FormatFloat(v.TractionBias),
            v.NumberOfGears.ToString(),
            FormatFloat(v.MaxVelocity),
            FormatFloat(v.EngineAcceleration),
            v.DriveType.ToChar().ToString(),
            v.EngineType.ToChar().ToString(),
            FormatFloat(v.BrakeDeceleration),
            FormatFloat(v.BrakeBias),
            v.HasABS ? "1" : "0",
            FormatFloat(v.SteeringLock),
            FormatFloat(v.SuspensionForceLevel),
            FormatFloat(v.SuspensionDampingLevel),
            FormatFloat(v.SeatOffsetDistance),
            FormatFloat(v.CollisionDamageMultiplier),
            v.MonetaryValue.ToString(),
            FormatFloat(v.SuspensionUpperLimit),
            FormatFloat(v.SuspensionLowerLimit),
            FormatFloat(v.SuspensionBias),
            FormatFloat(v.SuspensionAntiDiveMultiplier),
            v.ModelFlags,
            ((int)v.FrontLights).ToString(),
            ((int)v.RearLights).ToString()
        };

        return string.Join(" ", parts);
    }

    private string GenerateGTASALine(VehicleHandling v)
    {
        // GTASA: 36+ fields
        var parts = new List<string>
        {
            v.Identifier.PadRight(12),
            FormatFloat(v.Mass),
            FormatFloat(v.TurnMassOrDimensionX),
            FormatFloat(v.DragMultOrDimensionY),
            FormatFloat(v.CentreOfMassX),
            FormatFloat(v.CentreOfMassY),
            FormatFloat(v.CentreOfMassZ),
            v.PercentSubmerged.ToString(),
            FormatFloat(v.TractionMultiplier),
            FormatFloat(v.TractionLoss),
            FormatFloat(v.TractionBias),
            v.NumberOfGears.ToString(),
            FormatFloat(v.MaxVelocity),
            FormatFloat(v.EngineAcceleration),
            FormatFloat(v.EngineInertia),
            v.DriveType.ToChar().ToString(),
            v.EngineType.ToChar().ToString(),
            FormatFloat(v.BrakeDeceleration),
            FormatFloat(v.BrakeBias),
            v.HasABS ? "1" : "0",
            FormatFloat(v.SteeringLock),
            FormatFloat(v.SuspensionForceLevel),
            FormatFloat(v.SuspensionDampingLevel),
            FormatFloat(v.SuspensionHighSpeedComDamp),
            FormatFloat(v.SuspensionUpperLimit),
            FormatFloat(v.SuspensionLowerLimit),
            FormatFloat(v.SuspensionBias),
            FormatFloat(v.SuspensionAntiDiveMultiplier),
            FormatFloat(v.SeatOffsetDistance),
            FormatFloat(v.CollisionDamageMultiplier),
            v.MonetaryValue.ToString(),
            v.ModelFlags,
            v.HandlingFlags,
            ((int)v.FrontLights).ToString(),
            ((int)v.RearLights).ToString(),
            v.AnimGroup.ToString()
        };

        return string.Join(" ", parts);
    }

    private static string FormatFloat(float value)
    {
        // Format with reasonable precision, removing trailing zeros
        string formatted = value.ToString("0.######", CultureInfo.InvariantCulture);

        // Ensure at least one decimal place for floats
        if (!formatted.Contains('.'))
        {
            formatted += ".0";
        }

        return formatted;
    }
}

