using QSightClient.Models;
using QSightClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSightClient.Services
{
    public class AgentService
    {
        public event Action<IPCMessage>? OnScanRequested;
        private readonly ScanEngine _engine = new();
        public event Action<string>? OnScanStatusChanged;
        
        public AgentService()
        {
            _engine.OnStatusChanged += s =>
            {
                OnScanStatusChanged?.Invoke(s);
            };
        }

        public void HandleIPC(IPCMessage msg)
        {
            if (msg.Command == "SCAN")
            {
                _ = _engine.StartScan(msg.Path);
            }
        }
    }
}