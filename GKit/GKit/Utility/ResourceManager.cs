using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
#if UNITY
using UnityEngine;
#endif

namespace GKit {
	/// <summary>
	/// 리소스를 불러오는 클래스입니다.
	/// </summary>
	public static class ResourceManager {
#if UNITY
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
#elif WPF
		public static Uri GetUri(string relativePath) {
			return new Uri("pack://application:,,,/" + relativePath, UriKind.Absolute);
		}
		public static class Image {
			public static BitmapImage FromUri(string relativePath) {
				return new BitmapImage(GetUri("Resources/Image/" + relativePath));
			}
		}
#endif
	}
}