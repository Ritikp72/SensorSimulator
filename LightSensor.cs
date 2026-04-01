using System;

/// <summary>
/// Light sensor that measures ambient light in lux (0-1000 lux)
/// Inherits from SensorBase - demonstrates OOP inheritance
/// </summary>
public class LightSensor : SensorBase
{
    // Override abstract properties
    public override string Name => "💡 LIGHT SENSOR";
    public override string Unit => "lux";
    public override double MinPossible => 0;
    public override double MaxPossible => 1000;
    
    // Override virtual properties for thresholds
    public override double WarningThreshold { get; set; } = 100;   // Low light warning
    public override double DangerThreshold { get; set; } = 800;    // Very bright warning
    
    // Constructor - calls base constructor
    public LightSensor() : base(historySize: 10)
    {
    }
    
    // Implement abstract method
    public override double GenerateValue()
    {
        // Simulate indoor/outdoor light levels (50-800 lux typical)
        return rand.NextDouble() * 750 + 50;
    }
    
    // Override status icon based on light level
    public override string GetStatusIcon()
    {
        double recentAvg = Average;
        if (recentAvg >= DangerThreshold)
            return "🔴"; // Too bright
        if (recentAvg <= WarningThreshold)
            return "🟡"; // Too dark
        return "🟢"; // Good light
    }
    
    // Helper method to get light description
    public string GetLightDescription()
    {
        double current = MinValue; // Last reading
        if (current < 50) return "Dark";
        if (current < 200) return "Dim";
        if (current < 400) return "Normal indoor";
        if (current < 600) return "Bright indoor";
        return "Very bright";
    }
}
