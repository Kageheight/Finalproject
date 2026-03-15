using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QSightClient.Services
{
	public class WatcherService
	{
		private FileSystemWatcher? _watcher;
		private readonly ApiService _api;

		public event Action<string>? OnFileDetected;
		public event Action<string, string>? OnScanComplete; // fileName, result

		public WatcherService(ApiService api)
		{
			_api = api;
		}

		public void StartWatching()
		{
			var downloadsPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				"Downloads"
			);

			_watcher = new FileSystemWatcher(downloadsPath)
			{
				NotifyFilter = NotifyFilters.FileName,
				Filter = "*.*",
				EnableRaisingEvents = true
			};

			_watcher.Created += OnFileCreated;
		}

		public void StopWatching()
		{
			if (_watcher != null)
			{
				_watcher.EnableRaisingEvents = false;
				_watcher.Dispose();
				_watcher = null;
			}
		}

		private async void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			// 파일 쓰기 완료 대기 (다운로드 중 감지 방지)
			await Task.Delay(2000);

			if (!File.Exists(e.FullPath)) return;

			OnFileDetected?.Invoke(e.Name ?? e.FullPath);

			try
			{
				var sha256 = ComputeSha256(e.FullPath);
				var fileName = Path.GetFileName(e.FullPath);

                var scanId = await _api.CreateScanAsync("EMP001", fileName, sha256);
                if (scanId == null) return;

                // VT 분석 트리거
                await _api.CompleteScanAsync(scanId, fileName);

                // 결과 조회 (8초 대기)
                await Task.Delay(8000);

                var scanResult = await _api.GetScanResultAsync(scanId);
                var staticResult = scanResult?.scan?.static_result ?? "unknown";

                // 로그 저장
                App.Logs.SaveLog(new QSightClient.Models.ScanLog
                {
                    FilePath = e.FullPath,
                    FileName = fileName,
                    ScanId = scanId,
                    StaticResult = staticResult,
                    Timestamp = DateTime.Now,
                    ScanTime = DateTime.Now,
                    Result = staticResult
                });

                OnScanComplete?.Invoke(fileName, staticResult);
            }
			catch
			{
				// 파일 접근 실패 등 무시
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