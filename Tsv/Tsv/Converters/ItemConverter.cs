using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tsv.ViewModel;

namespace Tsv.Converters
{
    class ItemConverter : IValueConverter
    {
        private static string Blank = "  ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var vm = value as TsvDataViewModel;

            if (vm == null)
            {
                return null;
            }
            var v = vm.Item;
            v = v.TrimStart(new[] { ' ', '-', '+' }).TrimEnd(' ');
            v = PadSpace(v, vm.Level);
            return v;
        }

        private string PadSpace(string v, int level)
        {
            var result = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                result.Append(Blank);
            }
            return result.Append(v).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
