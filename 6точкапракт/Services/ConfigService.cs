using System;
using System.IO;
using System.Text.Json;
using ClassroomBooking.Configuration;

namespace ClassroomBooking.Services
{
    public class ConfigService
    {
        public AppSettings Load()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) return new AppSettings();

            try
            {
                string json = File.ReadAllText(path);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                return settings ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }
    }
}