using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CompreAqui.Converter
{
    public class VisibilityConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double valor = System.Convert.ToDouble(value);

            Visibility visibilidade;

            if (valor != 0)
                visibilidade = Visibility.Visible;
            else
                visibilidade = Visibility.Collapsed;

            return visibilidade;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
