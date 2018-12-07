using System;
using System.Windows.Markup;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;

namespace FMSC.Core.Windows.ComponentModel
{
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;
        public string Exclusions { get; set; }

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Exclusions == null)
                return Enum.GetValues(_type);

            IEnumerable<string> exclusions = Exclusions.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.ToLower()).ToArray();

            return Enum.GetValues(_type).Cast<object>().Where(e => !exclusions.Contains(e.ToString().ToLower()));
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetDescription(Enum @enum)
        {
            object[] attrs = @enum.GetType().GetField(@enum.ToString()).GetCustomAttributes(false);

            if (attrs.Length > 0)
                return (attrs[0] as DescriptionAttribute).Description;
            else
                return @enum.ToString();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetDescription((Enum)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
