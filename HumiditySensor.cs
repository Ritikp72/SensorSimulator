using System;

public class HumiditySensor
{
    private Random rand = new Random();
    private SensorStats stats;

    public HumiditySensor()
    {
        stats = new SensorStats(historySize: 10);
    }

    public double Read()
    {
        // Generate humidity reading (30-90% range)
        double value = rand.NextDouble() * 60 + 30;
        
        // Update statistics
        stats.Update(value);
        
        return value;
    }

    // Statistics access properties
    public double MinValue => stats.MinValue;
    public double MaxValue => stats.MaxValue;
    public double Average => stats.Average;
    public int ReadCount => stats.ReadCount;
    public string GetHistory() => stats.GetHistoryString();
    public DateTime MinTimestamp => stats.MinTimestamp;
    public DateTime MaxTimestamp => stats.MaxTimestamp;
    public TimeSpan SessionDuration => stats.SessionDuration;
    
    // Reset statistics
    public void Reset()
    {
        stats.Reset();
    }
}