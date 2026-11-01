using System;
using System.IO;
using System.Text.Json;

namespace VolumeScroller.Config
{
    public static class SettingsManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static string GetSettingsFilePath()
        {
            // Portable mode check: if settings.json exists right next to the executable, use it
            string localFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            if (File.Exists(localFile))
            {
                return localFile;
            }

            // Standard Windows application data path: %LocalAppData%\VolumeScroller\settings.json
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDir = Path.Combine(appData, "VolumeScroller");
            if (!Directory.Exists(appDir))
            {
                try
                {
                    Directory.CreateDirectory(appDir);
                }
                catch { }
            }

            return Path.Combine(appDir, "settings.json");
        }

        public static AppSettings Load()
        {
            try
            {
                string path = GetSettingsFilePath();
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null && settings.ShortcutKeys != null && settings.ShortcutKeys.Count > 0)
                    {
                        return settings;
                    }
                }
            }
            catch { }

            // Default fallback: Ctrl + Alt
            var fallback = new AppSettings();
            Save(fallback);
            return fallback;
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                string path = GetSettingsFilePath();
                string? dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(path, json);
            }
            catch { }
        }
    }
}
