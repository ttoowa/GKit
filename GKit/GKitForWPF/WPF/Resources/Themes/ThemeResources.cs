using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GKit.WPF.Resources {
	public static class StyleResource {
		internal const string ThemePath = "pack://application:,,,/GKitForWPF;component/WPF/Resources/Themes/";
		public const string FlatTheme = ThemePath + "FlatTheme.xaml";

		public static void Apply(ResourceDictionary appResource, ThemeType themeType) {
			string themeUri;
			switch(themeType) {
				default:
				case ThemeType.FlatTheme:
					themeUri = FlatTheme;
					break;
			}

			ApplyCustom(appResource, themeUri);
		}
		public static void ApplyCustom(ResourceDictionary appResource, string assemblyName, string relativeXamlPath) {
			string stylePath = $"pack://application:,,,/{assemblyName};component/{relativeXamlPath}";

			ApplyCustom(appResource, stylePath);
		}
		private static void ApplyCustom(ResourceDictionary appResource, string stylePath) {
			ResourceDictionary resourceDict = new ResourceDictionary();
			resourceDict.Source = new Uri(stylePath);
			appResource.MergedDictionaries.Add(resourceDict);
		}
	}
}
