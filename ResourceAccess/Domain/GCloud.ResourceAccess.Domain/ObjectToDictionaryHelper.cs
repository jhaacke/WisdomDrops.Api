using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GCloud.ResourceAccess.Domain
{
	public static class ObjectToDictionaryHelper
	{
		#region ObjectToDictionaryHelper Members

		public static IDictionary<string, object> ToDictionary( this object source )
		{
			return source.ToDictionary<object>();
		}

		public static IDictionary<string, T> ToDictionary<T>( this object source )
		{
			if( source == null )
				_ThrowExceptionWhenSourceArgumentIsNull();

			var dictionary = new Dictionary<string, T>();
			foreach( PropertyDescriptor property in TypeDescriptor.GetProperties( source ) )
				_AddPropertyToDictionary<T>( property, source, dictionary );
			return dictionary;
		}

		#endregion ObjectToDictionaryHelper Members

		#region Private Members

		private static void _AddPropertyToDictionary<T>( PropertyDescriptor property, object source, Dictionary<string, T> dictionary )
		{
			object value = property.GetValue( source );
			if( _IsOfType<T>( value ) )
				dictionary.Add( property.Name, (T)value );
		}

		private static bool _IsOfType<T>( object value )
		{
			return value is T;
		}

		private static void _ThrowExceptionWhenSourceArgumentIsNull()
		{
			throw new ArgumentNullException( "source", "Unable to convert object to a dictionary. The source object is null." );
		}

		#endregion Private Members

	}
}