using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace SensorDashboardWPF
{
    public partial class MainWindow : Window
    {
        // Sensors
        private TemperatureSensor temp;
        private PressureSensor pres;
        private HumiditySensor hum;
        private LightSensor light;
        private CO2Sensor co2;
        private NoiseSensor noise;
        
        // Timer for updates
        private DispatcherTimer updateTimer;
        private DateTime startTime;
        private bool isRunning;
        
        // Alert manager
        private AlertManager alertManager;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize sensors
            temp = new TemperatureSensor();
            pres = new PressureSensor();
            hum = new HumiditySensor();
            light = new LightSensor();
            co2 = new CO2Sensor();
            noise = new NoiseSensor();
            
            // Initialize alert manager
            alertManager = new AlertManager(soundEnabled: true);
            
            // Initialize timer
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(3);
            updateTimer.Tick += UpdateTimer_Tick;
            
            startTime = DateTime.Now;
            
            // Initial update
            UpdateDisplay();
        }
        
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            // Read all sensors
            double t = temp.Read();
            double p = pres.Read();
            double h = hum.Read();
            double l = light.Read();
            double c = co2.Read();
            double n = noise.Read();
            
            // Update header
            DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy    HH:mm:ss");
            TimeSpan uptime = DateTime.Now - startTime;
            UptimeText.Text = uptime.ToString(@"hh\:mm\:ss");
            
            // Update temperature
            TempValue.Text = $"{t:F1}°C";
            TempValue.Foreground = GetStatusColor(t, 70, 10);
            TempMin.Text = $"{temp.MinValue:F1}°C";
            TempMax.Text = $"{temp.MaxValue:F1}°C";
            TempAvg.Text = $"{temp.Average:F1}°C";
            TempProgress.Value = Math.Min(100, (t / 80) * 100);
            TempProgress.Foreground = GetStatusBrush(t, 70, 10);
            
            // Update pressure
            PressureValue.Text = $"{p:F1}";
            PressureValue.Foreground = p < 960 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            PressureMin.Text = $"{pres.MinValue:F1}";
            PressureMax.Text = $"{pres.MaxValue:F1}";
            PressureAvg.Text = $"{pres.Average:F1}";
            PressureProgress.Value = ((p - 950) / 50) * 100;
            PressureProgress.Foreground = p < 960 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            
            // Update humidity
            HumidityValue.Text = $"{h:F1}%";
            HumidityValue.Foreground = GetStatusColor(h, 85, 30);
            HumidityMin.Text = $"{hum.MinValue:F1}%";
            HumidityMax.Text = $"{hum.MaxValue:F1}%";
            HumidityAvg.Text = $"{hum.Average:F1}%";
            HumidityProgress.Value = h;
            HumidityProgress.Foreground = GetStatusBrush(h, 85, 30);
            
            // Update light
            LightValue.Text = $"{l:F0}";
            LightValue.Foreground = (l > 800 || l < 100) ? (SolidColorBrush)FindResource("WarningBrush") : (SolidColorBrush)FindResource("GoodBrush");
            LightMin.Text = $"{light.MinValue:F0}";
            LightMax.Text = $"{light.MaxValue:F0}";
            LightAvg.Text = $"{light.Average:F0}";
            LightProgress.Value = (l / 1000) * 100;
            LightProgress.Foreground = (l > 800 || l < 100) ? (SolidColorBrush)FindResource("WarningBrush") : (SolidColorBrush)FindResource("GoodBrush");
            
            // Update CO2
            CO2Value.Text = $"{c:F0}";
            CO2Value.Foreground = c > 1000 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            CO2Min.Text = $"{co2.MinValue:F0}";
            CO2Max.Text = $"{co2.MaxValue:F0}";
            CO2Avg.Text = $"{co2.Average:F0}";
            CO2Progress.Value = ((c - 300) / 1700) * 100;
            CO2Progress.Foreground = c > 1000 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            
            // Update noise
            NoiseValue.Text = $"{n:F0}";
            NoiseValue.Foreground = n > 70 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            NoiseMin.Text = $"{noise.MinValue:F0}";
            NoiseMax.Text = $"{noise.MaxValue:F0}";
            NoiseAvg.Text = $"{noise.Average:F0}";
            NoiseProgress.Value = (n / 120) * 100;
            NoiseProgress.Foreground = n > 70 ? (SolidColorBrush)FindResource("DangerBrush") : (SolidColorBrush)FindResource("GoodBrush");
            
            // Update alerts
            UpdateAlerts(t, p, h, l, c, n);
            
            // Process sound alerts
            alertManager.ProcessAlerts(t, p, h, l, c, n);
            
            // Log to CSV
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"{timestamp}, {t:F2}, {p:F2}, {h:F2}, {l:F0}, {c:F0}, {n:F0}";
            File.AppendAllText("C:\\Users\\ritik\\Desktop\\Data.csv", line + Environment.NewLine);
        }
        
        private void UpdateAlerts(double t, double p, double h, double l, double c, double n)
        {
            System.Text.StringBuilder alerts = new System.Text.StringBuilder();
            
            if (t > 70)
                alerts.AppendLine("🔴 HIGH TEMPERATURE (>70°C)");
            else if (t < 10)
                alerts.AppendLine("🟡 LOW TEMPERATURE (<10°C)");
            
            if (p < 960)
                alerts.AppendLine("🔴 LOW PRESSURE (<960 hPa) - Storm Possible!");
            
            if (h < 30)
                alerts.AppendLine("🟡 LOW HUMIDITY (<30%)");
            else if (h > 85)
                alerts.AppendLine("🔴 HIGH HUMIDITY (>85%)");
            
            if (l < 100)
                alerts.AppendLine("🟡 LOW LIGHT (<100 lux)");
            else if (l > 800)
                alerts.AppendLine("🔴 VERY BRIGHT LIGHT (>800 lux)");
            
            if (c > 1000)
                alerts.AppendLine($"🟡 HIGH CO2 ({c:F0} ppm)");
            if (c > 2000)
                alerts.AppendLine("🔴 DANGEROUS CO2 (>2000 ppm)");
            
            if (n > 70)
                alerts.AppendLine($"🟡 LOUD NOISE ({n:F0} dB)");
            if (n > 85)
                alerts.AppendLine("🔴 DANGEROUS NOISE (>85 dB)");
            
            if (alerts.Length == 0)
            {
                AlertsText.Text = "✅ All sensors operating within normal parameters";
                AlertsText.Foreground = (SolidColorBrush)FindResource("GoodBrush");
            }
            else
            {
                AlertsText.Text = alerts.ToString();
                AlertsText.Foreground = (SolidColorBrush)FindResource("TextBrush");
            }
        }
        
        private SolidColorBrush GetStatusColor(double value, double dangerHigh, double warningLow)
        {
            if (value > dangerHigh || value < warningLow)
                return (SolidColorBrush)FindResource("DangerBrush");
            if (value > dangerHigh * 0.9 || value < warningLow * 1.1)
                return (SolidColorBrush)FindResource("WarningBrush");
            return (SolidColorBrush)FindResource("GoodBrush");
        }
        
        private SolidColorBrush GetStatusBrush(double value, double dangerHigh, double warningLow)
        {
            return GetStatusColor(value, dangerHigh, warningLow);
        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                updateTimer.Start();
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                updateTimer.Stop();
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
        }
        
        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create screenshot
                string filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                
                // Capture window
                using (Bitmap bitmap = new Bitmap((int)ActualWidth, (int)ActualHeight))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen((int)Left, (int)Top, 0, 0, new System.Drawing.Size((int)ActualWidth, (int)ActualHeight));
                    }
                    bitmap.Save(filepath, ImageFormat.Png);
                }
                
                MessageBox.Show($"Screenshot saved to:\n{filepath}", "Screenshot", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screenshot:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = $"session_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                
                var sessionData = new
                {
                    SessionStart = startTime,
                    SessionEnd = DateTime.Now,
                    Duration = DateTime.Now - startTime,
                    Temperature = new { Min = temp.MinValue, Max = temp.MaxValue, Avg = temp.Average, Readings = temp.ReadCount },
                    Pressure = new { Min = pres.MinValue, Max = pres.MaxValue, Avg = pres.Average, Readings = pres.ReadCount },
                    Humidity = new { Min = hum.MinValue, Max = hum.MaxValue, Avg = hum.Average, Readings = hum.ReadCount },
                    Light = new { Min = light.MinValue, Max = light.MaxValue, Avg = light.Average, Readings = light.ReadCount },
                    CO2 = new { Min = co2.MinValue, Max = co2.MaxValue, Avg = co2.Average, Readings = co2.ReadCount },
                    Noise = new { Min = noise.MinValue, Max = noise.MaxValue, Avg = noise.Average, Readings = noise.ReadCount }
                };
                
                string json = System.Text.Json.JsonSerializer.Serialize(sessionData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filepath, json);
                
                MessageBox.Show($"Session data saved to:\n{filepath}", "Save Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
