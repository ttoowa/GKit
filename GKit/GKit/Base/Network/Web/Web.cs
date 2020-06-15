using System;
using System.IO;
using System.Net;
#if OnUnity
using GKitForUnity.MultiThread;
using GKitForUnity.Security;
using GKitForUnity.IO;
#elif OnWPF
using GKitForWPF.MultiThread;
using GKitForWPF.Security;
using GKitForWPF.IO;
#else
using GKit.MultiThread;
using GKit.Security;
using GKit.IO;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public static class Web {
		private static IDGenerator clientIDGenerator = new IDGenerator();

		public static void DownloadFileAsync(string url, DirectoryInfo directoryInfo, Action OnComplete = null, Action OnFailed = null, Action OnFinalize = null) {
			int ID;
			lock (clientIDGenerator) {
				ID = clientIDGenerator.GetID();
			}
			new Action(() => {
				try {
					DownloadFile(url, directoryInfo);
					OnComplete.RunInThread(null);
				} catch (Exception ex) {
					GDebug.Log(ex.ToString(), GLogLevel.Warnning);
					OnFailed.RunInThread(null);
				}
				OnFinalize.RunInThread(null);
				lock (clientIDGenerator) {
					clientIDGenerator.ReturnID(ID);
				}
			}).RunInThread("Bgoon.Web" + ID);
		}
		public static void DownloadFile(string url, DirectoryInfo directoryInfo) {
			byte[] data = DownloadData(url);

			//GetPath
			Uri uri = new Uri(url);
			string localPath;
			if (url.Length > 6 && url.Substring(url.Length - 6) == ":large") {
				//Twitter
				localPath = new Uri(url.Substring(0, url.Length - 6)).LocalPath;
			} else {
				localPath = uri.LocalPath;
			}
			string filePath = Path.Combine(directoryInfo.FullName, Path.GetFileName(localPath));
			string fileName = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath);
			//Find Extension
			if (string.IsNullOrEmpty(extension)) {
				try {
					using (MemoryStream stream = new MemoryStream(data)) {
						ImageFileFormat format = IOUtility.GetImageFormat(stream);

						if (format != ImageFileFormat.Unknown) {
							extension = "." + format.ToString();
							filePath += extension;
						} else {
							extension = "";
						}
					}
				} catch (Exception ex) {
					GDebug.Log(ex.ToString(), GLogLevel.Warnning);
					extension = "";
				}
			}

			int num = 2;
			while (File.Exists(filePath)) {
				filePath = Path.Combine(directoryInfo.FullName, fileName + num + extension);
				++num;
			}

			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				fileStream.Write(data, 0, data.Length);
			}
		}
		public static void DownloadFile(string url, FileInfo fileInfo) {
			byte[] data = DownloadData(url);

			//GetPath
			Uri uri = new Uri(url);
			string filePath = fileInfo.FullName;
			string fileName = Path.GetFileNameWithoutExtension(filePath);
			string extension = fileInfo.Extension;

			fileInfo.Directory.Create();
			fileInfo.Delete();
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				fileStream.Write(data, 0, data.Length);
			}
		}

		public static void SaveUnknownFileAsync(byte[] data, DirectoryInfo directoryInfo, Action OnComplete = null, Action OnFailed = null, Action OnFinalize = null) {
			int ID;
			lock (clientIDGenerator) {
				ID = clientIDGenerator.GetID();
			}
			new Action(() => {
				try {
					SaveUnknownFile(data, directoryInfo);
					OnComplete.RunInThread(null);
				} catch (Exception ex) {
					GDebug.Log(ex.ToString(), GLogLevel.Warnning);
					OnFailed.RunInThread(null);
				}
				OnFinalize.RunInThread(null);
				lock (clientIDGenerator) {
					clientIDGenerator.ReturnID(ID);
				}
			}).RunInThread("Bgoon.Web" + ID);
		}
		public static void SaveUnknownFile(byte[] data, DirectoryInfo directoryInfo) {
			string fileName = "Unknown_" + Encrypt.SimplexHash.ComputeMD5(data.Length.ToString());
			string filePath = Path.Combine(directoryInfo.FullName, fileName);
			string extension = "";
			//Find Extension
			try {
				using (MemoryStream stream = new MemoryStream(data)) {
					ImageFileFormat format = IOUtility.GetImageFormat(stream);

					if (format != ImageFileFormat.Unknown) {
						extension = "." + format.ToString();
						filePath += extension;
					}
				}
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
			}

			int num = 2;
			while (File.Exists(filePath)) {
				filePath = Path.Combine(directoryInfo.FullName, fileName + num + extension);
				++num;
			}

			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				fileStream.Write(data, 0, data.Length);
			}
		}

		public static byte[] DownloadData(string url) {
			WebClient webClient = new WebClient();
			webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
			//webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla / 5.0(Windows NT 6.1; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 36.0.1941.0 Safari / 537.36");

			return webClient.DownloadData(url);
		}

		public static class GoogleDrive {
			public static string GetDirectLink(string ID) {
				return "https://drive.google.com/uc?export=download&id=" + ID;
			}
		}
	}
}
