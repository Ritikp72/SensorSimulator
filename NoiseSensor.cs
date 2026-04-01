using System;

/// <summary>
/// Noise sensor that measures sound levels in decibels (dB)
/// Inherits from SensorBase - demonstrates OOP inheritance
/// </summary>
public class NoiseSensor : SensorBase
{
    // Override abstract properties
    public override string Name => "🔊 NOISE SENSOR";
    public override string Unit => "dB";
    public override double MinPossible => 20;
    public override double MaxPossible => 120;
    
    // Override virtual properties for thresholds (sound levels)
    public override double WarningThreshold { get; set; } = 70;    // Loud warning
    public override double DangerThreshold { get; set; } = 85;    // Hearing damage risk
    
    // Constructor - calls base constructor
    public NoiseSensor() : base(historySize: 10)
    {
    }
    
    // Implement abstract method
    public override double GenerateValue()
    {
        // Simulate typical noise levels (30-90 dB)
        // 30 = quiet library
        // 60 = normal conversation
        // 85 = busy traffic
        // 100+ = very loud
        return rand.NextDouble() * 50 + 30;
    }
    
    // Override status icon based on noise level
    public override string GetStatusIcon()
    {
        double recentAvg = Average;
        if (recentAvg >= DangerThreshold)
            return "🔴"; // Dangerous
        if (recentAvg >= WarningThreshold)
            return "🟡"; // Loud
        return "🟢"; // Normal
    }
    
    // Helper method to get noise description
    public string GetNoiseDescription()
    {
        double current = MinValue; // Last reading
        if (current < 30) return "Very Quiet";
        if (current < 50) return "Quiet";
        if (current < 60) return "Normal";
        if (current < 70) return "Loud";
        if (current < 85) return "Very Loud";
        return "Dangerous!";
    }
}
