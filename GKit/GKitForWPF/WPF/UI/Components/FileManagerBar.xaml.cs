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
	/// FileManager.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class FileManagerBar : UserControl {
		public ActionEvent OnClick_CreateFileButton;
		public ActionEvent OnClick_OpenFileButton;
		public ActionEvent OnClick_SaveFileButton;

		public FileManagerBar() {
			InitializeComponent();

			if(this.IsDesignMode())
				return;

			Init();
			RegisterEvents();
		}
		private void Init() {
			OnClick_CreateFileButton = new ActionEvent();
			OnClick_OpenFileButton = new ActionEvent();
			OnClick_SaveFileButton = new ActionEvent();
		}
		private void RegisterEvents() {
			Grid[] buttons = new Grid[] {
				CreateFileButton,
				OpenFileButton,
				SaveFileButton,
			};

			for(int i=0; i<buttons.Length; ++i) {
				buttons[i].SetButtonReaction();
			}

			CreateFileButton.SetOnClick(OnClick_CreateFileButton.Invoke);
			OpenFileButton.SetOnClick(OnClick_OpenFileButton.Invoke);
			SaveFileButton.SetOnClick(OnClick_SaveFileButton.Invoke);
		}

	}
}
