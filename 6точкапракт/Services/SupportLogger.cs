using System;
using System.IO;

namespace ClassroomBooking.Services
{
    public static class SupportLogger
    {
        private static string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClassroomBooking", "Logs");
        public static string LogFilePath => Path.Combine(LogDirectory, "application.log");

        public static void Info(string message) => Write("INFO", message);
        public static void Warning(string message) => Write("WARNING", message);
        public static void Error(string message, Exception ex) => Write("ERROR", message + Environment.NewLine + ex.ToString());

        private static void Write(string level, string message)
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, line);
            }
            catch { /* Ошибка лога не должна ронять программу */ }
        }
    }
}