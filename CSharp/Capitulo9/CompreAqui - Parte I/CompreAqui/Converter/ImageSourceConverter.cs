using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;


namespace CompreAqui.Converter
{
    public class ImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string valor = value as string;
            return new BitmapImage(new Uri(string.Concat(@"ms-appx:/Icones/", valor), UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
