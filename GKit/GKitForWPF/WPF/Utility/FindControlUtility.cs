using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GKit.WPF {
	public class FindByTagAttribute : Attribute {
	}
	public static class FindControlUtility {

		/// <summary>
		/// UserControl에 FindByTag Attribute가 붙은 멤버를 정의하고, FindControlsByTag 함수를 호출하면 Tag를 사용해 컨트롤을 찾아 초기화해 줍니다.
		/// </summary>
		public static void FindControlsByTag(this FrameworkElement control) {
			List<FrameworkElement> logicalElements = new List<FrameworkElement>();
			GetLogicalElements(control, logicalElements);

			FieldInfo[] fields = control.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(fieldInfo => fieldInfo.GetCustomAttribute(typeof(FindByTagAttribute)) != null).ToArray();
			Dictionary<string, FieldInfo> fieldDict = new Dictionary<string, FieldInfo>();
			foreach (FieldInfo field in fields) {
				fieldDict.Add(field.Name, field);
			}

			foreach (FrameworkElement element in logicalElements) {
				if (element.Tag != null) {
					string elementName = element.Tag.ToString();
					if (fieldDict.ContainsKey(elementName)) {
						FieldInfo field = fieldDict[elementName];

						field.SetValue(control, element);
					}
				}
			}
		}

		public static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject {
			if (dependencyObject == null) {
				return null;
			}

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++) {
				DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);

				T result = (child as T) ?? GetFirstChildOfType<T>(child);

				if (result != null) {
					return result;
				}
			}

			return null;
		}

		public static FrameworkElement[] GetLogicalElements(object control) {
			List<FrameworkElement> elementList = new List<FrameworkElement>();
			GetLogicalElements(control, elementList);

			return elementList.ToArray();
		}
		private static void GetLogicalElements(object parent, List<FrameworkElement> logicalElements) {
			if (parent == null) return;
			if (parent.GetType().IsSubclassOf(typeof(FrameworkElement)))
				logicalElements.Add((FrameworkElement)parent);
			DependencyObject doParent = parent as DependencyObject;
			if (doParent == null) return;
			foreach (object child in LogicalTreeHelper.GetChildren(doParent)) {
				GetLogicalElements(child, logicalElements);
			}
		}
	}
}
