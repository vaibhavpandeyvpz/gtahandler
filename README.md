# GTA Handler

A visual editor for **handling.cfg** files from the classic GTA trilogy:
- GTA:III
- GTA:VC
- GTA:SA

![Windows](https://img.shields.io/badge/Platform-Windows-blue) ![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![License](https://img.shields.io/badge/License-MIT-green)

![Screenshot](assets/screenshot.png)

## Features

### Core Features
- 🎮 **Multi-Game Support**: Edit handling files from GTA:III, GTA:VC, and GTA:SA
- 📝 **Full Property Editor**: Edit all vehicle handling properties with appropriate controls
- 🎯 **Game-Aware UI**: Shows only relevant fields for each game version
- 💾 **Safe Editing**: Preserves original file structure, comments, and formatting
- 🔄 **Reset Support**: Easily reset individual vehicles to original values
- ⚡ **Real-time Validation**: Input validation based on official ranges

### UI/UX Features
- 🌙 **Dark Theme**: Professional dark UI with orange accent colors
- 📑 **Tabbed Editing**: Open multiple vehicles simultaneously for comparison
- 🔍 **Search**: Filter vehicles by name in the sidebar
- 📁 **Categorized List**: Vehicles grouped by type (Cars, Bikes, Boats, Planes, etc.)
- 📋 **Copy Values**: Copy handling values from another vehicle in the same category
- 📄 **Paste Config**: Paste raw handling.cfg lines directly from mod readmes
- 🎨 **Custom Title Bar**: Modern borderless window with custom controls
- 💬 **Themed Dialogs**: Custom styled confirmation and error dialogs

### Keyboard Shortcuts
- `Ctrl+O` - Open file
- `Ctrl+S` - Save file

## Screenshots

The application features a modern dark interface with:
- Categorized vehicle sidebar with search
- Tabbed multi-vehicle editing
- Organized property sections (Physics, Transmission, Suspension, etc.)
- Game-specific field visibility

## Requirements

- Windows 10 or later
- .NET 8.0 Runtime

## Installation

### From Release
1. Download the latest release ZIP from [Releases](https://github.com/vaibhavpandeyvpz/gtahandler/releases)
2. Extract to any folder
3. Run `GTAHandler.exe`

### Building from Source

```bash
# Clone the repository
git clone https://github.com/vaibhavpandeyvpz/gtahandler.git
cd gtahandler

# Build
dotnet build

# Run
dotnet run
```

Or open `GTAHandler.sln` in Visual Studio 2022.

## Usage

1. **Select Game**: Choose the game version (GTA:III, GTA:VC, or GTA:SA) from the dropdown
2. **Open File**: Click "Open" or press `Ctrl+O` to browse for a handling.cfg file
3. **Select Vehicle**: Click a vehicle from the categorized list on the left
4. **Edit Properties**: Modify values in the property editor tabs
5. **Copy Values** (Optional): Use "Copy From" to copy values from another vehicle
6. **Paste Config** (Optional): Paste a raw handling line from a mod's readme
7. **Save**: Click "Save" or press `Ctrl+S` to save changes

### Copy From Dialog

The "Copy From" feature allows you to:
- Select another vehicle from the same category to copy all values
- Paste a raw handling.cfg line directly (useful for applying mod values from readmes)
- The vehicle ID/name is preserved, only the handling values are copied

## Field Reference

### Physics
- **Mass**: Vehicle weight in kilograms (1.0 - 50000.0)
- **Turn Mass/Dimension X**: SA: Rotational inertia | GTA3/VC: Vehicle width
- **Drag Mult/Dimension Y**: SA: Air resistance | GTA3/VC: Vehicle length

### Centre of Mass
- **X/Y/Z Offset**: Centre of mass position (-10.0 to 10.0)

### Traction
- **Traction Multiplier**: Overall grip level (0.5 - 2.0)
- **Traction Loss**: Grip reduction during acceleration/braking (0.0 - 1.0)
- **Traction Bias**: Front/rear grip balance (0.0 = rear, 1.0 = front)

### Transmission
- **Number of Gears**: Transmission gears (1-5)
- **Max Velocity**: Top speed in km/h (5.0 - 300.0)
- **Engine Acceleration**: Power output (0.1 - 50.0)
- **Engine Inertia**: Flywheel mass (SA only)
- **Drive Type**: F (Front), R (Rear), 4 (All-wheel)
- **Engine Type**: P (Petrol), D (Diesel), E (Electric)

### Brakes
- **Brake Deceleration**: Stopping power (0.1 - 10.0)
- **Brake Bias**: Front/rear brake balance (0.0 - 1.0)
- **ABS**: Anti-lock braking system (on/off)

### Suspension
- **Force Level**: Spring stiffness
- **Damping Level**: Shock absorber strength
- **High Speed Damping**: Damping at high speeds (SA only)
- **Upper/Lower Limits**: Suspension travel range
- **Bias**: Front/rear suspension balance
- **Anti-Dive Multiplier**: Prevents nose-dive under braking (VC/SA)

### Miscellaneous
- **Seat Offset**: Driver position
- **Collision Damage Multiplier**: Damage sensitivity (0.2 - 5.0)
- **Monetary Value**: Vehicle price (1 - 100000)

### Flags
- **Model Flags**: Vehicle model behavior flags (hex)
- **Handling Flags**: Handling behavior flags (hex)

### Lights
- **Front/Rear Lights**: Light types (0-3)

### Animation (SA Only)
- **Anim Group**: Vehicle animation group index

## Architecture

The application follows the MVVM (Model-View-ViewModel) pattern:

```
gtahandler/
├── Models/          # Data models (VehicleHandling, GameType, etc.)
├── ViewModels/      # View logic (MainViewModel)
├── Views/           # XAML UI (MainWindow, Dialogs)
├── Services/        # File parsing/writing
├── Converters/      # WPF value converters
├── App.xaml         # Application resources & styles
└── *.png            # Game logo assets
```

## Technologies

- **WPF** - Windows Presentation Foundation
- **CommunityToolkit.Mvvm** - MVVM framework
- **FontAwesome.WPF** - Icon library

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Links

- [Website](https://vaibhavpandey.com/)
- [YouTube](https://www.youtube.com/channel/UC5uV1PRvtnNj9P8VfqO93Pw)
- [Report Issues](https://github.com/vaibhavpandeyvpz/gtahandler/issues)

## License

This project is provided for educational purposes under the MIT License.

GTA III, Vice City, and San Andreas are trademarks of Rockstar Games. All game logos are property of their respective owners and are used for illustration purposes only.

## Author

**Vaibhav Pandey (VPZ)**
- Email: contact@vaibhavpandey.com
- GitHub: [@vaibhavpandeyvpz](https://github.com/vaibhavpandeyvpz)
