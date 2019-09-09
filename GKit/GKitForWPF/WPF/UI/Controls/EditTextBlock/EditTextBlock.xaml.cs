using GKit;
using GKit.WPF.UI.Converters;
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

namespace GKit.WPF.UI.Controls {
	public delegate void TextEditedDelegate(string oldText, string newText, ref bool cancelEdit);

	public partial class EditTextBlock : UserControl {
		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(nameof(IsEditing), typeof(bool), typeof(EditTextBlock), new PropertyMetadata(false));
		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(nameof(Text), typeof(string), typeof(EditTextBlock), new PropertyMetadata(null));
		public static readonly DependencyProperty EditingTextProperty = DependencyProperty.RegisterAttached(nameof(EditingText), typeof(string), typeof(EditTextBlock), new PropertyMetadata(null));
		public static readonly DependencyProperty EditingBackgroundProperty = DependencyProperty.RegisterAttached(nameof(EditingBackground), typeof(Brush), typeof(EditTextBlock), new PropertyMetadata("FFFFFF".ToBrush()));
		public static readonly DependencyProperty EditingForegroundProperty = DependencyProperty.RegisterAttached(nameof(EditingForeground), typeof(Brush), typeof(EditTextBlock), new PropertyMetadata("666666".ToBrush()));

		public event TextEditedDelegate TextEdited;

		public bool IsEditing {
			get {
				return (bool)GetValue(IsEditingProperty);
			}
			set {
				SetValue(IsEditingProperty, value);
			}
		}

		public string Text {
			get {
					return (string)GetValue(TextProperty);
			} set {
					SetValue(TextProperty, value);
			}
		}
		public string EditingText {
			get {
				return (string)GetValue(EditingTextProperty);
			}
			set {
				SetValue(EditingTextProperty, value);
			}
		}

		public Brush EditingBackground {
			get {
				return (Brush)GetValue(EditingBackgroundProperty);
			}
			set {
				SetValue(EditingBackgroundProperty, value);
			}
		}
		public Brush EditingForeground {
			get {
				return (Brush)GetValue(EditingBackgroundProperty);
			}
			set {
				SetValue(EditingBackgroundProperty, value);
			}
		}

		public EditTextBlock() {
			InitializeComponent();
			InitBindings();
		}
		private void InitBindings() {
			StaticTextBlock.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(FontSize)) { Source = this, Mode = BindingMode.OneWay });
			StaticTextBlock.SetBinding(TextBlock.FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this, Mode = BindingMode.OneWay });
			StaticTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this, Mode = BindingMode.OneWay });
			StaticTextBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this, Mode = BindingMode.OneWay });

			EditingTextBox.SetBinding(TextBox.FontSizeProperty, new Binding(nameof(FontSize)) { Source = this, Mode = BindingMode.OneWay });
			EditingTextBox.SetBinding(TextBox.FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this, Mode = BindingMode.OneWay });
			EditingTextBox.SetBinding(TextBox.TextProperty, new Binding(nameof(EditingText)) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			EditingTextBox.SetBinding(TextBox.BackgroundProperty, new Binding(nameof(EditingBackground)) { Source = this, Mode = BindingMode.OneWay });
			EditingTextBox.SetBinding(TextBox.ForegroundProperty, new Binding(nameof(EditingForeground)) { Source = this, Mode = BindingMode.OneWay });
			EditingTextBox.SetBinding(TextBox.VisibilityProperty, new Binding(nameof(IsEditing)) { Source = this, Mode = BindingMode.OneWay, Converter = new BoolToVisibilityConverter() });

			EventArea.SetBinding(Border.IsHitTestVisibleProperty, new Binding(nameof(IsEditing)) { Source = this, Mode = BindingMode.OneWay, Converter = new BoolInvertConverter() });
		}

		public void StartEditing() {
			IsEditing = true;
			StartCaptureMouse();


			EditingText = Text;
			EditingTextBox.Focus();
			EditingTextBox.CaretIndex = EditingTextBox.Text.Length;
		}
		public void EndEditing() {
			IsEditing = false;
			StopCaptureMouse();

			if (Text == EditingText)
				return;

			bool cancelEdit = false;
			TextEdited?.Invoke(Text, EditingText, ref cancelEdit);

			if (cancelEdit)
				return;

			Text = EditingText;
		}

		private void StartCaptureMouse() {
			InputManager.Current.PreProcessInput += InputManager_PreProcessInput;
		}
		private void StopCaptureMouse() {
			InputManager.Current.PreProcessInput -= InputManager_PreProcessInput;
		}

		private void EventArea_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				StartEditing();
				e.Handled = true;
			}
		}
		private void InputManager_PreProcessInput(object sender, PreProcessInputEventArgs e) {
			if(e.StagingItem.Input is MouseButtonEventArgs) {
				MouseButtonEventArgs clickEventArgs = (MouseButtonEventArgs)e.StagingItem.Input;

				if(clickEventArgs.ChangedButton == MouseButton.Left &&
				clickEventArgs.LeftButton == MouseButtonState.Pressed) {
					if(IsEditing && !EditingTextBox.IsMouseOver) {
						EndEditing();
					}
				}
			} else if(e.StagingItem.Input is KeyEventArgs) {
				KeyEventArgs keyEventArgs = (KeyEventArgs)e.StagingItem.Input;

				if(keyEventArgs.IsDown && keyEventArgs.Key == Key.Return) {
					if (IsEditing) {
						EndEditing();
					}
				}
			}
		}

	}
}
