using System;
using System.Threading;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
#else
using System.Windows.Media.Imaging;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class GResourceUtility {
#if OnUnity
		public static Type Get<Type>(string path) where Type : UnityEngine.Object {
			return Resources.Load<Type>(path);
		}
		public static async Task<Type> GetAsync<Type>(string path) where Type : UnityEngine.Object {
			ManualResetEvent waitSwitch = new ManualResetEvent(false);

			//로드
			var request = Resources.LoadAsync<Type>(path);
			request.completed += (AsyncOperation obj) => {
				waitSwitch.Set();
			};

			//대기
			Task waitTask = Task.Factory.StartNew(() => {
				waitSwitch.WaitOne();
			});
			await waitTask;

			return (Type)request.asset;
		}
#elif OnWPF
		public static Uri GetUri(string path) {
			return new Uri("pack://application:,,,/" + path, UriKind.Absolute);
		}
		public static Uri GetUri(string assemblyName, string path) {
			return new Uri($"pack://application:,,,/{assemblyName};component/{path}", UriKind.Absolute);
		}
		public static class Image {
			public static BitmapImage FromUri(string relativePath) {
				return new BitmapImage(GetUri("Resources/Image/" + relativePath));
			}
		}
#endif
	}
}