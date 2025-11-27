using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GTAHandler.Models;

namespace GTAHandler.Views;

public partial class CopyFromDialog : Window
{
    public VehicleHandling? SelectedVehicle { get; private set; }
    public VehicleHandling? ParsedVehicle { get; private set; }

    private readonly List<VehicleHandling> _allVehicles;
    private readonly GameType _gameType;

    public CopyFromDialog(List<VehicleHandling> vehicles, VehicleCategory category, GameType gameType)
    {
        InitializeComponent();

        _allVehicles = vehicles;
        _gameType = gameType;
        VehicleList.ItemsSource = vehicles;
        CategoryText.Text = $"Select a vehicle from {category.GetDisplayName()}:";

        VehicleList.SelectionChanged += (s, e) =>
        {
            if (VehicleList.SelectedItem != null)
            {
                // Clear parsed vehicle when selecting from list
                ParsedVehicle = null;
                UpdateCopyButtonState();
            }
        };

        // Focus raw config box
        Loaded += (s, e) => RawConfigBox.Focus();
    }

    private void UpdateCopyButtonState()
    {
        CopyButton.IsEnabled = VehicleList.SelectedItem != null || ParsedVehicle != null;
    }

    private void RawConfigBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = RawConfigBox.Text?.Trim() ?? string.Empty;

