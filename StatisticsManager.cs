using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

/// <summary>
/// Manages saving and loading sensor statistics to JSON files
/// Demonstrates JSON serialization in C#
/// </summary>
public class StatisticsManager
{
    private string dataDirectory;
    
    public StatisticsManager(string dataDirectory = "sensor_data")
    {
        this.dataDirectory = dataDirectory;
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
    }
    
    /// <summary>
    /// Data structure for saving session statistics
    /// </summary>
    public class SessionData
    {
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalReadings { get; set; }
        
        public SensorSnapshot Temperature { get; set; } = new SensorSnapshot();
        public SensorSnapshot Pressure { get; set; } = new SensorSnapshot();
        public SensorSnapshot Humidity { get; set; } = new SensorSnapshot();
        public SensorSnapshot Light { get; set; } = new SensorSnapshot();
        public SensorSnapshot CO2 { get; set; } = new SensorSnapshot();
        public SensorSnapshot Noise { get; set; } = new SensorSnapshot();
        
        public List<AlertRecord> Alerts { get; set; } = new List<AlertRecord>();
    }
    
    public class SensorSnapshot
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Average { get; set; }
        public DateTime MinTimestamp { get; set; }
        public DateTime MaxTimestamp { get; set; }
        public int ReadCount { get; set; }
        public List<double> History { get; set; } = new List<double>();
    }
    
    public class AlertRecord
    {
        public DateTime Timestamp { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public double Value { get; set; }
    }
    
    /// <summary>
    /// Save current session statistics to JSON file
    /// </summary>
    public void SaveSession(DateTime startTime, 
        TemperatureSensor temp, PressureSensor pres, HumiditySensor hum,
        LightSensor light, CO2Sensor co2, NoiseSensor noise)
    {
        try
        {
            SessionData session = new SessionData
            {
                SessionStart = startTime,
                SessionEnd = DateTime.Now,
                Duration = DateTime.Now - startTime,
                TotalReadings = temp.ReadCount + pres.ReadCount + hum.ReadCount
            };
            
            // Capture sensor snapshots
            session.Temperature = CreateSnapshot(temp.MinValue, temp.MaxValue, temp.Average, 
                temp.MinTimestamp, temp.MaxTimestamp, temp.ReadCount, temp.GetHistory());
            session.Pressure = CreateSnapshot(pres.MinValue, pres.MaxValue, pres.Average,
                pres.MinTimestamp, pres.MaxTimestamp, pres.ReadCount, pres.GetHistory());
            session.Humidity = CreateSnapshot(hum.MinValue, hum.MaxValue, hum.Average,
                hum.MinTimestamp, hum.MaxTimestamp, hum.ReadCount, hum.GetHistory());
            session.Light = CreateSnapshot(light.MinValue, light.MaxValue, light.Average,
                light.MinTimestamp, light.MaxTimestamp, light.ReadCount, light.GetHistory());
            session.CO2 = CreateSnapshot(co2.MinValue, co2.MaxValue, co2.Average,
                co2.MinTimestamp, co2.MaxTimestamp, co2.ReadCount, co2.GetHistory());
            session.Noise = CreateSnapshot(noise.MinValue, noise.MaxValue, noise.Average,
                noise.MinTimestamp, noise.MaxTimestamp, noise.ReadCount, noise.GetHistory());
            
            // Serialize to JSON
            string json = JsonSerializer.Serialize(session, new JsonSerializerOptions
            {
                WriteIndented = true  // Pretty print
            });
            
            // Save to file
            string filename = $"session_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string filepath = Path.Combine(dataDirectory, filename);
            File.WriteAllText(filepath, json);
            
            Console.WriteLine($"✓ Session saved to: {filepath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error saving session: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Load the most recent session from JSON file
    /// </summary>
    public SessionData? LoadMostRecentSession()
    {
        try
        {
            // Find most recent session file
            string[] files = Directory.GetFiles(dataDirectory, "session_*.json");
            if (files.Length == 0)
            {
                Console.WriteLine("⚠ No saved sessions found.");
                return null;
            }
            
            // Sort by name (date) descending
            Array.Sort(files);
            Array.Reverse(files);
            
            // Load most recent
            string json = File.ReadAllText(files[0]);
            return JsonSerializer.Deserialize<SessionData>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error loading session: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Load a specific session by filename
    /// </summary>
    public SessionData? LoadSession(string filename)
    {
        try
        {
            string filepath = Path.Combine(dataDirectory, filename);
            if (!File.Exists(filepath))
            {
                Console.WriteLine($"⚠ File not found: {filename}");
                return null;
            }
            
            string json = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<SessionData>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error loading session: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// List all saved sessions
    /// </summary>
    public List<string> ListSavedSessions()
    {
        try
        {
            string[] files = Directory.GetFiles(dataDirectory, "session_*.json");
            List<string> sessions = new List<string>();
            
            foreach (string file in files)
            {
                sessions.Add(Path.GetFileName(file));
            }
            
            sessions.Sort();
            sessions.Reverse();
            
            return sessions;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }
    
    /// <summary>
    /// Export all sensor readings to a simple CSV file
    /// </summary>
    public void ExportToCSV(string csvPath, double[] temp, double[] pressure, double[] humidity)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                writer.WriteLine("Timestamp,Temperature,Pressure,Humidity");
                
                DateTime start = DateTime.Now.AddSeconds(-temp.Length * 3);
                for (int i = 0; i < temp.Length; i++)
                {
                    writer.WriteLine($"{start.AddSeconds(i * 3):yyyy-MM-dd HH:mm:ss},{temp[i]:F2},{pressure[i]:F2},{humidity[i]:F2}");
                }
            }
            
            Console.WriteLine($"✓ CSV exported to: {csvPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Error exporting CSV: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Create a snapshot from current values
    /// </summary>
    private SensorSnapshot CreateSnapshot(double min, double max, double avg, 
        DateTime minTime, DateTime maxTime, int count, string historyStr)
    {
        return new SensorSnapshot
        {
            MinValue = min,
            MaxValue = max,
            Average = avg,
            MinTimestamp = minTime,
            MaxTimestamp = maxTime,
            ReadCount = count,
            History = ParseHistory(historyStr)
        };
    }
    
    /// <summary>
    /// Parse history string to list
    /// </summary>
    private List<double> ParseHistory(string historyStr)
    {
        List<double> result = new List<double>();
        string cleaned = historyStr.Trim('[', ']');
        
        if (string.IsNullOrWhiteSpace(cleaned))
            return result;
        
        string[] parts = cleaned.Split(',');
        foreach (string part in parts)
        {
            if (double.TryParse(part.Trim(), out double value))
            {
                result.Add(value);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Get the data directory path
    /// </summary>
    public string DataDirectory => dataDirectory;
}
