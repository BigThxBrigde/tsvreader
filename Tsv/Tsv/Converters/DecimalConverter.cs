using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tsv.Service;

namespace Tsv.Converters
{
    class DecimalConverter : IValueConverter
    {
        private static int roundDecimal = Ioc.Default.GetInstance<Config>().Settings.RoundDecimal;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return null; }
            var v = (decimal?)value;
            if (v == null || !v.HasValue)
            {
                return null;
            }

            return Decimal.Round(v.Value, roundDecimal).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
