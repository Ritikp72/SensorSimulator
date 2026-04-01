using System;

/// <summary>
/// Abstract base class for all sensors.
/// Demonstrates OOP inheritance - all sensors inherit from this class.
/// </summary>
public abstract class SensorBase
{
    // Protected fields - accessible by derived classes
    protected Random rand;
    protected SensorStats stats;
    
    // Properties that derived classes must implement
    public abstract string Name { get; }
    public abstract string Unit { get; }
    public abstract double MinPossible { get; }
    public abstract double MaxPossible { get; }
    
    // Virtual properties - can be overridden but have default implementation
    public virtual double WarningThreshold { get; set; }
    public virtual double DangerThreshold { get; set; }
    
    // Statistics properties (using composition with SensorStats)
    public double MinValue => stats.MinValue;
    public double MaxValue => stats.MaxValue;
    public double Average => stats.Average;
    public int ReadCount => stats.ReadCount;
    public string GetHistory() => stats.GetHistoryString();
    public DateTime MinTimestamp => stats.MinTimestamp;
    public DateTime MaxTimestamp => stats.MaxTimestamp;
    public TimeSpan SessionDuration => stats.SessionDuration;
    
    // Constructor
    public SensorBase(int historySize = 10)
    {
        rand = new Random();
        stats = new SensorStats(historySize);
    }
    
    /// <summary>
    /// Abstract method - must be implemented by derived classes
    /// </summary>
    public abstract double GenerateValue();
    
    /// <summary>
    /// Reads the sensor and updates statistics
    /// </summary>
    public double Read()
    {
        double value = GenerateValue();
        stats.Update(value);
        return value;
    }
    
    /// <summary>
    /// Virtual method - can be overridden by derived classes
    /// </summary>
    public virtual string GetStatusIcon()
    {
        double current = ReadCount > 0 ? MinValue : 0;
        // Override in derived classes for specific thresholds
        return "⚪";
    }
    
    /// <summary>
    /// Checks if current value is in warning/danger range
    /// </summary>
    public string GetStatus(double currentValue)
    {
        if (currentValue >= DangerThreshold)
            return "🔴 DANGER";
        if (currentValue <= WarningThreshold)
            return "🟡 WARNING";
        return "🟢 NORMAL";
    }
    
    /// <summary>
    /// Resets all statistics
    /// </summary>
    public void Reset()
    {
        stats.Reset();
    }
}
