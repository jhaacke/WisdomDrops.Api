using System;
using System.Globalization;
using System.Windows.Data;

namespace Cube.TestClient.Core
{
	[ValueConversion( typeof(string), typeof(int) )]
	public class StringToIntConverter : IValueConverter
	{
		#region Constructors

		public StringToIntConverter()
		{
			DefaultValue = 0;
		}

		#endregion

		#region Properties

		public int DefaultValue { get; set; }

		#endregion

		#region Members

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var convertedValue = string.Empty;
			if( value != null )
			{
				convertedValue = value.ToString();
			}
			return convertedValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var convertedValue = DefaultValue;
			if( value != null )
			{
				if( !int.TryParse( (string)value, out convertedValue ) )
				{
					convertedValue = DefaultValue;
				}
			}
			return convertedValue;
		}

		#endregion
	}
}