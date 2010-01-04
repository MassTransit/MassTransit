namespace MassTransit.SystemView.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public class TreeItemToImagePathConverter :
        IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (new Random().Next(0, 10) == 0)
                return new BitmapImage(new Uri("http://www.gstatic.com/codesite/ph/images/defaultlogo.png"));
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}