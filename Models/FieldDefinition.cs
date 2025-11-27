using System.Collections.Generic;
using System.Linq;

namespace GTAHandler.Models;

public enum FieldType
{
    String,
    Integer,
    Float,
    Boolean,
    DriveType,
    EngineType,
    LightType,
    HexFlags
}

public class FieldDefinition
{
    public string PropertyName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public FieldType FieldType { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public string Unit { get; init; } = string.Empty;
    public bool IsAvailableInGTA3 { get; init; } = true;
    public bool IsAvailableInGTAVC { get; init; } = true;
    public bool IsAvailableInGTASA { get; init; } = true;
    public int ColumnIndex { get; init; } = -1;
    public string Tooltip => string.IsNullOrEmpty(Unit)
        ? Description
        : $"{Description} ({Unit})";

    public bool IsAvailableFor(GameType gameType) => gameType switch
    {
        GameType.GTA3 => IsAvailableInGTA3,
        GameType.GTAVC => IsAvailableInGTAVC,
        GameType.GTASA => IsAvailableInGTASA,
        _ => false
    };
}

public static class FieldDefinitions
{
    public static readonly List<FieldDefinition> AllFields = new()
    {
        // Identity
        new FieldDefinition
        {
            PropertyName = "Identifier",
            DisplayName = "Vehicle ID",
            Description = "Vehicle identifier (14 characters max)",
            Category = "Identity",
            FieldType = FieldType.String,
            ColumnIndex = 0
        },
        
        // Physics
        new FieldDefinition
        {
            PropertyName = "Mass",
            DisplayName = "Mass",
            Description = "Vehicle mass",
            Category = "Physics",
            FieldType = FieldType.Float,
            MinValue = 1.0,
            MaxValue = 50000.0,
            Unit = "kg",
            ColumnIndex = 1
        },
        new FieldDefinition
        {
            PropertyName = "TurnMassOrDimensionX",
            DisplayName = "Turn Mass / Dimension X",
            Description = "SA: Turn Mass, GTA3/VC: Vehicle width",
            Category = "Physics",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1000000.0,
            Unit = "SA: units, 3/VC: m",
            ColumnIndex = 2
        },
        new FieldDefinition
        {
            PropertyName = "DragMultOrDimensionY",
            DisplayName = "Drag Mult / Dimension Y",
            Description = "SA: Drag Multiplier, GTA3/VC: Vehicle length",
            Category = "Physics",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 50.0,
            Unit = "SA: mult, 3/VC: m",
            ColumnIndex = 3
        },
        new FieldDefinition
        {
            PropertyName = "DimensionZ",
            DisplayName = "Dimension Z",
            Description = "Vehicle height (not used in SA)",
            Category = "Physics",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 20.0,
            Unit = "m",
            IsAvailableInGTASA = false,
            ColumnIndex = 4
        },
        
        // Centre of Mass
        new FieldDefinition
        {
            PropertyName = "CentreOfMassX",
            DisplayName = "Centre of Mass X",
            Description = "Centre of mass X offset",
            Category = "Centre of Mass",
            FieldType = FieldType.Float,
            MinValue = -10.0,
            MaxValue = 10.0,
            Unit = "m",
            ColumnIndex = 5
        },
        new FieldDefinition
        {
            PropertyName = "CentreOfMassY",
            DisplayName = "Centre of Mass Y",
            Description = "Centre of mass Y offset",
            Category = "Centre of Mass",
            FieldType = FieldType.Float,
            MinValue = -10.0,
            MaxValue = 10.0,
            Unit = "m",
            ColumnIndex = 6
        },
        new FieldDefinition
        {
            PropertyName = "CentreOfMassZ",
            DisplayName = "Centre of Mass Z",
            Description = "Centre of mass Z offset",
            Category = "Centre of Mass",
            FieldType = FieldType.Float,
            MinValue = -10.0,
            MaxValue = 10.0,
            Unit = "m",
            ColumnIndex = 7
        },
        
        // Buoyancy
        new FieldDefinition
        {
            PropertyName = "PercentSubmerged",
            DisplayName = "Percent Submerged",
            Description = "Buoyancy percent (>100% = sinks)",
            Category = "Buoyancy",
            FieldType = FieldType.Integer,
            MinValue = 10,
            MaxValue = 120,
            Unit = "%",
            ColumnIndex = 8
        },
        
        // Traction
        new FieldDefinition
        {
            PropertyName = "TractionMultiplier",
            DisplayName = "Traction Multiplier",
            Description = "Grip level multiplier",
            Category = "Traction",
            FieldType = FieldType.Float,
            MinValue = 0.5,
            MaxValue = 3.0,
            Unit = "×",
            ColumnIndex = 9
        },
        new FieldDefinition
        {
            PropertyName = "TractionLoss",
            DisplayName = "Traction Loss",
            Description = "Grip loss when accelerating/braking",
            Category = "Traction",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1.0,
            ColumnIndex = 10
        },
        new FieldDefinition
        {
            PropertyName = "TractionBias",
            DisplayName = "Traction Bias",
            Description = "Grip distribution (0=rear, 1=front)",
            Category = "Traction",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1.0,
            ColumnIndex = 11
        },
        
        // Transmission
        new FieldDefinition
        {
            PropertyName = "NumberOfGears",
            DisplayName = "Number of Gears",
            Description = "Number of transmission gears",
            Category = "Transmission",
            FieldType = FieldType.Integer,
            MinValue = 1,
            MaxValue = 5,
            ColumnIndex = 12
        },
        new FieldDefinition
        {
            PropertyName = "MaxVelocity",
            DisplayName = "Max Velocity",
            Description = "Maximum speed",
            Category = "Transmission",
            FieldType = FieldType.Float,
            MinValue = 5.0,
            MaxValue = 300.0,
            Unit = "km/h",
            ColumnIndex = 13
        },
        new FieldDefinition
        {
            PropertyName = "EngineAcceleration",
            DisplayName = "Engine Acceleration",
            Description = "Acceleration rate",
            Category = "Transmission",
            FieldType = FieldType.Float,
            MinValue = 0.1,
            MaxValue = 100.0,
            Unit = "m/s²",
            ColumnIndex = 14
        },
        new FieldDefinition
        {
            PropertyName = "EngineInertia",
            DisplayName = "Engine Inertia",
            Description = "Engine response delay (SA only)",
            Category = "Transmission",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 150.0,
            IsAvailableInGTA3 = false,
            IsAvailableInGTAVC = false,
            ColumnIndex = 15
        },
        new FieldDefinition
        {
            PropertyName = "DriveType",
            DisplayName = "Drive Type",
            Description = "Front/Rear/4-Wheel drive",
            Category = "Transmission",
            FieldType = FieldType.DriveType,
            ColumnIndex = 16
        },
        new FieldDefinition
        {
            PropertyName = "EngineType",
            DisplayName = "Engine Type",
            Description = "Petrol/Diesel/Electric",
            Category = "Transmission",
            FieldType = FieldType.EngineType,
            ColumnIndex = 17
        },
        
        // Brakes
        new FieldDefinition
        {
            PropertyName = "BrakeDeceleration",
            DisplayName = "Brake Deceleration",
            Description = "Braking power",
            Category = "Brakes",
            FieldType = FieldType.Float,
            MinValue = 0.1,
            MaxValue = 50.0,
            Unit = "m/s²",
            ColumnIndex = 18
        },
        new FieldDefinition
        {
            PropertyName = "BrakeBias",
            DisplayName = "Brake Bias",
            Description = "Brake distribution (0=rear, 1=front)",
            Category = "Brakes",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1.0,
            ColumnIndex = 19
        },
        new FieldDefinition
        {
            PropertyName = "HasABS",
            DisplayName = "ABS",
            Description = "Anti-lock braking system",
            Category = "Brakes",
            FieldType = FieldType.Boolean,
            ColumnIndex = 20
        },
        
        // Steering
        new FieldDefinition
        {
            PropertyName = "SteeringLock",
            DisplayName = "Steering Lock",
            Description = "Maximum steering angle",
            Category = "Steering",
            FieldType = FieldType.Float,
            MinValue = 10.0,
            MaxValue = 50.0,
            Unit = "°",
            ColumnIndex = 21
        },
        
        // Suspension
        new FieldDefinition
        {
            PropertyName = "SuspensionForceLevel",
            DisplayName = "Suspension Force",
            Description = "Spring stiffness",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 10.0,
            ColumnIndex = 22
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionDampingLevel",
            DisplayName = "Suspension Damping",
            Description = "Shock absorber strength",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 10.0,
            ColumnIndex = 23
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionHighSpeedComDamp",
            DisplayName = "High Speed Compression",
            Description = "High speed suspension damping (SA only)",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 200.0,
            IsAvailableInGTA3 = false,
            IsAvailableInGTAVC = false,
            ColumnIndex = 24
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionUpperLimit",
            DisplayName = "Suspension Upper Limit",
            Description = "Maximum suspension extension",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = -1.0,
            MaxValue = 2.0,
            Unit = "m",
            ColumnIndex = 25
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionLowerLimit",
            DisplayName = "Suspension Lower Limit",
            Description = "Maximum suspension compression",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = -2.0,
            MaxValue = 1.0,
            Unit = "m",
            ColumnIndex = 26
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionBias",
            DisplayName = "Suspension Bias",
            Description = "Front/rear suspension balance",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1.0,
            ColumnIndex = 27
        },
        new FieldDefinition
        {
            PropertyName = "SuspensionAntiDiveMultiplier",
            DisplayName = "Anti-Dive Multiplier",
            Description = "Nose dive prevention under braking",
            Category = "Suspension",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 1.0,
            IsAvailableInGTA3 = false,
            ColumnIndex = 28
        },
        
        // Misc
        new FieldDefinition
        {
            PropertyName = "SeatOffsetDistance",
            DisplayName = "Seat Offset",
            Description = "Ped seat position offset",
            Category = "Miscellaneous",
            FieldType = FieldType.Float,
            MinValue = -1.0,
            MaxValue = 2.0,
            Unit = "m",
            ColumnIndex = 29
        },
        new FieldDefinition
        {
            PropertyName = "CollisionDamageMultiplier",
            DisplayName = "Collision Damage Mult",
            Description = "Damage taken from collisions",
            Category = "Miscellaneous",
            FieldType = FieldType.Float,
            MinValue = 0.0,
            MaxValue = 5.0,
            Unit = "×",
            ColumnIndex = 30
        },
        new FieldDefinition
        {
            PropertyName = "MonetaryValue",
            DisplayName = "Monetary Value",
            Description = "Vehicle value in $",
            Category = "Miscellaneous",
            FieldType = FieldType.Integer,
            MinValue = 1,
            MaxValue = 1000000,
            Unit = "$",
            ColumnIndex = 31
        },
        
        // Flags
        new FieldDefinition
        {
            PropertyName = "ModelFlags",
            DisplayName = "Model Flags",
            Description = "Vehicle model flags (hex)",
            Category = "Flags",
            FieldType = FieldType.HexFlags,
            ColumnIndex = 32
        },
        new FieldDefinition
        {
            PropertyName = "HandlingFlags",
            DisplayName = "Handling Flags",
            Description = "Handling flags (hex, SA only)",
            Category = "Flags",
            FieldType = FieldType.HexFlags,
            IsAvailableInGTA3 = false,
            IsAvailableInGTAVC = false,
            ColumnIndex = 33
        },
        
        // Lights
        new FieldDefinition
        {
            PropertyName = "FrontLights",
            DisplayName = "Front Lights",
            Description = "Front light style",
            Category = "Lights",
            FieldType = FieldType.LightType,
            ColumnIndex = 34
        },
        new FieldDefinition
        {
            PropertyName = "RearLights",
            DisplayName = "Rear Lights",
            Description = "Rear light style",
            Category = "Lights",
            FieldType = FieldType.LightType,
            ColumnIndex = 35
        },
        
        // SA-specific
        new FieldDefinition
        {
            PropertyName = "AnimGroup",
            DisplayName = "Animation Group",
            Description = "Vehicle animation group (SA only)",
            Category = "Animation",
            FieldType = FieldType.Integer,
            MinValue = 0,
            MaxValue = 30,
            IsAvailableInGTA3 = false,
            IsAvailableInGTAVC = false,
            ColumnIndex = 36
        }
    };

    public static IEnumerable<FieldDefinition> GetFieldsForGame(GameType gameType)
    {
        return AllFields.Where(f => f.IsAvailableFor(gameType));
    }

    public static IEnumerable<string> GetCategories()
    {
        return AllFields.Select(f => f.Category).Distinct();
    }

    public static IEnumerable<FieldDefinition> GetFieldsByCategory(string category, GameType gameType)
    {
        return AllFields.Where(f => f.Category == category && f.IsAvailableFor(gameType));
    }
}

