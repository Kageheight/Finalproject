using System;
using System.Threading.Tasks;

namespace QSightClient.Services
{
    public class ScanEngine
    {
        public event Action<string>? OnStatusChanged;

        public async Task StartScan(string path)
        {
            OnStatusChanged?.Invoke("Scan Started");

            await Task.Delay(1000);

            OnStatusChanged?.Invoke("Analyzing file...");

            await Task.Delay(2000);

            OnStatusChanged?.Invoke("Collecting results...");

            await Task.Delay(1500);

            OnStatusChanged?.Invoke("Scan Complete");
        }
    }
}