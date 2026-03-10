using Microsoft.UI.Xaml.Controls;
using QSightClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QSightClient.Pages
{
    public sealed partial class LogsPage : Page
    {
        public LogsPage()
        {
            InitializeComponent();

            LoadLogs();
        }

        private void LoadLogs()
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "logs");

            if (!Directory.Exists(dir))
                return;

            var list = new List<ScanLog>();

            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                var json = File.ReadAllText(file);
                var log = JsonSerializer.Deserialize<ScanLog>(json);

                if (log != null)
                    list.Add(log);
            }

            LogsList.ItemsSource = list;
        }

        private void LogsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ScanLog log)
            {
                Frame.Navigate(typeof(LogDetailPage), log);
            }
        }
    }
}