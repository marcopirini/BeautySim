using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Manager3D
{



    public class ExitingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? exitingTime = (DateTime?)value;
            bool isNull = value == null;

            if (isNull || exitingTime == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                string toRet= exitingTime?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                return toRet;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool toBeClosed && targetType == typeof(Brush))
            {
                return toBeClosed ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightBlue);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
