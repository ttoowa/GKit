using GKitForWPF.Graphics;
using GKitForWPF.UI.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GKitForWPF.UI.Controls; 

public delegate void TextEditedDelegate(string oldText, string newText, ref bool cancelEdit);

public partial class EditTextBlock : UserControl {
    public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.RegisterAttached(nameof(IsEditing), typeof(bool), typeof(EditTextBlock), new PropertyMetadata(false));

    public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(nameof(Text), typeof(string), typeof(EditTextBlock), new PropertyMetadata(null));

    public static readonly DependencyProperty EditingBackgroundProperty =
        DependencyProperty.RegisterAttached(nameof(EditingBackground), typeof(Brush), typeof(EditTextBlock), new PropertyMetadata("FFFFFF".ToBrush()));

    public static readonly DependencyProperty EditingForegroundProperty =
        DependencyProperty.RegisterAttached(nameof(EditingForeground), typeof(Brush), typeof(EditTextBlock), new PropertyMetadata("666666".ToBrush()));

    public event TextEditedDelegate TextEdited;

    public bool IsEditing {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Brush EditingBackground {
        get => (Brush)GetValue(EditingBackgroundProperty);
        set => SetValue(EditingBackgroundProperty, value);
    }

    public Brush EditingForeground {
        get => (Brush)GetValue(EditingBackgroundProperty);
        set => SetValue(EditingBackgroundProperty, value);
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

        EditingTextBox.SetBinding(FontSizeProperty, new Binding(nameof(FontSize)) { Source = this, Mode = BindingMode.OneWay });
        EditingTextBox.SetBinding(FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this, Mode = BindingMode.OneWay });
        EditingTextBox.SetBinding(BackgroundProperty, new Binding(nameof(EditingBackground)) { Source = this, Mode = BindingMode.OneWay });
        EditingTextBox.SetBinding(ForegroundProperty, new Binding(nameof(EditingForeground)) { Source = this, Mode = BindingMode.OneWay });
        EditingTextBox.SetBinding(VisibilityProperty, new Binding(nameof(IsEditing)) { Source = this, Mode = BindingMode.OneWay, Converter = new BoolToVisibilityConverter() });

        EventArea.SetBinding(IsHitTestVisibleProperty, new Binding(nameof(IsEditing)) { Source = this, Mode = BindingMode.OneWay, Converter = new BoolInvertConverter() });
    }

    public void StartEditing() {
        IsEditing = true;
        StartCaptureMouse();


        EditingTextBox.Text = Text;
        EditingTextBox.Focus();
        EditingTextBox.CaretIndex = EditingTextBox.Text.Length;
    }

    public void EndEditing() {
        IsEditing = false;
        StopCaptureMouse();

        if (Text == EditingTextBox.Text)
            return;

        bool cancelEdit = false;
        TextEdited?.Invoke(Text, EditingTextBox.Text, ref cancelEdit);

        if (cancelEdit)
            return;

        Text = EditingTextBox.Text;
    }

    private void StartCaptureMouse() { InputManager.Current.PreProcessInput += InputManager_PreProcessInput; }
    private void StopCaptureMouse() { InputManager.Current.PreProcessInput -= InputManager_PreProcessInput; }

    private void EventArea_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        if (e.ChangedButton == MouseButton.Left) {
            StartEditing();
            e.Handled = true;
        }
    }

    private void InputManager_PreProcessInput(object sender, PreProcessInputEventArgs e) {
        if (e.StagingItem.Input is MouseButtonEventArgs) {
            MouseButtonEventArgs clickEventArgs = (MouseButtonEventArgs)e.StagingItem.Input;

            if (clickEventArgs.ChangedButton == MouseButton.Left && clickEventArgs.LeftButton == MouseButtonState.Pressed)
                if (IsEditing && !EditingTextBox.IsMouseOver)
                    EndEditing();
        } else if (e.StagingItem.Input is KeyEventArgs) {
            KeyEventArgs keyEventArgs = (KeyEventArgs)e.StagingItem.Input;

            if (keyEventArgs.IsDown && keyEventArgs.Key == Key.Return)
                if (IsEditing)
                    EndEditing();
        }
    }
}