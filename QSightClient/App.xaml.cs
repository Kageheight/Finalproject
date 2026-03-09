using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using QSightClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace QSightClient
{
    public partial class App : Application
    {
        private Window? _window;
        public static IPCService IPC { get; } = new();
        public static AgentService Agent { get; } = new();
        public static LogService Logs { get; } = new();

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            IPC.OnMessageReceived += Agent.HandleIPC;

            if(args.Arguments.Contains("--headless"))
            {
                RunHeadless(args.Arguments);
                return;
            }

            _window = new MainWindow();
            _window.Activate();

            _ = Task.Run(async () =>
            {
                await IPC.StartListening();
            });
        }

        private async void RunHeadless(string arguments)
        {
            var parts = arguments.Split("--scan");

            if(parts.Length > 1)
            {
                var path = parts[1].Trim().Trim('"');

                await Agent.StartHeadlessScan(path);
            }

            Environment.Exit(0);
        }
    }
}

