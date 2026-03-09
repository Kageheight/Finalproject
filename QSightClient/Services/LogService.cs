using QSightClient.Models;
using System;
using System.IO;
using System.Text.Json;

namespace QSightClient.Services
{
    public class LogService
    {
        private readonly string _logDir = Path.Combine(AppContext.BaseDirectory, "logs");

        public LogService()
        {
            Directory.CreateDirectory(_logDir);
        }

        public void SaveLog(ScanLog log)
        {
            var file = Path.Combine(_logDir, $"{DateTime.Now:yyyyMMdd_HHmmss}.json");

            var json = JsonSerializer.Serialize(
                log,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(file, json);
        }
    }
}