using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Security.Cryptography;
using Windows.Storage.Pickers;

namespace QSightClient.Pages
{
    public sealed partial class ScanPage : Page
    {
        private string? _selectedFilePath;
        private string? _currentScanId;

        public ScanPage()
        {
            InitializeComponent();
        }

        private async void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");

            // WinUI3 requires window handle
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            _selectedFilePath = file.Path;
            SelectedFileText.Text = file.Name;
            StartScanButton.IsEnabled = true;

            ScanStatusText.Text = "-";
            ScanSeverityText.Text = "-";
            StaticResultText.Text = "-";
            ScanIdText.Text = "-";
        }

        private async void StartScan_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFilePath == null) return;

            StartScanButton.IsEnabled = false;
            SelectFileButton.IsEnabled = false;
            ScanStatusText.Text = "분석 중";

            try
            {
                // SHA256 계산
                var sha256 = ComputeSha256(_selectedFilePath);
                var fileName = Path.GetFileName(_selectedFilePath);

                // 스캔 생성
                var scanId = await App.Api.CreateScanAsync("EMP001", fileName, sha256);

                if (scanId == null)
                {
                    ScanStatusText.Text = "스캔 생성 실패";
                    return;
                }

                _currentScanId = scanId;
                ScanIdText.Text = scanId;
                ScanStatusText.Text = "서버 전송 완료, 분석 대기 중...";

                // 결과 조회
                var result = await App.Api.GetScanResultAsync(scanId);
                if (result?.scan != null)
                {
                    ScanStatusText.Text = result.scan.status switch
                    {
                        "done" => "완료",
                        "analyzing" => "분석 중",
                        "failed" => " 실패",
                        _ => result.scan.status ?? "-"
                    };
                    ScanSeverityText.Text = result.scan.severity ?? "-";
                    StaticResultText.Text = result.scan.static_result ?? "-";
                }
            }
            catch (Exception ex)
            {
                ScanStatusText.Text = $"오류: {ex.Message}";
            }
            finally
            {
                StartScanButton.IsEnabled = true;
                SelectFileButton.IsEnabled = true;
            }
        }

        private static string ComputeSha256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}