using System;
using Microsoft.Win32;

namespace VolumeScroller.Startup
{
    public static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "VolumeScroller";

        public static bool IsRunAtStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
                var value = key?.GetValue(AppName) as string;
                return !string.IsNullOrWhiteSpace(value);
            }
            catch
            {
                return false;
            }
        }

        public static bool SetRunAtStartup(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
                if (key == null) return false;

                if (enable)
                {
                    string? exePath = Environment.ProcessPath;
                    if (string.IsNullOrWhiteSpace(exePath)) return false;

                    // Wrap in quotes to handle paths with spaces safely
                    key.SetValue(AppName, $"\"{exePath}\"");
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
