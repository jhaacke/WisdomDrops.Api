using System;
using System.Globalization;
using System.Windows.Data;

namespace Cube.TestClient.Core
{
	[ValueConversion( typeof(string), typeof(Guid) )]
	public class StringToGuidConverter : IValueConverter
	{
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
			object convertedValue = targetType.IsGenericType
				? (Guid?)null
				: Guid.Empty;
			if( value != null )
			{
				Guid parsedValue;
				if( Guid.TryParse( (string)value, out parsedValue ) )
				{
					convertedValue = parsedValue;
				}
			}
			return convertedValue;
		}

		#endregion
	}
}