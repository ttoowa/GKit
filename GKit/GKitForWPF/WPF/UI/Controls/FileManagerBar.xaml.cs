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
using GKit.WPF.UI.Converters;

namespace GKit.WPF.UI.Controls {
	/// <summary>
	/// FileManager.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class FileManagerBar : UserControl {
		public static readonly DependencyProperty CreateFileButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(CreateFileButtonVisible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(true));
		public static readonly DependencyProperty OpenFileButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(OpenFileButtonVisible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(true));
		public static readonly DependencyProperty SaveFileButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(SaveFileButtonVisible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(true));
		
		public static readonly DependencyProperty Separator1VisibleProperty = DependencyProperty.RegisterAttached(nameof(Separator1Visible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(false));

		public static readonly DependencyProperty ImportButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(ImportButtonVisible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(false));
		public static readonly DependencyProperty ExportButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(ExportButtonVisible), typeof(bool), typeof(FileManagerBar), new PropertyMetadata(false));

		public bool CreateFileButtonVisible {
			get {
				return (bool)GetValue(CreateFileButtonVisibleProperty);
			}
			set {
				SetValue(CreateFileButtonVisibleProperty, value);
			}
		}
		public bool OpenFileButtonVisible {
			get {
				return (bool)GetValue(OpenFileButtonVisibleProperty);
			}
			set {
				SetValue(OpenFileButtonVisibleProperty, value);
			}
		}
		public bool SaveFileButtonVisible {
			get {
				return (bool)GetValue(SaveFileButtonVisibleProperty);
			}
			set {
				SetValue(SaveFileButtonVisibleProperty, value);
			}
		}

		public bool Separator1Visible {
			get {
				return (bool)GetValue(Separator1VisibleProperty);
			}
			set {
				SetValue(Separator1VisibleProperty, value);
			}
		}

		public bool ImportButtonVisible {
			get {
				return (bool)GetValue(ImportButtonVisibleProperty);
			}
			set {
				SetValue(ImportButtonVisibleProperty, value);
			}
		}
		public bool ExportButtonVisible {
			get {
				return (bool)GetValue(ExportButtonVisibleProperty);
			}
			set {
				SetValue(ExportButtonVisibleProperty, value);
			}
		}

		public ActionEvent CreateFileButtonClick = new ActionEvent();
		public ActionEvent OpenFileButtonClick = new ActionEvent();
		public ActionEvent SaveFileButtonClick = new ActionEvent();
		public ActionEvent ImportButtonClick = new ActionEvent();
		public ActionEvent ExportButtonClick = new ActionEvent();

		public FileManagerBar() {
			InitializeComponent();

			if(this.IsDesignMode())
				return;

			InitBindings();
			RegisterEvents();
		}
		private void InitBindings() {
			BoolToVisibilityConverter boolToVisibilityConverter = new BoolToVisibilityConverter();

			CreateFileButton.SetBinding(VisibilityProperty, new Binding(nameof(CreateFileButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			OpenFileButton.SetBinding(VisibilityProperty, new Binding(nameof(OpenFileButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			SaveFileButton.SetBinding(VisibilityProperty, new Binding(nameof(SaveFileButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			Separator1.SetBinding(VisibilityProperty, new Binding(nameof(Separator1Visible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			ImportButton.SetBinding(VisibilityProperty, new Binding(nameof(ImportButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			ExportButton.SetBinding(VisibilityProperty, new Binding(nameof(ExportButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });

		}
		private void RegisterEvents() {
			CreateFileButton.RegisterClickEvent(CreateFileButtonClick);
			OpenFileButton.RegisterClickEvent(OpenFileButtonClick);
			SaveFileButton.RegisterClickEvent(SaveFileButtonClick);
			ImportButton.RegisterClickEvent(ImportButtonClick);
			ExportButton.RegisterClickEvent(ExportButtonClick);
		}

		
	}
}