        // Update placeholder visibility
        RawConfigPlaceholder.Visibility = string.IsNullOrEmpty(text)
            ? Visibility.Visible
            : Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(text))
        {
            ParsedVehicle = null;
            ParseStatusText.Visibility = Visibility.Collapsed;
            UpdateCopyButtonState();
            return;
        }

        // Try to parse the line
        try
        {
            ParsedVehicle = ParseHandlingLine(text);
            if (ParsedVehicle != null)
            {
                // Clear vehicle list selection when successfully parsing
                VehicleList.SelectedItem = null;

                ParseStatusText.Text = "✓ Config parsed successfully";
                ParseStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#537b35"));
                ParseStatusText.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            ParsedVehicle = null;
            ParseStatusText.Text = $"✕ {ex.Message}";
            ParseStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ed1b2e"));
            ParseStatusText.Visibility = Visibility.Visible;
        }

        UpdateCopyButtonState();
    }

    private VehicleHandling? ParseHandlingLine(string line)
    {
        // Remove comments
        var commentIndex = line.IndexOf(';');
        if (commentIndex >= 0)
            line = line.Substring(0, commentIndex);

        line = line.Trim();
        if (string.IsNullOrEmpty(line))
            return null;

        // Split by whitespace
        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        // Validate column count based on game type
        int expectedColumns = _gameType switch
        {
            GameType.GTA3 => 32,
            GameType.GTAVC => 32,
            GameType.GTASA => 36,
            _ => 32
        };

        if (parts.Length < expectedColumns)
        {
            throw new Exception($"Expected {expectedColumns} columns, found {parts.Length}");
        }

        var vehicle = new VehicleHandling();

        try
        {
            int i = 0;
            vehicle.Identifier = parts[i++];
            vehicle.Mass = ParseFloat(parts[i++]);
            vehicle.TurnMassOrDimensionX = ParseFloat(parts[i++]);
            vehicle.DragMultOrDimensionY = ParseFloat(parts[i++]);
            vehicle.CentreOfMassX = ParseFloat(parts[i++]);
            vehicle.CentreOfMassY = ParseFloat(parts[i++]);
            vehicle.CentreOfMassZ = ParseFloat(parts[i++]);
            vehicle.PercentSubmerged = ParseInt(parts[i++]);
            vehicle.TractionMultiplier = ParseFloat(parts[i++]);
            vehicle.TractionLoss = ParseFloat(parts[i++]);
            vehicle.TractionBias = ParseFloat(parts[i++]);
            vehicle.NumberOfGears = ParseInt(parts[i++]);
            vehicle.MaxVelocity = ParseFloat(parts[i++]);
            vehicle.EngineAcceleration = ParseFloat(parts[i++]);

            if (_gameType == GameType.GTASA)
            {
                vehicle.EngineInertia = ParseFloat(parts[i++]);
            }

            vehicle.DriveType = ParseDriveType(parts[i++]);
            vehicle.EngineType = ParseEngineType(parts[i++]);
            vehicle.BrakeDeceleration = ParseFloat(parts[i++]);
            vehicle.BrakeBias = ParseFloat(parts[i++]);
            vehicle.HasABS = ParseInt(parts[i++]) != 0;
            vehicle.SteeringLock = ParseFloat(parts[i++]);
            vehicle.SuspensionForceLevel = ParseFloat(parts[i++]);
            vehicle.SuspensionDampingLevel = ParseFloat(parts[i++]);

            if (_gameType == GameType.GTASA)
            {
                vehicle.SuspensionHighSpeedComDamp = ParseFloat(parts[i++]);
            }

            vehicle.SuspensionUpperLimit = ParseFloat(parts[i++]);
            vehicle.SuspensionLowerLimit = ParseFloat(parts[i++]);
            vehicle.SuspensionBias = ParseFloat(parts[i++]);

            if (_gameType == GameType.GTASA)
            {
                vehicle.SuspensionAntiDiveMultiplier = ParseFloat(parts[i++]);
            }

            vehicle.SeatOffsetDistance = ParseFloat(parts[i++]);
            vehicle.CollisionDamageMultiplier = ParseFloat(parts[i++]);
            vehicle.MonetaryValue = ParseInt(parts[i++]);

            // Model flags - handle hex (store as string)
            vehicle.ModelFlags = parts[i++];
            vehicle.HandlingFlags = parts[i++];

            vehicle.FrontLights = ParseLightType(parts[i++]);
            vehicle.RearLights = ParseLightType(parts[i++]);

            if (_gameType == GameType.GTASA)
            {
                vehicle.AnimGroup = ParseInt(parts[i++]);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Parse error: {ex.Message}");
        }

        return vehicle;
    }

    private float ParseFloat(string s) => float.Parse(s, CultureInfo.InvariantCulture);
    private int ParseInt(string s) => int.Parse(s, CultureInfo.InvariantCulture);

    private DriveType ParseDriveType(string s) => s.ToUpper() switch
    {
        "F" => DriveType.Front,
        "R" => DriveType.Rear,
        "4" => DriveType.FourWheel,
        _ => DriveType.Rear
    };

    private EngineType ParseEngineType(string s) => s.ToUpper() switch
    {
        "P" => EngineType.Petrol,
        "D" => EngineType.Diesel,
        "E" => EngineType.Electric,
        _ => EngineType.Petrol
    };

    private LightType ParseLightType(string s) => int.Parse(s) switch
    {
        0 => LightType.Long,
        1 => LightType.Small,
        2 => LightType.Big,
        3 => LightType.Tall,
        _ => LightType.Long
    };

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text?.ToLowerInvariant() ?? string.Empty;

        // Update placeholder visibility
        SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;

        // Filter vehicles
        if (string.IsNullOrWhiteSpace(searchText))
        {
            VehicleList.ItemsSource = _allVehicles;
        }
        else
        {
            VehicleList.ItemsSource = _allVehicles
                .Where(v => v.Identifier.ToLowerInvariant().Contains(searchText))
                .ToList();
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        // Prefer parsed vehicle over selected
        if (ParsedVehicle != null)
        {
            SelectedVehicle = ParsedVehicle;
        }
        else
        {
            SelectedVehicle = VehicleList.SelectedItem as VehicleHandling;
        }

        DialogResult = true;
        Close();
    }

    private void VehicleList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (VehicleList.SelectedItem != null)
        {
            SelectedVehicle = VehicleList.SelectedItem as VehicleHandling;
            DialogResult = true;
            Close();
        }
    }
}

