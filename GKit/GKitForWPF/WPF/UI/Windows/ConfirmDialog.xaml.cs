using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GKitForWPF.UI.Windows;

public enum ConfirmDialogButtons {
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
    RetryCancel
}

public enum ConfirmDialogResult {
    None,
    Ok,
    Yes,
    No,
    Cancel,
    Retry
}

public enum ConfirmDialogButtonAppearance {
    Secondary,
    Primary,
    Destructive
}

public sealed class ConfirmDialogButton {
    public string Id { get; }
    public string Text { get; }
    public ConfirmDialogButtonAppearance Appearance { get; }
    public bool IsDefault { get; internal set; }
    public bool IsCancel { get; }

    public ConfirmDialogButton(string id, string text,
        ConfirmDialogButtonAppearance appearance = ConfirmDialogButtonAppearance.Secondary,
        bool isDefault = false, bool isCancel = false) {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("A button id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Button text is required.", nameof(text));

        Id = id;
        Text = text;
        Appearance = appearance;
        IsDefault = isDefault;
        IsCancel = isCancel;
    }
}

public partial class ConfirmDialog : Window {
    private readonly IReadOnlyDictionary<string, ConfirmDialogResult> resultByButtonId;
    private ConfirmDialogButton selectedButton;

    public string Message { get; }
    public ReadOnlyCollection<ConfirmDialogButton> Buttons { get; }

    private ConfirmDialog(string message, string title, IEnumerable<ConfirmDialogButton> buttons,
        IReadOnlyDictionary<string, ConfirmDialogResult> resultByButtonId = null) {
        List<ConfirmDialogButton> buttonList = buttons?.Where(button => button != null).ToList()
            ?? throw new ArgumentNullException(nameof(buttons));
        if (buttonList.Count == 0)
            throw new ArgumentException("At least one button is required.", nameof(buttons));
        if (buttonList.GroupBy(button => button.Id, StringComparer.Ordinal).Any(group => group.Count() > 1))
            throw new ArgumentException("Button ids must be unique.", nameof(buttons));
        if (!buttonList.Any(button => button.IsDefault))
            buttonList[0].IsDefault = true;

        Message = message ?? string.Empty;
        Title = title ?? string.Empty;
        Buttons = buttonList.AsReadOnly();
        this.resultByButtonId = resultByButtonId;

        InitializeComponent();
        DataContext = this;
        Closing += OnClosing;
    }

    public static ConfirmDialogResult Show(Window owner, string message, string title,
        ConfirmDialogButtons buttons) {
        (IReadOnlyList<ConfirmDialogButton> choices, IReadOnlyDictionary<string, ConfirmDialogResult> results) =
            CreatePreset(buttons);
        ConfirmDialog dialog = new(message, title, choices, results);
        dialog.ShowModal(owner);
        return dialog.GetPresetResult();
    }

    public static ConfirmDialogResult Show(string message, string title,
        ConfirmDialogButtons buttons = ConfirmDialogButtons.Ok) {
        return Show(null, message, title, buttons);
    }

    public static ConfirmDialogButton Show(Window owner, string message, string title,
        IEnumerable<ConfirmDialogButton> buttons) {
        ConfirmDialog dialog = new(message, title, buttons);
        dialog.ShowModal(owner);
        return dialog.selectedButton;
    }

    public static ConfirmDialogButton Show(string message, string title,
        IEnumerable<ConfirmDialogButton> buttons) {
        return Show(null, message, title, buttons);
    }

    public static ConfirmDialogButton ShowCustom(Window owner, string message, string title,
        params ConfirmDialogButton[] buttons) {
        return Show(owner, message, title, (IEnumerable<ConfirmDialogButton>)buttons);
    }

    public static ConfirmDialogButton ShowCustom(string message, string title,
        params ConfirmDialogButton[] buttons) {
        return Show(null, message, title, (IEnumerable<ConfirmDialogButton>)buttons);
    }

    private void ShowModal(Window requestedOwner) {
        Window resolvedOwner = ResolveOwner(requestedOwner);
        if (resolvedOwner != null && resolvedOwner != this && resolvedOwner.IsVisible)
            Owner = resolvedOwner;
        else
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

        ShowDialog();
    }

    private static Window ResolveOwner(Window requestedOwner) {
        if (requestedOwner != null)
            return requestedOwner;

        Application application = Application.Current;
        return application?.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive)
            ?? application?.MainWindow;
    }

    private ConfirmDialogResult GetPresetResult() {
        if (selectedButton == null || resultByButtonId == null)
            return ConfirmDialogResult.None;
        return resultByButtonId.TryGetValue(selectedButton.Id, out ConfirmDialogResult result)
            ? result
            : ConfirmDialogResult.None;
    }

    private static (IReadOnlyList<ConfirmDialogButton>, IReadOnlyDictionary<string, ConfirmDialogResult>)
        CreatePreset(ConfirmDialogButtons buttons) {
        List<(ConfirmDialogButton Button, ConfirmDialogResult Result)> definitions = buttons switch {
            ConfirmDialogButtons.Ok => new() {
                (new ConfirmDialogButton("ok", "확인", ConfirmDialogButtonAppearance.Primary, true, true), ConfirmDialogResult.Ok)
            },
            ConfirmDialogButtons.OkCancel => new() {
                (new ConfirmDialogButton("ok", "확인", ConfirmDialogButtonAppearance.Primary, true), ConfirmDialogResult.Ok),
                (new ConfirmDialogButton("cancel", "취소", isCancel: true), ConfirmDialogResult.Cancel)
            },
            ConfirmDialogButtons.YesNo => new() {
                (new ConfirmDialogButton("yes", "예", ConfirmDialogButtonAppearance.Primary, true), ConfirmDialogResult.Yes),
                (new ConfirmDialogButton("no", "아니요", isCancel: true), ConfirmDialogResult.No)
            },
            ConfirmDialogButtons.YesNoCancel => new() {
                (new ConfirmDialogButton("yes", "예", ConfirmDialogButtonAppearance.Primary, true), ConfirmDialogResult.Yes),
                (new ConfirmDialogButton("no", "아니요"), ConfirmDialogResult.No),
                (new ConfirmDialogButton("cancel", "취소", isCancel: true), ConfirmDialogResult.Cancel)
            },
            ConfirmDialogButtons.RetryCancel => new() {
                (new ConfirmDialogButton("retry", "다시 시도", ConfirmDialogButtonAppearance.Primary, true), ConfirmDialogResult.Retry),
                (new ConfirmDialogButton("cancel", "취소", isCancel: true), ConfirmDialogResult.Cancel)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null)
        };

        return (definitions.Select(definition => definition.Button).ToList(),
            definitions.ToDictionary(definition => definition.Button.Id, definition => definition.Result,
                StringComparer.Ordinal));
    }

    private void OnChoiceButtonClick(object sender, RoutedEventArgs e) {
        if (sender is not Button { DataContext: ConfirmDialogButton button })
            return;

        selectedButton = button;
        Close();
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();

    private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key != Key.Escape)
            return;

        selectedButton = Buttons.FirstOrDefault(button => button.IsCancel);
        e.Handled = true;
        Close();
    }

    private void OnClosing(object sender, CancelEventArgs e) {
        selectedButton ??= Buttons.FirstOrDefault(button => button.IsCancel);
    }

    private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }
}
