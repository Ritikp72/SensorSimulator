using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

class Program
{
    // UI Colors
    const ConsoleColor HEADER_COLOR = ConsoleColor.Cyan;
    const ConsoleColor BORDER_COLOR = ConsoleColor.DarkGray;
    const ConsoleColor VALUE_COLOR = ConsoleColor.White;
    const ConsoleColor GOOD_COLOR = ConsoleColor.Green;
    const ConsoleColor WARNING_COLOR = ConsoleColor.Yellow;
    const ConsoleColor DANGER_COLOR = ConsoleColor.Red;
    const ConsoleColor LABEL_COLOR = ConsoleColor.Gray;

    // Interactive mode flag
    static bool interactiveMode = false;
    static int selectedSensor = 0;
    static List<string> menuOptions = new List<string>();

    static void Main()
    {
        // Load configuration
        AppConfig config = ConfigLoader.Load("config.json");
        
        // Initialize all sensors
        TemperatureSensor temp = new TemperatureSensor();
        PressureSensor pres = new PressureSensor();
        HumiditySensor hum = new HumiditySensor();
        LightSensor light = new LightSensor();
        CO2Sensor co2 = new CO2Sensor();
        NoiseSensor noise = new NoiseSensor();

        // Initialize alert manager for sound alerts
        AlertManager alertManager = new AlertManager(soundEnabled: true);
        
        // Initialize statistics manager for saving data
        StatisticsManager statsManager = new StatisticsManager("sensor_data");
        
        // Build menu options
        menuOptions = new List<string>
        {
            "Temperature",
            "Pressure", 
            "Humidity",
            "Light",
            "CO2",
            "Noise",
            "Toggle Sound Alerts",
            "Save Session to JSON",
            "Reset Statistics",
            "Exit"
        };

        DateTime sessionStartTime = DateTime.Now;

        // Setup console
        Console.Clear();
        Console.SetWindowSize(110, 50);
        Console.SetBufferSize(110, 1000);

        while (true)
        {
            // Read all sensor values
            double t = temp.Read();
            double p = pres.Read();
            double h = hum.Read();
            double l = light.Read();
            double c = co2.Read();
            double n = noise.Read();

            TimeSpan uptime = DateTime.Now - sessionStartTime;

            // Check for keyboard input in interactive mode
            if (interactiveMode && Console.KeyAvailable)
            {
                HandleKeyPress(Console.ReadKey(true), ref temp, ref pres, ref hum, ref light, ref co2, ref noise, ref alertManager, ref statsManager, sessionStartTime);
            }

            // Clear and draw UI
            if (config.DisplaySettings?.ClearScreen == true)
                Console.Clear();
            
            DrawHeader(uptime, config);

            // Draw all sensor cards in two rows
            Console.WriteLine();
            
            // First row - 3 sensors
            DrawCompactSensorCard("🌡️ TEMPERATURE", $"{t:F2}°C", temp.MinValue, temp.MaxValue, temp.Average, temp.ReadCount, temp.GetHistory(), 
                t > 70 ? DANGER_COLOR : (t < 10 ? WARNING_COLOR : GOOD_COLOR));
            
            DrawCompactSensorCard("🌡️ PRESSURE", $"{p:F2} hPa", pres.MinValue, pres.MaxValue, pres.Average, pres.ReadCount, pres.GetHistory(),
                p < 960 ? DANGER_COLOR : GOOD_COLOR);
            
            DrawCompactSensorCard("💧 HUMIDITY", $"{h:F1}%", hum.MinValue, hum.MaxValue, hum.Average, hum.ReadCount, hum.GetHistory(),
                h > 85 ? DANGER_COLOR : (h < 30 ? WARNING_COLOR : GOOD_COLOR));
            
            Console.WriteLine();
            
            // Second row - 3 sensors
            DrawCompactSensorCard("💡 LIGHT", $"{l:F0} lux", light.MinValue, light.MaxValue, light.Average, light.ReadCount, light.GetHistory(),
                l > 800 ? DANGER_COLOR : (l < 100 ? WARNING_COLOR : GOOD_COLOR));
            
            DrawCompactSensorCard("🌬️ CO2", $"{c:F0} ppm", co2.MinValue, co2.MaxValue, co2.Average, co2.ReadCount, co2.GetHistory(),
                c > 2000 ? DANGER_COLOR : (c > 1000 ? WARNING_COLOR : GOOD_COLOR));
            
            DrawCompactSensorCard("🔊 NOISE", $"{n:F0} dB", noise.MinValue, noise.MaxValue, noise.Average, noise.ReadCount, noise.GetHistory(),
                n > 85 ? DANGER_COLOR : (n > 70 ? WARNING_COLOR : GOOD_COLOR));

            // Draw progress bars for all sensors
            if (config.DisplaySettings?.ShowProgressBars == true)
            {
                Console.WriteLine();
                DrawProgressBarSection(t, p, h, l, c, n);
            }

            // Draw alerts section
            if (config.DisplaySettings?.ShowAlerts == true)
            {
                DrawAlertsSection(t, p, h, l, c, n);
            }
            
            // Process sound alerts
            alertManager.ProcessAlerts(t, p, h, l, c, n);

            // Draw interactive menu if enabled
            if (interactiveMode)
            {
                DrawInteractiveMenu();
            }

            // Draw footer
            DrawFooter(config);

            // Log to CSV
            if (config.AppSettings.LogToFile)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string line = $"{timestamp}, {t:F2}, {p:F2}, {h:F2}, {l:F0}, {c:F0}, {n:F0}";
                File.AppendAllText(config.AppSettings.LogFilePath, line + Environment.NewLine);
            }

            Thread.Sleep(config.AppSettings.UpdateIntervalSeconds * 1000);
        }
    }

    static void HandleKeyPress(ConsoleKeyInfo key, ref TemperatureSensor temp, ref PressureSensor pres, ref HumiditySensor hum, 
        ref LightSensor light, ref CO2Sensor co2, ref NoiseSensor noise, ref AlertManager alertMgr, ref StatisticsManager statsMgr, DateTime sessionStart)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                selectedSensor = (selectedSensor - 1 + menuOptions.Count) % menuOptions.Count;
                break;
            case ConsoleKey.DownArrow:
                selectedSensor = (selectedSensor + 1) % menuOptions.Count;
                break;
            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                ExecuteMenuAction(selectedSensor, ref temp, ref pres, ref hum, ref light, ref co2, ref noise, ref alertMgr, ref statsMgr, sessionStart);
                break;
            case ConsoleKey.Escape:
                interactiveMode = false;
                break;
        }
    }

    static void ExecuteMenuAction(int action, ref TemperatureSensor temp, ref PressureSensor pres, ref HumiditySensor hum,
        ref LightSensor light, ref CO2Sensor co2, ref NoiseSensor noise, ref AlertManager alertMgr, ref StatisticsManager statsMgr, DateTime sessionStart)
    {
        switch (action)
        {
            case 6: // Toggle Sound Alerts
                alertMgr.SoundEnabled = !alertMgr.SoundEnabled;
                Console.WriteLine($"✓ Sound alerts {(alertMgr.SoundEnabled ? "enabled" : "disabled")}!");
                Thread.Sleep(500);
                break;
            case 7: // Save Session
                statsMgr.SaveSession(sessionStart, temp, pres, hum, light, co2, noise);
                alertMgr.PlaySuccessSound();
                Thread.Sleep(1000);
                break;
            case 8: // Reset Statistics
                temp.Reset();
                pres.Reset();
                hum.Reset();
                light.Reset();
                co2.Reset();
                noise.Reset();
                alertMgr.PlaySuccessSound();
                Console.WriteLine("✓ All statistics reset!");
                Thread.Sleep(500);
                break;
            case 9: // Exit
                Console.WriteLine("\n👋 Goodbye!");
                Environment.Exit(0);
                break;
        }
    }

    static void DrawHeader(TimeSpan uptime, AppConfig config)
    {
        Console.ForegroundColor = HEADER_COLOR;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║                    🌡️  {config.AppSettings.AppName.ToUpper()}  🌡️                                           ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
        
        Console.ForegroundColor = LABEL_COLOR;
        Console.Write($"  📅 {DateTime.Now:yyyy-MM-dd}    ⏰ {DateTime.Now:HH:mm:ss}    ⏱️  Uptime: {uptime:hh\\:mm\\:ss}");
        
        if (interactiveMode)
        {
            Console.ForegroundColor = GOOD_COLOR;
            Console.Write("    🎮 INTERACTIVE MODE [ESC to exit]");
        }
        
        Console.WriteLine();
    }

    static void DrawCompactSensorCard(string title, string currentValue, double min, double max, double avg, int reads, string history, ConsoleColor statusColor)
    {
        Console.ForegroundColor = BORDER_COLOR;
        Console.Write("┌─────────────────────────────────────┐ ");
        Console.ResetColor();
    }

    static void DrawProgressBarSection(double temp, double pressure, double humidity, double light, double co2, double noise)
    {
        Console.ForegroundColor = HEADER_COLOR;
        Console.WriteLine("  ═══════════════════════════ SENSOR LEVELS ═══════════════════════════");
        
        DrawProgressBar("🌡️ Temperature", temp, 0, 80, WARNING_COLOR, DANGER_COLOR);
        DrawProgressBar("🌡️ Pressure", pressure, 950, 1000, GOOD_COLOR, DANGER_COLOR);
        DrawProgressBar("💧 Humidity", humidity, 0, 100, WARNING_COLOR, DANGER_COLOR);
        DrawProgressBar("💡 Light", light, 0, 1000, WARNING_COLOR, DANGER_COLOR);
        DrawProgressBar("🌬️ CO2", co2, 300, 2000, WARNING_COLOR, DANGER_COLOR);
        DrawProgressBar("🔊 Noise", noise, 20, 120, WARNING_COLOR, DANGER_COLOR);
        
        Console.WriteLine();
    }

    static void DrawProgressBar(string label, double value, double min, double max, ConsoleColor normalColor, ConsoleColor dangerColor)
    {
        int barWidth = 40;
        double percentage = Math.Max(0, Math.Min(1, (value - min) / (max - min)));
        int filledWidth = (int)(percentage * barWidth);
        
        Console.ForegroundColor = LABEL_COLOR;
        Console.Write($"  {label,-18} ");
        
        Console.ForegroundColor = BORDER_COLOR;
        Console.Write("[");
        
        for (int i = 0; i < barWidth; i++)
        {
            if (i < filledWidth)
            {
                // Color gradient based on percentage
                if (percentage > 0.85)
                    Console.ForegroundColor = dangerColor;
                else if (percentage > 0.7)
                    Console.ForegroundColor = WARNING_COLOR;
                else
                    Console.ForegroundColor = normalColor;
                Console.Write("█");
            }
            else
            {
                Console.ForegroundColor = BORDER_COLOR;
                Console.Write("░");
            }
        }
        
        Console.ForegroundColor = BORDER_COLOR;
        Console.Write("] ");
        
        Console.ForegroundColor = VALUE_COLOR;
        Console.WriteLine($"{value,7:F1}");
    }

    static void DrawAlertsSection(double t, double p, double h, double l, double co2, double n)
    {
        Console.ForegroundColor = HEADER_COLOR;
        Console.WriteLine("  ═══════════════════════════════════ ALERTS ═══════════════════════════════════");
        
        List<string> alerts = new List<string>();
        
        if (t > 70) alerts.Add("🔴 HIGH TEMPERATURE (>70°C)");
        else if (t < 10) alerts.Add("🟡 LOW TEMPERATURE (<10°C)");
        
        if (p < 960) alerts.Add("🔴 LOW PRESSURE (<960 hPa) - Storm Possible!");
        
        if (h < 30) alerts.Add("🟡 LOW HUMIDITY (<30%)");
        else if (h > 85) alerts.Add("🔴 HIGH HUMIDITY (>85%)");
        
        if (l < 100) alerts.Add("🟡 LOW LIGHT (<100 lux)");
        else if (l > 800) alerts.Add("🔴 VERY BRIGHT LIGHT (>800 lux)");
        
        if (co2 > 1000) alerts.Add("🟡 HIGH CO2 (>1000 ppm)");
        if (co2 > 2000) alerts.Add("🔴 DANGEROUS CO2 (>2000 ppm)");
        
        if (n > 70) alerts.Add("🟡 LOUD NOISE (>70 dB)");
        if (n > 85) alerts.Add("🔴 DANGEROUS NOISE (>85 dB)");
        
        if (alerts.Count == 0)
        {
            Console.ForegroundColor = GOOD_COLOR;
            Console.WriteLine("  ✅ All sensors operating within normal parameters");
        }
        else
        {
            foreach (var alert in alerts)
            {
                Console.ForegroundColor = alert.StartsWith("🔴") ? DANGER_COLOR : WARNING_COLOR;
                Console.WriteLine($"  {alert}");
            }
        }
        
        Console.WriteLine();
    }

    static void DrawInteractiveMenu()
    {
        Console.ForegroundColor = HEADER_COLOR;
        Console.WriteLine("  ═══════════════════════════ MENU ═══════════════════════════════════");
        
        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (i == selectedSensor)
            {
                Console.ForegroundColor = GOOD_COLOR;
                Console.Write($"  ► ");
            }
            else
            {
                Console.ForegroundColor = LABEL_COLOR;
                Console.Write($"    ");
            }
            
            string option = menuOptions[i];
            if (option == "Toggle Interactive Mode")
                option = (interactiveMode ? "⏹️ Disable" : "▶️ Enable") + " Interactive Mode";
            
            Console.WriteLine(option);
        }
        
        Console.ForegroundColor = LABEL_COLOR;
        Console.WriteLine();
        Console.WriteLine("  Use ↑↓ to navigate, ENTER to select, ESC to close menu");
        Console.WriteLine();
    }

    static void DrawFooter(AppConfig config)
    {
        Console.ForegroundColor = LABEL_COLOR;
        Console.WriteLine("  ─────────────────────────────────────────────────────────────────────────────────────────────────────");
        Console.Write($"  📁 Logging: {(config.AppSettings.LogToFile ? "ON" : "OFF")} | ");
        Console.Write($"  Interval: {config.AppSettings.UpdateIntervalSeconds}s | ");
        Console.Write($"  History: {config.AppSettings.HistorySize} readings | ");
        Console.WriteLine("🔄 Auto-refresh...");
    }
}
