# SensorSimulator

A comprehensive C# sensor simulation application that simulates multiple environmental sensors with real-time statistics, sound alerts, and a WPF-based graphical dashboard.

## Features

### 🚀 Core Functionality
- **6 Sensor Types**: Temperature, Humidity, Pressure, Light, CO2, and Noise sensors
- **Real-time Simulation**: Each sensor generates realistic data with configurable update intervals
- **Statistics Tracking**: Min, max, average, and standard deviation for all sensor readings
- **Configurable Alerts**: Threshold-based alerts with sound notifications

### 💾 Data Persistence
- **JSON Save/Load**: Save and load sensor data in JSON format
- **Configuration System**: Customize sensor parameters via `config.json`
- **Auto-save**: Optional automatic data persistence

### 🎨 User Interface
- **Console Application**: Full-featured command-line interface with interactive menus
- **WPF Dashboard**: Modern graphical user interface with real-time charts
- **Screenshot Capture**: Save dashboard views as images

## Project Structure

```
SensorSimulator/
├── SensorBase.cs           # Base class for all sensors
├── CO2Sensor.cs            # CO2 sensor implementation
├── HumiditySensor.cs       # Humidity sensor implementation
├── LightSensor.cs          # Light sensor implementation
├── NoiseSensor.cs          # Noise sensor implementation
├── PressureSensor.cs       # Pressure sensor implementation
├── TempretureSensor.cs     # Temperature sensor implementation
├── SensorStats.cs          # Statistics tracking model
├── StatisticsManager.cs    # Statistics calculation and management
├── AlertManager.cs         # Alert handling and sound notifications
├── ConfigLoader.cs         # Configuration file management
├── Program.cs              # Console application entry point
├── config.json             # Configuration file
├── SensorSimulator.csproj  # Main project file
└── SensorDashboardWPF/     # WPF Dashboard application
    ├── App.xaml             # Application definition
    ├── MainWindow.xaml      # Main window UI
    └── SensorDashboardWPF.csproj
```

## Requirements

- .NET 8.0 SDK or later
- Windows (for WPF GUI and sound alerts)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SensorSimulator.git
   cd SensorSimulator
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

## Usage

### Console Application

Run the console simulator:
```bash
dotnet run --project SensorSimulator.csproj
```

**Available Commands:**
- `1` - Start simulation
- `2` - Stop simulation
- `3` - Show current readings
- `4` - Show statistics
- `5` - Save data to JSON
- `6` - Load data from JSON
- `7` - Configure alerts
- `8` - Run demo mode
- `9` - Run benchmark
- `0` - Exit

### WPF Dashboard

Launch the graphical dashboard:
```bash
dotnet run --project SensorDashboardWPF/SensorDashboardWPF.csproj
```

**Dashboard Features:**
- Real-time sensor readings display
- Live charts for each sensor type
- Alert status indicators
- Screenshot capture button
- Statistics summary panel

## Configuration

Edit `config.json` to customize:

```json
{
  "updateIntervalMs": 1000,
  "alerts": {
    "enableSound": true,
    "temperatureThreshold": 30.0,
    "humidityThreshold": 80.0,
    "co2Threshold": 1000.0,
    "noiseThreshold": 85.0,
    "pressureThreshold": 1100.0,
    "lightThreshold": 1000.0
  },
  "sensors": {
    "temperature": { "min": -10, "max": 50, "unit": "°C" },
    "humidity": { "min": 0, "max": 100, "unit": "%" },
    "pressure": { "min": 950, "max": 1050, "unit": "hPa" },
    "light": { "min": 0, "max": 1000, "unit": "lux" },
    "co2": { "min": 400, "max": 2000, "unit": "ppm" },
    "noise": { "min": 20, "max": 120, "unit": "dB" }
  },
  "autoSave": {
    "enabled": true,
    "intervalSeconds": 300
  }
}
```

## Sensor Types

| Sensor | Unit | Typical Range | Description |
|--------|------|---------------|-------------|
| Temperature | °C | -10 to 50 | Ambient temperature |
| Humidity | % | 0 to 100 | Relative humidity |
| Pressure | hPa | 950 to 1050 | Atmospheric pressure |
| Light | lux | 0 to 1000 | Light intensity |
| CO2 | ppm | 400 to 2000 | Carbon dioxide level |
| Noise | dB | 20 to 120 | Sound pressure level |


