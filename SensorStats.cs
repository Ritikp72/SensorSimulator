using System;
using System.Collections.Generic;

/// <summary>
/// Tracks statistics for sensor readings including min, max, average, and history
/// </summary>
public class SensorStats
{
    private Queue<double> history;
    private int maxHistorySize;
    private double minValue;
    private double maxValue;
    private double sum;
    private int readCount;
    private DateTime minTimestamp;
    private DateTime maxTimestamp;
    private DateTime startTime;

    public SensorStats(int historySize = 10)
    {
        maxHistorySize = historySize;
        history = new Queue<double>(maxHistorySize);
        startTime = DateTime.Now;
        readCount = 0;
        sum = 0;
    }

    /// <summary>
    /// Updates statistics with a new reading
    /// </summary>
    public void Update(double value)
    {
        // Update min/max tracking
        if (readCount == 0)
        {
            minValue = maxValue = value;
            minTimestamp = maxTimestamp = DateTime.Now;
        }
        else
        {
            if (value < minValue)
            {
                minValue = value;
                minTimestamp = DateTime.Now;
            }
            if (value > maxValue)
            {
                maxValue = value;
                maxTimestamp = DateTime.Now;
            }
        }

        // Update sum for average calculation
        sum += value;
        readCount++;

        // Manage circular history buffer
        if (history.Count >= maxHistorySize)
        {
            history.Dequeue();
        }
        history.Enqueue(value);
    }

    /// <summary>
    /// Returns the current minimum value
    /// </summary>
    public double MinValue => readCount > 0 ? minValue : 0;

    /// <summary>
    /// Returns when the minimum was recorded
    /// </summary>
    public DateTime MinTimestamp => minTimestamp;

    /// <summary>
    /// Returns the current maximum value
    /// </summary>
    public double MaxValue => readCount > 0 ? maxValue : 0;

    /// <summary>
    /// Returns when the maximum was recorded
    /// </summary>
    public DateTime MaxTimestamp => maxTimestamp;

    /// <summary>
    /// Returns the average of all readings
    /// </summary>
    public double Average => readCount > 0 ? sum / readCount : 0;

    /// <summary>
    /// Returns the total number of readings
    /// </summary>
    public int ReadCount => readCount;

    /// <summary>
    /// Returns time since first reading
    /// </summary>
    public TimeSpan SessionDuration => DateTime.Now - startTime;

    /// <summary>
    /// Returns the history buffer as an array
    /// </summary>
    public double[] GetHistory()
    {
        return history.ToArray();
    }

    /// <summary>
    /// Returns history formatted as a string
    /// </summary>
    public string GetHistoryString(int decimalPlaces = 2)
    {
        var values = history.ToArray();
        if (values.Length == 0) return "[]";
        
        string[] strValues = Array.ConvertAll(values, v => v.ToString($"F{decimalPlaces}"));
        return "[" + string.Join(", ", strValues) + "]";
    }

    /// <summary>
    /// Resets all statistics
    /// </summary>
    public void Reset()
    {
        history.Clear();
        readCount = 0;
        sum = 0;
        startTime = DateTime.Now;
    }
}
