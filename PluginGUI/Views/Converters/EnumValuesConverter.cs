using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PluginGUI.Views.Converters
{
    public class EnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as Type;
            if (type != null && type.IsEnum)
            {
                return type.GetEnumValues();
            }

            return new List<object>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
