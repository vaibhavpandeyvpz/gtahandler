using CommunityToolkit.Mvvm.ComponentModel;

namespace GTAHandler.Models;

/// <summary>
/// Represents vehicle handling data. Properties vary slightly by game version.
/// </summary>
public partial class VehicleHandling : ObservableObject
{
    // ===== Identity =====
    [ObservableProperty] private string _identifier = string.Empty; // A - Vehicle name (14 chars max)

    // ===== Physics =====
    [ObservableProperty] private float _mass = 1000.0f; // B - fMass [1.0 to 50000.0]

    // GTA3/VC: Dimensions.x, SA: fTurnMass
    [ObservableProperty] private float _turnMassOrDimensionX = 2.0f; // C

    // GTA3/VC: Dimensions.y, SA: fDragMult
    [ObservableProperty] private float _dragMultOrDimensionY = 5.0f; // D

    // GTA3/VC: Dimensions.z, SA: Not used
    [ObservableProperty] private float _dimensionZ = 1.5f; // E (not used in SA)

    // ===== Centre of Mass =====
    [ObservableProperty] private float _centreOfMassX = 0.0f; // F [-10.0 to 10.0]
    [ObservableProperty] private float _centreOfMassY = 0.0f; // G [-10.0 to 10.0]
    [ObservableProperty] private float _centreOfMassZ = 0.0f; // H [-10.0 to 10.0]

    // ===== Buoyancy =====
    [ObservableProperty] private int _percentSubmerged = 75; // I [10 to 120]

    // ===== Traction =====
    [ObservableProperty] private float _tractionMultiplier = 1.0f; // J [0.5 to 2.0]
    [ObservableProperty] private float _tractionLoss = 0.8f; // K [0.0 to 1.0]
    [ObservableProperty] private float _tractionBias = 0.5f; // L [0.0 to 1.0]

    // ===== Transmission =====
    [ObservableProperty] private int _numberOfGears = 5; // M [1 to 5 in SA, 1 to 4 in others]
    [ObservableProperty] private float _maxVelocity = 160.0f; // N [5.0 to 300.0]
    [ObservableProperty] private float _engineAcceleration = 20.0f; // O [0.1 to 50.0]

    // SA only: Engine Inertia
    [ObservableProperty] private float _engineInertia = 5.0f; // P (SA only) [0.0 to 50.0]

    [ObservableProperty] private DriveType _driveType = DriveType.Rear; // P (GTA3/VC) or Q (SA)
    [ObservableProperty] private EngineType _engineType = EngineType.Petrol; // Q (GTA3/VC) or R (SA)

    // ===== Brakes =====
    [ObservableProperty] private float _brakeDeceleration = 8.0f; // R (GTA3/VC) or S (SA) [0.1 to 10.0]
    [ObservableProperty] private float _brakeBias = 0.5f; // S (GTA3/VC) or T (SA) [0.0 to 1.0]
    [ObservableProperty] private bool _hasABS = false; // T (GTA3/VC) or U (SA) [0/1]

    // ===== Steering =====
    [ObservableProperty] private float _steeringLock = 30.0f; // U (GTA3/VC) or V (SA) [10.0 to 40.0]

    // ===== Suspension =====
    [ObservableProperty] private float _suspensionForceLevel = 1.5f; // V (GTA3/VC) or a (SA)
    [ObservableProperty] private float _suspensionDampingLevel = 0.1f; // W (GTA3/VC) or b (SA)

    // SA only: High Speed Suspension Compression Damping
    [ObservableProperty] private float _suspensionHighSpeedComDamp = 0.0f; // c (SA only)

    [ObservableProperty] private float _suspensionUpperLimit = 0.3f; // aa (GTA3) / aa (VC) / d (SA)
    [ObservableProperty] private float _suspensionLowerLimit = -0.15f; // ab (GTA3) / ab (VC) / e (SA)
    [ObservableProperty] private float _suspensionBias = 0.5f; // ac (GTA3) / ac (VC) / f (SA)
    [ObservableProperty] private float _suspensionAntiDiveMultiplier = 0.0f; // ad (VC) / g (SA) - not in GTA3

    // ===== Misc =====
    [ObservableProperty] private float _seatOffsetDistance = 0.2f; // X (GTA3/VC) or aa (SA)
    [ObservableProperty] private float _collisionDamageMultiplier = 0.5f; // Y (GTA3/VC) or ab (SA) [0.2 to 5.0]
    [ObservableProperty] private int _monetaryValue = 25000; // Z (GTA3/VC) or ac (SA) [1 to 100000]

    // ===== Flags =====
    [ObservableProperty] private string _modelFlags = "0"; // Hex flags
    [ObservableProperty] private string _handlingFlags = "0"; // Hex flags (SA only has this separate)

    // ===== Lights =====
    [ObservableProperty] private LightType _frontLights = LightType.Long; // ae (GTA3) / af (VC/SA)
    [ObservableProperty] private LightType _rearLights = LightType.Small; // af (GTA3) / ag (SA)

    // ===== SA-specific =====
    [ObservableProperty] private int _animGroup = 0; // Vehicle animation group (SA only)

    // ===== Meta =====
    [ObservableProperty] private GameType _gameType = GameType.GTA3;
    [ObservableProperty] private string _rawLine = string.Empty; // Original line for reference
    [ObservableProperty] private int _lineNumber = 0; // Line number in file
    [ObservableProperty] private bool _isModified = false;
    [ObservableProperty] private VehicleCategory _category = VehicleCategory.Cars;

    public void DetermineCategory()
    {
        Category = VehicleCategoryExtensions.DetermineCategory(Identifier, ModelFlags, GameType);
    }

    public VehicleHandling Clone()
    {
        return new VehicleHandling
        {
            Identifier = Identifier,
            Mass = Mass,
            TurnMassOrDimensionX = TurnMassOrDimensionX,
            DragMultOrDimensionY = DragMultOrDimensionY,
            DimensionZ = DimensionZ,
            CentreOfMassX = CentreOfMassX,
            CentreOfMassY = CentreOfMassY,
            CentreOfMassZ = CentreOfMassZ,
            PercentSubmerged = PercentSubmerged,
            TractionMultiplier = TractionMultiplier,
            TractionLoss = TractionLoss,
            TractionBias = TractionBias,
            NumberOfGears = NumberOfGears,
            MaxVelocity = MaxVelocity,
            EngineAcceleration = EngineAcceleration,
            EngineInertia = EngineInertia,
            DriveType = DriveType,
            EngineType = EngineType,
            BrakeDeceleration = BrakeDeceleration,
            BrakeBias = BrakeBias,
            HasABS = HasABS,
            SteeringLock = SteeringLock,
            SuspensionForceLevel = SuspensionForceLevel,
            SuspensionDampingLevel = SuspensionDampingLevel,
            SuspensionHighSpeedComDamp = SuspensionHighSpeedComDamp,
            SuspensionUpperLimit = SuspensionUpperLimit,
            SuspensionLowerLimit = SuspensionLowerLimit,
            SuspensionBias = SuspensionBias,
            SuspensionAntiDiveMultiplier = SuspensionAntiDiveMultiplier,
            SeatOffsetDistance = SeatOffsetDistance,
            CollisionDamageMultiplier = CollisionDamageMultiplier,
            MonetaryValue = MonetaryValue,
            ModelFlags = ModelFlags,
            HandlingFlags = HandlingFlags,
            FrontLights = FrontLights,
            RearLights = RearLights,
            AnimGroup = AnimGroup,
            GameType = GameType,
            RawLine = RawLine,
            LineNumber = LineNumber,
            IsModified = IsModified
        };
    }
}


