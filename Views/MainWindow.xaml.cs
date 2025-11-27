using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GTAHandler.Models;
using GTAHandler.ViewModels;

namespace GTAHandler.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StateChanged += MainWindow_StateChanged;
    }

    #region Custom Title Bar

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // Double-click to toggle maximize
            ToggleMaximize();
        }
        else
        {
            // Single click to drag
            if (WindowState == WindowState.Maximized)
            {
                // If maximized, restore first before dragging
                var mousePos = e.GetPosition(this);
                var screenPos = PointToScreen(mousePos);

                WindowState = WindowState.Normal;

                // Position window so mouse is at the same relative position
                Left = screenPos.X - (ActualWidth / 2);
                Top = screenPos.Y - 18; // Half of title bar height
            }
            DragMove();
        }
    }

    private void TitleBar_MouseMove(object sender, MouseEventArgs e)
    {
        // This is intentionally left empty - used for potential future drag operations
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void MainWindow_StateChanged(object? sender, System.EventArgs e)
    {
        // Update maximize button tooltip based on state
        if (MaximizeButton != null)
        {
            MaximizeButton.ToolTip = WindowState == WindowState.Maximized ? "Restore" : "Maximize";
        }
    }

    #endregion

    private void Tab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is VehicleHandling vehicle)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ActiveTab = vehicle;
            }
        }
    }

    private void CloseTab_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is VehicleHandling vehicle)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.CloseTabCommand.Execute(vehicle);
            }
        }
        e.Handled = true;
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }
    }
}


