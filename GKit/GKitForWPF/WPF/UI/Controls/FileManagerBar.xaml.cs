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

namespace GKit.WPF.UI.Controls {
	/// <summary>
	/// FileManager.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class FileManagerBar : UserControl {
		public ActionEvent CreateFileButton_OnClick;
		public ActionEvent OpenFileButton_OnClick;
		public ActionEvent SaveFileButton_OnClick;

		public FileManagerBar() {
			InitializeComponent();

			if(this.IsDesignMode())
				return;

			Init();
			RegisterEvents();
		}
		private void Init() {
			CreateFileButton_OnClick = new ActionEvent();
			OpenFileButton_OnClick = new ActionEvent();
			SaveFileButton_OnClick = new ActionEvent();
		}
		private void RegisterEvents() {
			CreateFileButton.Click += (object sender, RoutedEventArgs e) => { CreateFileButton_OnClick?.Invoke(); };
			OpenFileButton.Click += (object sender, RoutedEventArgs e) => { OpenFileButton_OnClick?.Invoke(); };
			SaveFileButton.Click += (object sender, RoutedEventArgs e) => { SaveFileButton_OnClick?.Invoke(); };
		}

	}
}
