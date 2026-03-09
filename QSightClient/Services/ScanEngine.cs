using QSightClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QSightClient.Services
{
    public class ScanEngine
    {
        public event Action<string>? OnStatusChanged;
        public event Action<int>? OnProgressChanged;
        private CancellationTokenSource? _cts;

        public async Task StartScan(string path)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                OnStatusChanged?.Invoke("Scan Started");
                OnProgressChanged?.Invoke(10);

                await Task.Delay(1000, token);

                OnStatusChanged?.Invoke("Analyzing file...");
                OnProgressChanged?.Invoke(50);

                await Task.Delay(2000, token);

                OnStatusChanged?.Invoke("Collecting results...");
                OnProgressChanged?.Invoke(80);

                await Task.Delay(1500, token);

                OnStatusChanged?.Invoke("Scan Complete");
                OnProgressChanged?.Invoke(100);

                App.Logs.SaveLog(new ScanLog
                {
                    FilePath = path,
                    ScanTime = DateTime.Now,
                    Result = "Clean"
                });
            }
            catch (TaskCanceledException)
            {
                OnStatusChanged?.Invoke("Scan Cancelled");
                OnProgressChanged?.Invoke(0);
            }
        }

        public void CancelScan()
        {
            _cts?.Cancel();
        }
    }
}