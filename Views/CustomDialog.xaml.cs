using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome.Sharp;

namespace GTAHandler.Views;

public enum DialogType
{
    Information,
    Warning,
    Error,
    Question,
    Success
}

public enum DialogButtons
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
}

public enum DialogResult
{
    None,
    OK,
    Cancel,
    Yes,
    No
}

public partial class CustomDialog : Window
{
    public new DialogResult DialogResult { get; private set; } = DialogResult.None;

    public new string Title { get; set; } = "Dialog";
    public string Message { get; set; } = string.Empty;

    public CustomDialog()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void SetIcon(DialogType type)
    {
        var (icon, color) = type switch
        {
            DialogType.Information => (IconChar.InfoCircle, "#0091cd"),  // Info blue
            DialogType.Warning => (IconChar.ExclamationTriangle, "#ecb731"),      // Warning yellow
            DialogType.Error => (IconChar.TimesCircle, "#ed1b2e"),        // Error red
            DialogType.Question => (IconChar.QuestionCircle, "#0091cd"),     // Info blue
            DialogType.Success => (IconChar.CheckCircle, "#537b35"),      // Success green
            _ => (IconChar.InfoCircle, "#0091cd")
        };

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        DialogIcon.Icon = icon;
        DialogIcon.Foreground = brush;

        // Set icon background
        IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)) { Opacity = 0.15 };
    }

    private void AddButtons(DialogButtons buttons)
    {
        ButtonPanel.Children.Clear();

        switch (buttons)
        {
            case DialogButtons.OK:
                AddButton("OK", DialogResult.OK, true);
                break;
            case DialogButtons.OKCancel:
                AddButton("Cancel", DialogResult.Cancel, false);
                AddButton("OK", DialogResult.OK, true);
                break;
            case DialogButtons.YesNo:
                AddButton("No", DialogResult.No, false);
                AddButton("Yes", DialogResult.Yes, true);
                break;
            case DialogButtons.YesNoCancel:
                AddButton("Cancel", DialogResult.Cancel, false);
                AddButton("No", DialogResult.No, false);
                AddButton("Yes", DialogResult.Yes, true);
                break;
        }
    }

    private void AddButton(string content, DialogResult result, bool isPrimary)
    {
        var button = new Button
        {
            Content = content,
            MinWidth = 80,
            Margin = new Thickness(8, 0, 0, 0),
            Padding = new Thickness(16, 8, 16, 8)
        };

        if (!isPrimary)
        {
            button.Background = new SolidColorBrush(Color.FromRgb(0x2a, 0x2a, 0x4e));
        }

        button.Click += (s, e) =>
        {
            DialogResult = result;
            Close();
        };

        ButtonPanel.Children.Add(button);
    }

    public static DialogResult Show(string message, string title = "Message",
        DialogType type = DialogType.Information, DialogButtons buttons = DialogButtons.OK,
        Window? owner = null)
    {
        var dialog = new CustomDialog
        {
            Title = title,
            Message = message,
            Owner = owner ?? Application.Current.MainWindow
        };

        dialog.SetIcon(type);
        dialog.AddButtons(buttons);
        dialog.ShowDialog();

        return dialog.DialogResult;
    }

    public static DialogResult ShowQuestion(string message, string title = "Confirm", Window? owner = null)
    {
        return Show(message, title, DialogType.Question, DialogButtons.YesNo, owner);
    }

    public static DialogResult ShowWarning(string message, string title = "Warning", Window? owner = null)
    {
        return Show(message, title, DialogType.Warning, DialogButtons.OK, owner);
    }

    public static DialogResult ShowError(string message, string title = "Error", Window? owner = null)
    {
        return Show(message, title, DialogType.Error, DialogButtons.OK, owner);
    }

    public static DialogResult ShowInfo(string message, string title = "Information", Window? owner = null)
    {
        return Show(message, title, DialogType.Information, DialogButtons.OK, owner);
    }

    public static DialogResult ShowSuccess(string message, string title = "Success", Window? owner = null)
    {
        return Show(message, title, DialogType.Success, DialogButtons.OK, owner);
    }
}

