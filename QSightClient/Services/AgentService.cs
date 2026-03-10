using QSightClient.Models;
using QSightClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace QSightClient.Services
{
    public class AgentService
    {
        private readonly ScanEngine _engine = new();
        private readonly ConcurrentQueue<ScanRequest> _queue = new();
        private bool _isProcessing = false;
        public event Action<IPCMessage>? OnScanRequested;
        public event Action<string>? OnScanStatusChanged;
        public event Action<int>? OnScanProgressChanged;
        public event Action<List<string>>? OnQueueChanged;

        public AgentService()
        {
            _engine.OnStatusChanged += s =>
            {
                OnScanStatusChanged?.Invoke(s);
            };

            _engine.OnProgressChanged += p =>
            {
                OnScanProgressChanged?.Invoke(p);
            };
        }

        public void HandleIPC(IPCMessage msg)
        {
            if (msg.Command == "SCAN")
            {
                _queue.Enqueue(new ScanRequest{ Path = msg.Path });

                NotifyQueue();
                ProcessQueue();
            }
        }

        public async Task StartHeadlessScan(string path)
        {
            await _engine.StartScan(path);
        }

        private void NotifyQueue()
        {
            var list = _queue.Select(q => q.Path).ToList();
            OnQueueChanged?.Invoke(list);
        }

        private async void ProcessQueue()
        {
            if (_isProcessing)
                return;

            _isProcessing = true;

            while (_queue.TryDequeue(out var req))
            {
                NotifyQueue();
                await _engine.StartScan(req.Path);
            }

            _isProcessing = false;
        }

        public void CancelScan()
        {
            _engine.CancelScan();
        }
    }
}