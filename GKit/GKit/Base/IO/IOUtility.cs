using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.IO {
	public static class IOUtility {
		public static FileInfo AppFileInfo {
			get {
				return new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
			}
		}


		//Move and Copy
		/// <summary>
		/// 파일 혹은 폴더를 이동합니다. 이미 있는 이름일 경우 넘버링을 붙입니다.
		/// </summary>
		public static void MoveFile(FileInfo originInfo, DirectoryInfo destInfo) {
			string origin = originInfo.FullName;
			string destDirectory = destInfo.FullName;
			if (File.Exists(origin)) {
				FileInfo fileInfo = new FileInfo(Path.Combine(destDirectory, Path.GetFileName(origin)));
				string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
				string extension = fileInfo.Extension;

				if (File.Exists(fileInfo.FullName)) {
					int num = 2;
					while (File.Exists(Path.Combine(destDirectory, fileName + num + extension))) {
						++num;
					}
					fileInfo = new FileInfo(Path.Combine(destDirectory, fileName + num + extension));
				}

				File.Move(origin, fileInfo.FullName);
			} else if (Directory.Exists(origin)) {
				DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(destDirectory, originInfo.Name));
				if (Directory.Exists(dirInfo.FullName)) {
					int num = 2;
					while (File.Exists(Path.Combine(destDirectory, originInfo.Name + num))) {
						++num;
					}
					dirInfo = new DirectoryInfo(Path.Combine(destDirectory, originInfo.Name + num));
				}

				Directory.Move(origin, dirInfo.FullName);
			}
		}
		public static void MoveAllFiles(DirectoryInfo originInfo, DirectoryInfo destInfo) {
			try {
				destInfo.Create();

				foreach (FileInfo fileInfo in originInfo.GetFiles()) {
					try {
						string newFilePath = Path.Combine(destInfo.FullName, fileInfo.Name);
						if (File.Exists(newFilePath)) {
							File.Delete(newFilePath);
						}
						fileInfo.MoveTo(newFilePath);
					} catch (Exception ex) {
						GDebug.Log(ex.ToString());
					}
				}
				foreach (DirectoryInfo subDirectory in originInfo.GetDirectories()) {
					DirectoryInfo nextDestDir = destInfo.CreateSubdirectory(subDirectory.Name);
					MoveAllFiles(subDirectory, nextDestDir);
				}
			} catch (Exception ex) {
				GDebug.Log(ex.ToString());
			}
		}

		public static void CopyFile(FileInfo originInfo, DirectoryInfo destInfo) {
			string origin = originInfo.FullName;
			string destDirectory = destInfo.FullName;
			if (File.Exists(origin)) {
				FileInfo fileInfo = new FileInfo(Path.Combine(destDirectory, Path.GetFileName(origin)));
				string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
				string extension = fileInfo.Extension;

				if (File.Exists(fileInfo.FullName)) {
					int num = 2;
					while (File.Exists(Path.Combine(destDirectory, fileName + num + extension))) {
						++num;
					}
					fileInfo = new FileInfo(Path.Combine(destDirectory, fileName + num + extension));
				}

				File.Copy(origin, fileInfo.FullName);
			}
		}
		public static void CopyFile(FileInfo originInfo, FileInfo destInfo, bool overwrite = true) {
			destInfo.Directory.Create();
			File.Copy(originInfo.FullName, destInfo.FullName, overwrite);
		}
		public static void CopyAllFiles(DirectoryInfo originInfo, DirectoryInfo destInfo) {
			foreach (DirectoryInfo dir in originInfo.GetDirectories())
				CopyAllFiles(dir, destInfo.CreateSubdirectory(dir.Name));
			foreach (FileInfo file in originInfo.GetFiles())
				file.CopyTo(Path.Combine(destInfo.FullName, file.Name));
		}

		public static ImageFileFormat GetImageFormat(Stream stream) {
			const int BufferSize = 4;

			if (stream.Length < BufferSize)
				return ImageFileFormat.Unknown;

			var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
			var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
			var png = new byte[] { 137, 80, 78, 71 };    // PNG
			var tiff = new byte[] { 73, 73, 42 };         // TIFF
			var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
			var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
			var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

			var buffer = new byte[BufferSize];

			stream.Read(buffer, 0, buffer.Length);

			if (bmp.SequenceEqual(buffer.Take(bmp.Length)))
				return ImageFileFormat.bmp;

			if (gif.SequenceEqual(buffer.Take(gif.Length)))
				return ImageFileFormat.gif;

			if (png.SequenceEqual(buffer.Take(png.Length)))
				return ImageFileFormat.png;

			if (tiff.SequenceEqual(buffer.Take(tiff.Length)))
				return ImageFileFormat.tiff;

			if (tiff2.SequenceEqual(buffer.Take(tiff2.Length)))
				return ImageFileFormat.tiff;

			if (jpeg.SequenceEqual(buffer.Take(jpeg.Length)))
				return ImageFileFormat.jpg;

			if (jpeg2.SequenceEqual(buffer.Take(jpeg2.Length)))
				return ImageFileFormat.jpg;

			return ImageFileFormat.Unknown;
		}

		public static void SaveText(this string text, string filename) {
			SaveText(text, filename, Encoding.UTF8);
		}
		public static void SaveText(this string text, string filename, Encoding encoding) {
			FileInfo fileInfo = new FileInfo(filename);
			if (!fileInfo.Directory.Exists) {
				fileInfo.Directory.Create();
			}
			using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write)) {
				using (StreamWriter writer = new StreamWriter(fileStream, encoding)) {
					writer.Write(text);
				}
			}
		}
		public static void SaveTextAppend(this string text, string filename) {
			SaveTextAppend(text, filename, Encoding.UTF8);
		}
		public static void SaveTextAppend(this string text, string filename, Encoding encoding) {
			FileInfo fileInfo = new FileInfo(filename);
			if (!fileInfo.Directory.Exists) {
				fileInfo.Directory.Create();
			}
			using (FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)) {
				fileStream.Seek(0, SeekOrigin.End);
				using (StreamWriter writer = new StreamWriter(fileStream, encoding)) {
					writer.WriteLine(text);
				}
			}
		}
		public static string LoadText(string filename) {
			return LoadText(filename, Encoding.UTF8);
		}
		public static string LoadText(string filename, Encoding encoding) {
			using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				using (StreamReader reader = new StreamReader(fileStream, encoding)) {
					return reader.ReadToEnd();
				}
			}
		}

		public static string NormalizePath(string path) {
			return path.Replace('/', '\\')
					   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}
		public static bool ComparePath(string path1, string path2) {
			return NormalizePath(path1) == NormalizePath(path2);
		}

		public static string GetRelativePath(string frontPath, string filename) {
			string onlyFilename = Path.GetFileName(filename);

			frontPath = NormalizePath(frontPath);
			filename = NormalizePath(filename);

			if (frontPath.Last() != '\\') {
				frontPath += "\\";
			}
			if (filename.StartsWith(frontPath)) {
				filename = filename.Substring(frontPath.Length);
			}

			return Path.Combine(Path.GetDirectoryName(filename), onlyFilename);
		}

		public static bool IsDirectory(string path) {
            if (!File.Exists(path)) return false;
            
			FileAttributes attr = File.GetAttributes(path);

			return (attr & FileAttributes.Directory) == FileAttributes.Directory;
		}

		public static bool IsEmptyDirectory(string dir) {
			IEnumerable<string> items = Directory.EnumerateFileSystemEntries(dir);
			using (IEnumerator<string> entry = items.GetEnumerator()) {
				return !entry.MoveNext();
			}
		}

		public static string GetMetadataHash(string filename) {
			FileInfo fileInfo = new FileInfo(filename);

			long createdTimeLong = DateTimeToLong(fileInfo.CreationTimeUtc);
			long writedTimeLong = DateTimeToLong(fileInfo.LastWriteTimeUtc);

			return Security.Encrypt.SimplexHash.ComputeMD5((createdTimeLong ^ writedTimeLong ^ fileInfo.Length).ToString());
		}

		private static long DateTimeToLong(DateTime time) {
			byte[] writeTimeBytes = BitConverter.GetBytes(time.ToOADate());
			return BitConverter.ToInt64(writeTimeBytes, 0);
		}
	}
}