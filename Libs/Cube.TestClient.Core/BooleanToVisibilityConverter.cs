using System;
using System.Windows;
using System.Windows.Data;

namespace Cube.TestClient.Core
{
	[ValueConversion( typeof(bool), typeof(Visibility) )]
	public class BooleanToVisibilityConverter : IValueConverter
	{
		#region Constructors

		public BooleanToVisibilityConverter()
		{
			TrueVisibility = Visibility.Visible;
			FalseVisibility = Visibility.Collapsed;
		}

		#endregion

		#region Properties

		public Visibility FalseVisibility { get; set; }

		public Visibility NullVisiblity { get; set; }

		public Visibility TrueVisibility { get; set; }

		#endregion

		#region Members

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var visibility = NullVisiblity;
			if( value != null )
			{
				visibility = System.Convert.ToBoolean( value )
					? TrueVisibility
					: FalseVisibility;
			}
			return visibility;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}