using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Configuration classes for loading settings from config.json
/// Demonstrates JSON deserialization in C#
/// </summary>
public class AppConfig
{
    public AppSettings AppSettings { get; set; } = new AppSettings();
    public SensorConfig Sensors { get; set; } = new SensorConfig();
    public DisplaySettings DisplaySettings { get; set; } = new DisplaySettings();
}

public class AppSettings
{
    public string AppName { get; set; } = "Sensor Simulator";
    public int UpdateIntervalSeconds { get; set; } = 3;
    public bool LogToFile { get; set; } = true;
    public string LogFilePath { get; set; } = "C:\\Users\\ritik\\Desktop\\Data.csv";
    public int HistorySize { get; set; } = 10;
}

public class SensorConfig
{
    public SensorSettings Temperature { get; set; } = new SensorSettings();
    public SensorSettings Humidity { get; set; } = new SensorSettings();
    public SensorSettings Pressure { get; set; } = new SensorSettings();
    public SensorSettings Light { get; set; } = new SensorSettings();
    public SensorSettings CO2 { get; set; } = new SensorSettings();
    public SensorSettings Noise { get; set; } = new SensorSettings();
}

public class SensorSettings
{
    public bool Enabled { get; set; } = true;
    public double MinValue { get; set; } = 0;
    public double MaxValue { get; set; } = 100;
    public double WarningThreshold { get; set; } = 70;
    public double DangerThreshold { get; set; } = 90;
    public double WarningThresholdLow { get; set; } = 30;
    public double WarningThresholdHigh { get; set; } = 70;
}

public class DisplaySettings
{
    public bool ShowProgressBars { get; set; } = true;
    public bool ShowStatistics { get; set; } = true;
    public bool ShowAlerts { get; set; } = true;
    public bool ClearScreen { get; set; } = true;
}

/// <summary>
/// ConfigLoader - loads and provides access to configuration
/// </summary>
public static class ConfigLoader
{
    private static AppConfig _config;
    private static string _configPath = "config.json";
    
    public static AppConfig Load(string configPath = "config.json")
    {
        _configPath = configPath;
        
        try
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json);
                Console.WriteLine($"✓ Configuration loaded from {_configPath}");
                return _config;
            }
            else
            {
                Console.WriteLine($"⚠ Config file not found: {_configPath}");
                Console.WriteLine("  Using default configuration...");
                return GetDefaultConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error loading config: {ex.Message}");
            Console.WriteLine("  Using default configuration...");
            return GetDefaultConfig();
        }
    }
    
    public static AppConfig GetConfig()
    {
        if (_config == null)
        {
            return Load(_configPath);
        }
        return _config;
    }
    
    private static AppConfig GetDefaultConfig()
    {
        // Properties now have default values, so we can just return a new instance
        return new AppConfig();
    }
}
