using System;
using System.Linq;

namespace GTAHandler.Models;

public enum VehicleCategory
{
    Cars,
    Bikes,
    Boats,
    Planes,
    Helicopters,
    Trailers,
    Other
}

public static class VehicleCategoryExtensions
{
    public static string GetDisplayName(this VehicleCategory category) => category switch
    {
        VehicleCategory.Cars => "Cars",
        VehicleCategory.Bikes => "Bikes",
        VehicleCategory.Boats => "Boats",
        VehicleCategory.Planes => "Planes",
        VehicleCategory.Helicopters => "Helicopters",
        VehicleCategory.Trailers => "Trailers",
        VehicleCategory.Other => "Other",
        _ => category.ToString()
    };

    public static string GetIconName(this VehicleCategory category) => category switch
    {
        VehicleCategory.Cars => "Car",
        VehicleCategory.Bikes => "Motorcycle",
        VehicleCategory.Boats => "Ship",
        VehicleCategory.Planes => "Plane",
        VehicleCategory.Helicopters => "Helicopter",
        VehicleCategory.Trailers => "Truck",
        VehicleCategory.Other => "Cube",
        _ => "Car"
    };

    /// <summary>
    /// Determines the vehicle category based on model flags and vehicle name
    /// </summary>
    public static VehicleCategory DetermineCategory(string identifier, string modelFlags, GameType gameType)
    {
        // Known vehicle names for categorization
        var bikes = new[] { "BIKE", "MOPED", "DIRTBIKE", "ANGEL", "FREEWAY", "FCR900", "NRG500",
                           "HPV1000", "BF400", "WAYFARER", "QUADBIKE", "BMX", "CHOPPERB", "MTB" };
        var boats = new[] { "PREDATOR", "SPEEDER", "REEFER", "RIO", "SQUALO", "TROPIC",
                           "COASTGRD", "DINGHY", "MARQUIS", "CUPBOAT", "LAUNCH", "SEAPLANE" };
        var planes = new[] { "DODO", "RUSTLER", "BEAGLE", "CROPDUST", "STUNT", "SHAMAL",
                            "HYDRA", "NEVADA", "AT400", "ANDROM", "AIRTRAIN", "DEADDODO", "RCBARON" };
        var helicopters = new[] { "SPARROW", "SEASPAR", "MAVERICK", "COASTMAV", "POLMAV",
                                  "HUNTER", "LEVIATHN", "CARGOBOB", "RAINDANC", "RCGOBLIN",
                                  "RCCOPTER", "RCRAIDER", "HELI" };
        var trailers = new[] { "ARTICT1", "ARTICT2", "ARTICT3", "PETROTR", "BAGBOXA", "BAGBOXB",
                              "TUGSTAIR", "FARM_TR1", "UTIL_TR1", "FREIFLAT", "STREAK", "FREIGHT",
                              "CSTREAK", "TRAIN" };

        var upperName = identifier.ToUpperInvariant().Trim();

        // Check by name first
        if (bikes.Any(b => upperName.Contains(b)))
            return VehicleCategory.Bikes;
        if (boats.Any(b => upperName.Equals(b, StringComparison.OrdinalIgnoreCase)))
            return VehicleCategory.Boats;
        if (planes.Any(p => upperName.Equals(p, StringComparison.OrdinalIgnoreCase)))
            return VehicleCategory.Planes;
        if (helicopters.Any(h => upperName.Equals(h, StringComparison.OrdinalIgnoreCase)))
            return VehicleCategory.Helicopters;
        if (trailers.Any(t => upperName.Equals(t, StringComparison.OrdinalIgnoreCase)))
            return VehicleCategory.Trailers;

        // Check model flags for SA
        if (gameType == GameType.GTASA && !string.IsNullOrEmpty(modelFlags))
        {
            try
            {
                var flags = Convert.ToUInt32(modelFlags, 16);
                // 7th digit: 1: IS_BIKE, 2: IS_HELI, 4: IS_PLANE, 8: IS_BOAT
                var vehicleTypeFlags = (flags >> 24) & 0xF;

                if ((vehicleTypeFlags & 0x1) != 0) return VehicleCategory.Bikes;
                if ((vehicleTypeFlags & 0x2) != 0) return VehicleCategory.Helicopters;
                if ((vehicleTypeFlags & 0x4) != 0) return VehicleCategory.Planes;
                if ((vehicleTypeFlags & 0x8) != 0) return VehicleCategory.Boats;
            }
            catch
            {
                // Ignore parsing errors
            }
        }

        // Default to cars
        return VehicleCategory.Cars;
    }
}

