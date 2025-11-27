using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GTAHandler.Views;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
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
        Close();
    }

    private void Website_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("https://vaibhavpandey.com/");
    }

    private void YouTube_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("https://www.youtube.com/channel/UC5uV1PRvtnNj9P8VfqO93Pw");
    }

    private void GitHub_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/vaibhavpandeyvpz/gtahandler");
    }

    private void Email_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("mailto:contact@vaibhavpandey.com");
    }

    private void Support_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/vaibhavpandeyvpz/gtahandler/issues");
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore errors opening URL
        }
    }
}

