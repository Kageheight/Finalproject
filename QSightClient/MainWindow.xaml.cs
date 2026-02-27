using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using QSightClient.Services;
using QSightClient.Pages;
using QSightClient.Models;

namespace QSightClient
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Q-Sight Agent";

            App.IPC.OnMessageReceived += IPC_ONMessageReceived;
            //RootNav.SelectionChanged += RootNav_SelectionChanged;
            //ContentFrame.Navigate(typeof(StatusPage));
        }

        private void IPC_ONMessageReceived(IPCMessage message)
        {
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                ContentFrame.Navigate(typeof(StatusPage), message);
            });
        }

        private void RootNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;

            switch (item?.Content?.ToString())
            {
                case "Status":
                    ContentFrame.Navigate(typeof(StatusPage));
                    break;

                case "Scan":
                    ContentFrame.Navigate(typeof(ScanPage));
                    break;

                case "Logs":
                    ContentFrame.Navigate(typeof(LogsPage));
                    break;

                case "About":
                    ContentFrame.Navigate(typeof(AboutPage));
                    break;
            }
        }
    }
}