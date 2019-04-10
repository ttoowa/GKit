using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigLibrary;

namespace DLLPublisher {
	class Program {
		static void Main(string[] args) {
			Root root = new Root();
		}
	}

	public class Root {
		private string[] FileNames = new string[] {
			"BigLibrary.dll",
			"Newtonsoft.Json.dll",
		};
#if UNITY
		private const string OriginDir = @"X:\WorkDesk\A_Programing\Csharp\2018\20180109_BigLibrary\BigLibrary\BigLibrary\bin\Unity\";
		private string[] dstDirs = new string[] {
			@"X:\WorkDesk\A_Unity\2017\20170909_Prim\PrimEngine\Assets\Script\Library",
			@"X:\WorkDesk\A_Unity\2018\20180728_TEAM0702\TEAM0702\Assets\Script\Library",
			@"X:\WorkDesk\A_Unity\2018\20180817_GemStrike\GemStrike\Assets\Script\Library",
		};
#elif WPF
		private const string OriginDir = @"X:\WorkDesk\A_Programing\Csharp\2018\20180109_BigLibrary\BigLibrary\BigLibrary\bin\WPF\";
		private string[] dstDirs = new string[] {

		};
#endif


		public Root() {
			Init();
		}
		private void Init() {

			for (int i = 0; i < FileNames.Length; ++i) {
				string fileName = FileNames[i];
				string originPath = Path.Combine(OriginDir, fileName);
				for (int i2 = 0; i2 < dstDirs.Length; ++i2) {
					string dstPath = Path.Combine(dstDirs[i2], fileName);
					IOUtility.CopyFile(new FileInfo(originPath), new FileInfo(dstPath));
				}
			}

			Console.WriteLine("파일 이동에 성공했습니다.");
		}
	}
}
