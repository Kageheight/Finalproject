using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QSightClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace QSightClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatusPage : Page
    {
        public StatusPage()
        {
            InitializeComponent();

            NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;

            App.IPC.OnMessageReceived += IPC_OnMessageReceived;
        }

        private void IPC_OnMessageReceived(IPCMessage msg)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LastMessageText.Text = $"{msg.Command} : {msg.Path}";
            });
        }
    }
}
