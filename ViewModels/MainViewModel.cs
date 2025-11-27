using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GTAHandler.Models;
using GTAHandler.Services;
using GTAHandler.Views;
using Microsoft.Win32;

namespace GTAHandler.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HandlingParser _parser = new();
    private readonly HandlingWriter _writer = new();

    private List<string> _originalLines = new();
    private string _currentFilePath = string.Empty;

    [ObservableProperty]
    private GameType _selectedGameType = GameType.GTA3;

    [ObservableProperty]
    private ObservableCollection<VehicleHandling> _vehicles = new();

    [ObservableProperty]
    private ObservableCollection<VehicleCategoryGroup> _groupedVehicles = new();

    [ObservableProperty]
    private VehicleHandling? _selectedVehicle;

    [ObservableProperty]
    private ObservableCollection<VehicleHandling> _openTabs = new();

    [ObservableProperty]
    private VehicleHandling? _activeTab;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Select a game and load a handling.cfg file to begin";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveFileAsCommand))]
    private bool _isFileLoaded;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    [ObservableProperty]
    private string _windowTitle = "GTA Handler by VPZ";

    [ObservableProperty]
    private ObservableCollection<FieldDefinition> _currentFields = new();

    public ObservableCollection<GameTypeItem> GameTypes { get; } = new()
    {
        new GameTypeItem { GameType = GameType.GTA3, DisplayName = "GTA:III" },
        new GameTypeItem { GameType = GameType.GTAVC, DisplayName = "GTA:VC" },
        new GameTypeItem { GameType = GameType.GTASA, DisplayName = "GTA:SA" }
    };

    public ObservableCollection<DriveTypeItem> DriveTypes { get; } = new()
    {
        new DriveTypeItem { DriveType = Models.DriveType.Front, DisplayName = "Front (F)" },
        new DriveTypeItem { DriveType = Models.DriveType.Rear, DisplayName = "Rear (R)" },
        new DriveTypeItem { DriveType = Models.DriveType.FourWheel, DisplayName = "4-Wheel (4)" }
    };

    public ObservableCollection<EngineTypeItem> EngineTypes { get; } = new()
    {
        new EngineTypeItem { EngineType = Models.EngineType.Petrol, DisplayName = "Petrol (P)" },
        new EngineTypeItem { EngineType = Models.EngineType.Diesel, DisplayName = "Diesel (D)" },
        new EngineTypeItem { EngineType = Models.EngineType.Electric, DisplayName = "Electric (E)" }
    };

    public ObservableCollection<LightTypeItem> LightTypes { get; } = new()
    {
        new LightTypeItem { LightType = LightType.Long, DisplayName = "Long (0)" },
        new LightTypeItem { LightType = LightType.Small, DisplayName = "Small (1)" },
        new LightTypeItem { LightType = LightType.Big, DisplayName = "Big (2)" },
        new LightTypeItem { LightType = LightType.Tall, DisplayName = "Tall (3)" }
    };

    public MainViewModel()
    {
        UpdateFieldsForGameType();
    }

    partial void OnSelectedGameTypeChanged(GameType value)
    {
        UpdateFieldsForGameType();

        // Clear current data when game type changes
        if (IsFileLoaded)
        {
            var result = CustomDialog.ShowQuestion(
                "Changing game type will clear the current data. Continue?",
                "Confirm");

            if (result != Views.DialogResult.Yes)
            {
                return;
            }
        }

        ClearData();
    }

    partial void OnSelectedVehicleChanged(VehicleHandling? value)
    {
        if (value != null)
        {
            OpenVehicleTab(value);
        }
    }

    partial void OnActiveTabChanged(VehicleHandling? value)
    {
        if (value != null)
        {
            StatusMessage = $"Editing: {value.Identifier}";
        }
    }

    [RelayCommand]
    private void OpenVehicleTab(VehicleHandling vehicle)
    {
        if (!OpenTabs.Contains(vehicle))
        {
            OpenTabs.Add(vehicle);
        }
        ActiveTab = vehicle;
        StatusMessage = $"Editing: {vehicle.Identifier}";
    }

    [RelayCommand]
    private void CloseTab(VehicleHandling vehicle)
    {
        var index = OpenTabs.IndexOf(vehicle);
        OpenTabs.Remove(vehicle);

        if (ActiveTab == vehicle)
        {
            if (OpenTabs.Count > 0)
            {
                // Select the previous tab or the first one
                var newIndex = Math.Max(0, Math.Min(index, OpenTabs.Count - 1));
                ActiveTab = OpenTabs[newIndex];
                SelectedVehicle = ActiveTab;
            }
            else
            {
                ActiveTab = null;
                SelectedVehicle = null;
            }
        }
    }

    [RelayCommand]
    private void CloseAllTabs()
    {
        OpenTabs.Clear();
        ActiveTab = null;
    }

    [RelayCommand]
    private void CloseOtherTabs(VehicleHandling vehicle)
    {
        var toRemove = OpenTabs.Where(v => v != vehicle).ToList();
        foreach (var v in toRemove)
        {
            OpenTabs.Remove(v);
        }
        ActiveTab = vehicle;
    }

    [RelayCommand]
    private void CopyFromVehicle()
    {
        if (ActiveTab == null) return;

        // Get vehicles from the same category
        var sameCategory = Vehicles
            .Where(v => v.Category == ActiveTab.Category && v != ActiveTab)
            .OrderBy(v => v.Identifier)
            .ToList();

        if (!sameCategory.Any())
        {
            CustomDialog.ShowInfo(
                $"No other vehicles found in the {ActiveTab.Category.GetDisplayName()} category.",
                "Copy From Vehicle");
            return;
        }

        // Show selection dialog
        var dialog = new CopyFromDialog(sameCategory, ActiveTab.Category, SelectedGameType)
        {
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true && dialog.SelectedVehicle != null)
        {
            CopyVehicleValues(dialog.SelectedVehicle, ActiveTab);
            CustomDialog.ShowSuccess(
                $"Copied values from {dialog.SelectedVehicle.Identifier} to {ActiveTab.Identifier}.",
                "Values Copied");
        }
    }

    private void CopyVehicleValues(VehicleHandling source, VehicleHandling target)
    {
        // Copy all editable values except identifier and meta info
        target.Mass = source.Mass;
        target.TurnMassOrDimensionX = source.TurnMassOrDimensionX;
        target.DragMultOrDimensionY = source.DragMultOrDimensionY;
        target.DimensionZ = source.DimensionZ;
        target.CentreOfMassX = source.CentreOfMassX;
        target.CentreOfMassY = source.CentreOfMassY;
        target.CentreOfMassZ = source.CentreOfMassZ;
        target.PercentSubmerged = source.PercentSubmerged;
        target.TractionMultiplier = source.TractionMultiplier;
        target.TractionLoss = source.TractionLoss;
        target.TractionBias = source.TractionBias;
        target.NumberOfGears = source.NumberOfGears;
        target.MaxVelocity = source.MaxVelocity;
        target.EngineAcceleration = source.EngineAcceleration;
        target.EngineInertia = source.EngineInertia;
        target.DriveType = source.DriveType;
        target.EngineType = source.EngineType;
        target.BrakeDeceleration = source.BrakeDeceleration;
        target.BrakeBias = source.BrakeBias;
        target.HasABS = source.HasABS;
        target.SteeringLock = source.SteeringLock;
        target.SuspensionForceLevel = source.SuspensionForceLevel;
        target.SuspensionDampingLevel = source.SuspensionDampingLevel;
        target.SuspensionHighSpeedComDamp = source.SuspensionHighSpeedComDamp;
        target.SuspensionUpperLimit = source.SuspensionUpperLimit;
        target.SuspensionLowerLimit = source.SuspensionLowerLimit;
        target.SuspensionBias = source.SuspensionBias;
        target.SuspensionAntiDiveMultiplier = source.SuspensionAntiDiveMultiplier;
        target.SeatOffsetDistance = source.SeatOffsetDistance;
        target.CollisionDamageMultiplier = source.CollisionDamageMultiplier;
        target.MonetaryValue = source.MonetaryValue;
        target.ModelFlags = source.ModelFlags;
        target.HandlingFlags = source.HandlingFlags;
        target.FrontLights = source.FrontLights;
        target.RearLights = source.RearLights;
        target.AnimGroup = source.AnimGroup;

        target.IsModified = true;
        HasUnsavedChanges = true;
    }

    partial void OnSearchTextChanged(string value)
    {
        UpdateGroupedVehicles();
    }

    private void UpdateGroupedVehicles()
    {
        var searchLower = SearchText?.ToLowerInvariant() ?? string.Empty;

        var filtered = string.IsNullOrWhiteSpace(searchLower)
            ? Vehicles.ToList()
            : Vehicles.Where(v => v.Identifier.ToLowerInvariant().Contains(searchLower)).ToList();

        GroupedVehicles.Clear();

        var groups = filtered
            .GroupBy(v => v.Category)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in groups)
        {
            var categoryGroup = new VehicleCategoryGroup
            {
                Category = group.Key,
                DisplayName = group.Key.GetDisplayName(),
                Vehicles = new ObservableCollection<VehicleHandling>(group.OrderBy(v => v.Identifier))
            };
            GroupedVehicles.Add(categoryGroup);
        }
    }

    private void UpdateFieldsForGameType()
    {
        CurrentFields.Clear();
        foreach (var field in FieldDefinitions.GetFieldsForGame(SelectedGameType))
        {
            CurrentFields.Add(field);
        }
    }

    [RelayCommand]
    private void BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Config Files (*.cfg)|*.cfg|All Files (*.*)|*.*",
            Title = $"Select handling.cfg for {SelectedGameType.GetDisplayName()}"
        };

        if (dialog.ShowDialog() == true)
        {
            LoadFile(dialog.FileName);
        }
    }

    [RelayCommand]
    private void LoadFile(string filePath)
    {
        try
        {
            var (vehicles, rawLines) = _parser.ParseFile(filePath, SelectedGameType);

            _originalLines = rawLines;
            _currentFilePath = filePath;

            Vehicles.Clear();
            foreach (var vehicle in vehicles.OrderBy(v => v.Identifier))
            {
                // Determine category
                vehicle.DetermineCategory();

                // Subscribe to property changes for tracking modifications
                vehicle.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName != nameof(VehicleHandling.IsModified) &&
                        e.PropertyName != nameof(VehicleHandling.Category))
                    {
                        ((VehicleHandling)s!).IsModified = true;
                        HasUnsavedChanges = true;
                    }
                };
                Vehicles.Add(vehicle);
            }

            // Update grouped view
            UpdateGroupedVehicles();

            IsFileLoaded = true;
            HasUnsavedChanges = false;
            StatusMessage = $"Loaded {vehicles.Count} vehicles from {Path.GetFileName(filePath)}";
            WindowTitle = $"GTA Handler by VPZ - {Path.GetFileName(filePath)} [{SelectedGameType.GetDisplayName()}]";

            // Select first vehicle in first category
            if (GroupedVehicles.Any() && GroupedVehicles.First().Vehicles.Any())
            {
                SelectedVehicle = GroupedVehicles.First().Vehicles.First();
            }
        }
        catch (HandlingParseException ex)
        {
            string message = ex.Message;
            if (!string.IsNullOrEmpty(ex.Details))
            {
                message += $"\n\n{ex.Details}";
            }

            string title = ex.ErrorType switch
            {
                ParseErrorType.FileNotFound => "File Not Found",
                ParseErrorType.FileEmpty => "Empty File",
                ParseErrorType.FileUnreadable => "Cannot Read File",
                ParseErrorType.InvalidFormat => "Invalid Format",
                ParseErrorType.WrongGameFormat => "Wrong Game Selected",
                ParseErrorType.NoVehiclesFound => "No Vehicles Found",
                _ => "Error"
            };

            CustomDialog.ShowError(message, title);
            StatusMessage = $"Failed to load: {ex.ErrorType}";
        }
        catch (Exception ex)
        {
            CustomDialog.ShowError($"Unexpected error loading file:\n\n{ex.Message}", "Error");
            StatusMessage = "Error loading file";
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void SaveFile()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            SaveFileAs();
            return;
        }

        try
        {
            _writer.SaveFile(_currentFilePath, Vehicles.ToList(), _originalLines, SelectedGameType);

            // Reset modification flags
            foreach (var vehicle in Vehicles)
            {
                vehicle.IsModified = false;
            }
            HasUnsavedChanges = false;

            StatusMessage = $"Saved to {Path.GetFileName(_currentFilePath)}";
        }
        catch (Exception ex)
        {
            CustomDialog.ShowError($"Error saving file: {ex.Message}", "Error");
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void SaveFileAs()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Config Files (*.cfg)|*.cfg|All Files (*.*)|*.*",
            Title = "Save handling.cfg",
            FileName = "handling.cfg"
        };

        if (dialog.ShowDialog() == true)
        {
            _currentFilePath = dialog.FileName;
            SaveFile();
            WindowTitle = $"GTA Handler by VPZ - {Path.GetFileName(_currentFilePath)} [{SelectedGameType.GetDisplayName()}]";
        }
    }

    private bool CanSave() => IsFileLoaded;

    [RelayCommand]
    private void ResetVehicle()
    {
        if (ActiveTab == null) return;

        var result = CustomDialog.ShowQuestion(
            $"Reset {ActiveTab.Identifier} to original values?",
            "Confirm Reset");

        if (result == Views.DialogResult.Yes)
        {
            try
            {
                // Re-parse the original line
                var originalVehicles = _parser.ParseFile(_currentFilePath, SelectedGameType).Vehicles;
                var original = originalVehicles.FirstOrDefault(v => v.Identifier == ActiveTab.Identifier);

                if (original != null)
                {
                    // Copy original values to active tab
                    CopyVehicleValues(original, ActiveTab);
                    ActiveTab.IsModified = false;
                    StatusMessage = $"Reset {ActiveTab.Identifier} to original values";
                }
                else
                {
                    CustomDialog.ShowError(
                        $"Could not find original values for {ActiveTab.Identifier}.",
                        "Reset Failed");
                }
            }
            catch (Exception ex)
            {
                CustomDialog.ShowError(
                    $"Failed to reset vehicle: {ex.Message}",
                    "Reset Failed");
            }
        }
    }

    [RelayCommand]
    private void ClearData()
    {
        Vehicles.Clear();
        GroupedVehicles.Clear();
        OpenTabs.Clear();
        ActiveTab = null;
        SelectedVehicle = null;
        SearchText = string.Empty;
        _originalLines.Clear();
        _currentFilePath = string.Empty;
        IsFileLoaded = false;
        HasUnsavedChanges = false;
        WindowTitle = "GTA Handler by VPZ";
        StatusMessage = "Select a game and load a handling.cfg file to begin";
    }

    [RelayCommand]
    private void ShowAbout()
    {
        var dialog = new AboutDialog
        {
            Owner = Application.Current.MainWindow
        };
        dialog.ShowDialog();
    }
}

public class GameTypeItem
{
    public GameType GameType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public override string ToString() => DisplayName;
}

public class DriveTypeItem
{
    public Models.DriveType DriveType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public override string ToString() => DisplayName;
}

public class EngineTypeItem
{
    public Models.EngineType EngineType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public override string ToString() => DisplayName;
}

public class LightTypeItem
{
    public LightType LightType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public override string ToString() => DisplayName;
}

public class VehicleCategoryGroup
{
    public VehicleCategory Category { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public ObservableCollection<VehicleHandling> Vehicles { get; set; } = new();
    public int Count => Vehicles.Count;
}

