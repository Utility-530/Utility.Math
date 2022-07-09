using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NormalMath.Wpf.Demo.Common
{
    public class RatioConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {


            if (values.All(a => a is double))
            {
                double result =
                    (double.Parse(values[0].ToString()) + double.Parse(values[1].ToString()))
                    / (2 * double.Parse(values[1].ToString()));

                return (int)(result * 100);
            }
            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
