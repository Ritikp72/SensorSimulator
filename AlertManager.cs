using System;
using System.Media;
using System.Collections.Generic;

/// <summary>
/// Manages audio alerts for sensor warnings
/// Uses System.Media for Windows beep sounds
/// </summary>
public class AlertManager
{
    private bool soundEnabled;
    private Dictionary<string, DateTime> lastAlertTime;
    private TimeSpan alertCooldown = TimeSpan.FromSeconds(5);
    
    public AlertManager(bool soundEnabled = true)
    {
        this.soundEnabled = soundEnabled;
        lastAlertTime = new Dictionary<string, DateTime>();
    }
    
    /// <summary>
    /// Play warning beep - different tones for different alert levels
    /// </summary>
    public void PlayWarningBeep()
    {
        if (!soundEnabled) return;
        
        try
        {
            // Windows beep with medium pitch
            Console.Beep(800, 200);
        }
        catch (Exception)
        {
            // Ignore sound errors
        }
    }
    
    /// <summary>
    /// Play danger beep - higher pitch alarm
    /// </summary>
    public void PlayDangerBeep()
    {
        if (!soundEnabled) return;
        
        try
        {
            // Two-tone alarm sound
            Console.Beep(1000, 150);
            Thread.Sleep(100);
            Console.Beep(1200, 150);
        }
        catch (Exception)
        {
            // Ignore sound errors
        }
    }
    
    /// <summary>
    /// Play critical alarm - continuous alert
    /// </summary>
    public void PlayCriticalAlarm()
    {
        if (!soundEnabled) return;
        
        try
        {
            // Three-tone urgent alarm
            Console.Beep(1200, 100);
            Thread.Sleep(50);
            Console.Beep(1200, 100);
            Thread.Sleep(50);
            Console.Beep(1200, 100);
        }
        catch (Exception)
        {
            // Ignore sound errors
        }
    }
    
    /// <summary>
    /// Play success sound - pleasant confirmation
    /// </summary>
    public void PlaySuccessSound()
    {
        if (!soundEnabled) return;
        
        try
        {
            // Rising tone for success
            Console.Beep(400, 100);
            Thread.Sleep(50);
            Console.Beep(600, 100);
        }
        catch (Exception)
        {
            // Ignore sound errors
        }
    }
    
    /// <summary>
    /// Check if alert can be triggered (cooldown check)
    /// </summary>
    public bool CanTriggerAlert(string alertId)
    {
        if (!lastAlertTime.ContainsKey(alertId))
            return true;
        
        return DateTime.Now - lastAlertTime[alertId] > alertCooldown;
    }
    
    /// <summary>
    /// Record that an alert was triggered
    /// </summary>
    public void RecordAlert(string alertId)
    {
        lastAlertTime[alertId] = DateTime.Now;
    }
    
    /// <summary>
    /// Enable or disable sound alerts
    /// </summary>
    public bool SoundEnabled
    {
        get => soundEnabled;
        set => soundEnabled = value;
    }
    
    /// <summary>
    /// Set cooldown between alerts (default 5 seconds)
    /// </summary>
    public TimeSpan AlertCooldown
    {
        get => alertCooldown;
        set => alertCooldown = value;
    }
    
    /// <summary>
    /// Process sensor alerts and play appropriate sounds
    /// Returns true if any alert was triggered
    /// </summary>
    public bool ProcessAlerts(double temp, double pressure, double humidity, double light, double co2, double noise)
    {
        bool anyAlert = false;
        
        // Temperature alerts
        if (temp > 70 && CanTriggerAlert("temp_high"))
        {
            PlayCriticalAlarm();
            RecordAlert("temp_high");
            anyAlert = true;
        }
        else if (temp < 10 && CanTriggerAlert("temp_low"))
        {
            PlayWarningBeep();
            RecordAlert("temp_low");
            anyAlert = true;
        }
        
        // Pressure alerts
        if (pressure < 960 && CanTriggerAlert("pressure_low"))
        {
            PlayDangerBeep();
            RecordAlert("pressure_low");
            anyAlert = true;
        }
        
        // Humidity alerts
        if ((humidity > 85 || humidity < 30) && CanTriggerAlert("humidity"))
        {
            PlayWarningBeep();
            RecordAlert("humidity");
            anyAlert = true;
        }
        
        // Light alerts
        if ((light > 800 || light < 100) && CanTriggerAlert("light"))
        {
            PlayWarningBeep();
            RecordAlert("light");
            anyAlert = true;
        }
        
        // CO2 alerts
        if (co2 > 2000 && CanTriggerAlert("co2_critical"))
        {
            PlayCriticalAlarm();
            RecordAlert("co2_critical");
            anyAlert = true;
        }
        else if (co2 > 1000 && CanTriggerAlert("co2_high"))
        {
            PlayDangerBeep();
            RecordAlert("co2_high");
            anyAlert = true;
        }
        
        // Noise alerts
        if (noise > 85 && CanTriggerAlert("noise_critical"))
        {
            PlayCriticalAlarm();
            RecordAlert("noise_critical");
            anyAlert = true;
        }
        else if (noise > 70 && CanTriggerAlert("noise_high"))
        {
            PlayWarningBeep();
            RecordAlert("noise_high");
            anyAlert = true;
        }
        
        return anyAlert;
    }
}
