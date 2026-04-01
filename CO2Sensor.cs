using System;

/// <summary>
/// CO2 sensor that measures carbon dioxide in parts per million (ppm)
/// Inherits from SensorBase - demonstrates OOP inheritance
/// </summary>
public class CO2Sensor : SensorBase
{
    // Override abstract properties
    public override string Name => "🌬️ CO2 SENSOR";
    public override string Unit => "ppm";
    public override double MinPossible => 300;
    public override double MaxPossible => 5000;
    
    // Override virtual properties for thresholds (CO2 levels)
    public override double WarningThreshold { get; set; } = 1000;   // High CO2 warning (indoor air quality)
    public override double DangerThreshold { get; set; } = 2000;   // Dangerous CO2 level
    
    // Constructor - calls base constructor
    public CO2Sensor() : base(historySize: 10)
    {
    }
    
    // Implement abstract method
    public override double GenerateValue()
    {
        // Simulate typical indoor CO2 (400-2000 ppm)
        // 400 = outdoor fresh air
        // 1000+ = poor ventilation
        // 2000+ = dangerous
        return rand.NextDouble() * 600 + 400;
    }
    
    // Override status icon based on CO2 level
    public override string GetStatusIcon()
    {
        double recentAvg = Average;
        if (recentAvg >= DangerThreshold)
            return "🔴"; // Dangerous
        if (recentAvg >= WarningThreshold)
            return "🟡"; // High
        return "🟢"; // Good
    }
    
    // Helper method to get air quality description
    public string GetAirQuality()
    {
        double current = MinValue; // Last reading
        if (current < 600) return "Excellent";
        if (current < 800) return "Good";
        if (current < 1000) return "Fair";
        if (current < 1500) return "Poor";
        if (current < 2000) return "Very Poor";
        return "Dangerous!";
    }
}
