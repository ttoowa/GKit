using System;
using System.Windows;

namespace GKitForWPF.Resources {
	public static class StyleResource {
		internal const string ThemePath = "pack://application:,,,/GKitForWPF;component/WPF/Resources/Themes/";
		public const string FlatTheme = ThemePath + "FlatTheme.xaml";

		public static void Apply(ResourceDictionary appResource, ThemeType themeType) {
			string themeUri;
			switch (themeType) {
				default:
				case ThemeType.FlatTheme:
					themeUri = FlatTheme;
					break;
			}

			ApplyCustom(appResource, themeUri);
		}
		public static void ApplyCustom(ResourceDictionary appResource, string assemblyName, string relativeXamlPath) {
			ApplyCustom(appResource, GResourceUtility.GetUri(assemblyName, relativeXamlPath));
		}
		private static void ApplyCustom(ResourceDictionary appResource, string stylePath) {
			ApplyCustom(appResource, new Uri(stylePath));
		}
		private static void ApplyCustom(ResourceDictionary appResource, Uri styleUri) {
			ResourceDictionary resourceDict = new ResourceDictionary();
			resourceDict.Source = styleUri;
			appResource.MergedDictionaries.Add(resourceDict);
		}
	}
}
