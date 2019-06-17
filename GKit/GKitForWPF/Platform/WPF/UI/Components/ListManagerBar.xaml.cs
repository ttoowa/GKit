using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GKit;

namespace GKit.WPF.Components {
	/// <summary>
	/// ListManagerBar.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ListManagerBar : UserControl {

		public ActionEvent OnClick_CreateItemButton;
		public ActionEvent OnClick_CreateFolderButton;
		public ActionEvent OnClick_CopyButton;
		public ActionEvent OnClick_RemoveButton;

		public ListManagerBar() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			Init();
			RegisterEvents();
		}
		private void Init() {
			OnClick_CreateItemButton = new ActionEvent();
			OnClick_CreateFolderButton = new ActionEvent();
			OnClick_CopyButton = new ActionEvent();
			OnClick_RemoveButton = new ActionEvent();
		}
		private void RegisterEvents() {
			Grid[] buttons = new Grid[] {
				CreateItemButton,
				CreateFolderButton,
				CopyButton,
				RemoveButton,
			};

			for (int i = 0; i < buttons.Length; ++i) {
				Grid button = buttons[i];
				button.SetButtonReaction(button.Children[button.Children.Count-1] as Border);
			}

			CreateItemButton.SetOnClick(OnClick_CreateItemButton.Invoke);
			CreateFolderButton.SetOnClick(OnClick_CreateFolderButton.Invoke);
			CopyButton.SetOnClick(OnClick_CopyButton.Invoke);
			RemoveButton.SetOnClick(OnClick_RemoveButton.Invoke);
		}
	}
}
