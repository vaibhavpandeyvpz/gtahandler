using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GTAHandler.Models;

namespace GTAHandler.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;
        bool invert = parameter as string == "Invert";

        if (invert)
            boolValue = !boolValue;

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool visibility = value is Visibility v && v == Visibility.Visible;
        bool invert = parameter as string == "Invert";

        return invert ? !visibility : visibility;
    }
}

public class GameTypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            GameType.GTA3 => new SolidColorBrush(Color.FromRgb(66, 133, 244)),
            GameType.GTAVC => new SolidColorBrush(Color.FromRgb(255, 111, 145)),
            GameType.GTASA => new SolidColorBrush(Color.FromRgb(255, 171, 64)),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class GameTypeToDisplayNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            GameType.GTA3 => "GTA:III",
            GameType.GTAVC => "GTA:VC",
            GameType.GTASA => "GTA:SA",
            _ => value?.ToString() ?? ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ModifiedToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isModified = value is bool b && b;
        return isModified
            ? new SolidColorBrush(Color.FromRgb(255, 193, 7))
            : new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool invert = parameter as string == "Invert";
        bool isNull = value == null;

        if (invert)
            isNull = !isNull;

        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class GameTypeVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not GameType gameType || parameter is not string gameParam)
            return Visibility.Visible;

        return gameParam switch
        {
            "GTA3" => gameType == GameType.GTA3 ? Visibility.Visible : Visibility.Collapsed,
            "GTAVC" => gameType == GameType.GTAVC ? Visibility.Visible : Visibility.Collapsed,
            "GTASA" => gameType == GameType.GTASA ? Visibility.Visible : Visibility.Collapsed,
            "NotGTA3" => gameType != GameType.GTA3 ? Visibility.Visible : Visibility.Collapsed,
            "NotGTAVC" => gameType != GameType.GTAVC ? Visibility.Visible : Visibility.Collapsed,
            "NotGTASA" => gameType != GameType.GTASA ? Visibility.Visible : Visibility.Collapsed,
            "SAOnly" => gameType == GameType.GTASA ? Visibility.Visible : Visibility.Collapsed,
            "VCAndSA" => gameType != GameType.GTA3 ? Visibility.Visible : Visibility.Collapsed,
            _ => Visibility.Visible
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HasUnsavedChangesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool hasChanges = value is bool b && b;
        return hasChanges ? " *" : "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EqualityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return false;

        return Equals(values[0], values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

