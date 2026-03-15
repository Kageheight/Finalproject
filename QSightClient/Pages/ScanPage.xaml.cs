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
            ScanStatusText.Text = "분석 중 version2";

            try
            {
                var sha256 = ComputeSha256(_selectedFilePath);
                var fileName = Path.GetFileName(_selectedFilePath);

                var scanId = await App.Api.CreateScanAsync("EMP001", fileName, sha256);
                if (scanId == null)
                {
                    ScanStatusText.Text = "스캔 생성 실패";
                    return;
                }

                _currentScanId = scanId;
                ScanIdText.Text = scanId;
                await App.Api.CompleteScanAsync(scanId, fileName);

                await System.Threading.Tasks.Task.Delay(15000);

                var result = await App.Api.GetScanResultAsync(scanId);
                var staticResult = result?.scan?.static_result ?? "unknown";

                ScanStatusText.Text = "완료";
                ScanSeverityText.Text = result?.scan?.severity ?? "-";
                StaticResultText.Text = staticResult;

                // 로그 저장
                App.Logs.SaveLog(new QSightClient.Models.ScanLog
                {
                    FilePath = _selectedFilePath,
                    FileName = fileName,
                    ScanId = scanId,
                    StaticResult = staticResult,
                    Timestamp = DateTime.Now,
                    ScanTime = DateTime.Now,
                    Result = staticResult
                });

                // 파일로 직접 확인
                File.WriteAllText(
                    @"C:\Users\jeang\Desktop\qsight_log_test.txt",
                    $"파일:{fileName}, 결과:{staticResult}, 로그수:{App.Logs.Logs.Count}, 시간:{DateTime.Now}"
                );

                ScanStatusText.Text = $"완료 - {staticResult}";
            }
            catch (Exception ex)
            {
                File.WriteAllText(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "qsight_error.txt"),
                    ex.ToString()
                );
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