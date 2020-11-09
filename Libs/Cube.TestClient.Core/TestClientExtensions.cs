using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cube.TestClient.Core
{
	public static class TestClientExtensions
	{
		#region TestClientExtensions Members

		public static DataTable DumpListToTable<TTarget>(this IEnumerable<TTarget> targets, string dataColumnHeader = "Value")
		{
			DataTable table;
			if (typeof(TTarget).IsEnum)
			{
				table = _DumpEnumListToDataTable(targets);
			}
			else if (_IsPrimitiveType<TTarget>())
			{
				table = _DumpPrimitiveListToDataTable(targets, dataColumnHeader);
			}
			else
			{
				table = _DumpListToTable(targets);
			}
			return table;
		}

		public static DataTable DumpObjectGraphToTable<TTarget>(this TTarget target)
		{
			var table = new DataTable();

			_DumpObjectGraphToTable(target, table, 0);

			return table;
		}

		public static DataTable DumpPropertiesToTable<TTarget>(this TTarget target)
		{
			var table = new DataTable();
			table.Columns.Add("Row", typeof(int));
			table.Columns.Add("Property", typeof(string));
			table.Columns.Add("Value", typeof(string));

			if (target != null)
			{
				var targetType = target.GetType();
				var properties = targetType.GetProperties().Where(prop => prop.GetIndexParameters().Length == 0);

				int index = 0;
				foreach (var prop in properties)
				{
					index++;
					table.Rows.Add(index, prop.Name, _FormatValue(prop.GetValue(target, null)));
				}
			}
			return table;
		}

		public static DataTable DumpValueToTable<TTarget>(this TTarget target)
		{
			var table = new DataTable();
			table.Columns.Add("Row", typeof(int));
			table.Columns.Add("Value", typeof(string));

			table.Rows.Add(1, _FormatValue(target));

			return table;
		}

		public static string[] EnumValues(Type type)
		{
			List<string> hintAppend = new List<string>();
			foreach (var value in Enum.GetValues(type))
			{
				hintAppend.Add(string.Format("{0}={1}", (int)value, value.ToString()));
			}
			return hintAppend.ToArray();
		}

		public static string FormatByteArray(byte[] byteArray)
		{
			byteArray = byteArray == null ? new byte[] { } : byteArray.Reverse().ToArray();

			return byteArray.Length == 0 ? string.Empty : string.Format("0x{0:X16}", BitConverter.ToUInt64(byteArray, 0));
		}

		#endregion TestClientExtensions Members

		#region Private Members

		private static void _DumpEnumerablePropertiesToTable(object parent, DataTable dataTable, int level)
		{
			var enumerableProperties = _GetEnumerableProperties(parent);

			foreach (var enumerable in enumerableProperties)
			{
				dataTable.Rows.Add(_GetFiller(level) + enumerable.Item1);

				int index = 0;
				foreach (var item in enumerable.Item2)
				{
					if (item != null && _IsSimpleType(item.GetType()))
					{
						string data = string.Format("{0}[{1}] {2}",
							_GetFiller(level + 1), index, _FormatValue(item));
						dataTable.Rows.Add(data);
						index++;
					}
					else
					{
						_DumpObjectGraphToTable(item, dataTable, level + 1);
					}
				}
			}
		}

		private static DataTable _DumpEnumListToDataTable<TEnum>(IEnumerable<TEnum> targets)
		{
			var table = new DataTable();
			table.Columns.Add("Row", typeof(int));
			table.Columns.Add("Enum value", typeof(string));
			table.Columns.Add("Raw Value", typeof(int));

			int rowIndex = 0;
			foreach (var target in targets)
			{
				rowIndex++;

				var values = new object[] { rowIndex, target.ToString(), (int)(object)target };

				table.Rows.Add(values);
			}

			return table;
		}

		private static DataTable _DumpListToTable<TTarget>(IEnumerable<TTarget> targets)
		{
			var table = new DataTable();
			table.Columns.Add("Row", typeof(int));


			var targetType = typeof(TTarget);
			var properties = targetType.GetProperties().Where(prop => prop.GetIndexParameters().Length == 0).ToArray();

			foreach (var prop in properties)
			{
				table.Columns.Add(prop.Name);
			}

			int rowIndex = 0;
			foreach (var target in targets)
			{
				rowIndex++;
				var values = new object[properties.Length + 1];
				values[0] = rowIndex;

				for (int index = 0; index < properties.Length; index++)
				{
					var prop = properties[index];
					values[index + 1] = _FormatValue(prop.GetValue(target));
				}
				table.Rows.Add(values);
			}

			return table;
		}

		private static void _DumpNonSimplePropertiesToTable(object parent, DataTable dataTable, int level)
		{
			var nonSimpleObjects = _GetNonSimpleChildObjects(parent);

			foreach (var nonSimpleObject in nonSimpleObjects)
			{
				dataTable.Rows.Add();
				_DumpObjectGraphToTable(nonSimpleObject.Item2, dataTable, level + 1, nonSimpleObject.Item1);
			}
		}

		private static void _DumpObjectGraphToTable(object target, DataTable dataTable, int level, string targetName = null)
		{
			_DumpTargetTypeToTable(target, dataTable, level, targetName);

			_DumpSimplePropertiesToTable(target, dataTable, level);

			_DumpEnumerablePropertiesToTable(target, dataTable, level);

			_DumpNonSimplePropertiesToTable(target, dataTable, level);
		}

		private static DataTable _DumpPrimitiveListToDataTable<TTarget>(IEnumerable<TTarget> targets, string dataColumnHeader)
		{
			var table = new DataTable();

			table.Columns.Add("Row", typeof(int));
			table.Columns.Add(dataColumnHeader, typeof(string));

			int rowIndex = 0;
			foreach (var target in targets)
			{
				rowIndex++;

				var values = new object[] { rowIndex, target.ToString() };

				table.Rows.Add(values);
			}

			return table;
		}

		private static void _DumpSimplePropertiesToTable(object parent, DataTable dataTable, int level)
		{
			var simpleProperties = _GetSimplePropertyValues(parent);

			var simplePropertyNames = simpleProperties.Select(pp => pp.Item1).ToArray();
			var simplePropertyValues = simpleProperties.Select(pp => pp.Item2).ToArray();

			if (dataTable.Columns.Count < simpleProperties.Count())
			{
				int difference = simpleProperties.Count() - dataTable.Columns.Count;

				for (int count = 0; count < difference; count++)
				{
					dataTable.Columns.Add();
				}
			}

			if (simpleProperties.Any())
			{
				simplePropertyNames[0] = _GetFiller(level) + simplePropertyNames[0];
				simplePropertyValues[0] = _GetFiller(level) + simplePropertyValues[0];

				dataTable.Rows.Add(simplePropertyNames);
				dataTable.Rows.Add(simplePropertyValues);
			}
		}

		private static void _DumpTargetTypeToTable(object target, DataTable dataTable, int level, string propertyName = null)
		{
			var className =
				propertyName != null ?
					string.Format("{0}---- {1} ({2}) ----", _GetFiller(level), propertyName, target.GetType().Name) :
					string.Format("{0}---- {1} ----", _GetFiller(level), target.GetType().Name);

			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add();
			}

			dataTable.Rows.Add(className);
		}

		private static string _FormatValue(object value)
		{
			string formatedValue;
			if (value == null)
			{
				formatedValue = "{null}";
			}
			else if (value is byte[])
			{
				formatedValue = FormatByteArray((byte[])value);
			}
			else
			{
				formatedValue = value.ToString();
			}

			return formatedValue;
		}

		private static IEnumerable<Tuple<string, IEnumerable>> _GetEnumerableProperties(object parent)
		{
			var allProperties = parent.GetType().GetProperties();

			var enumerableProperties = allProperties.Where(pi => _IsEnumerableType(pi.PropertyType)).OrderBy(pi => pi.Name);

			List<Tuple<string, IEnumerable>> enumerables = new List<Tuple<string, IEnumerable>>();

			foreach (var property in enumerableProperties)
			{
				var enumerable = (IEnumerable)property.GetValue(parent);

				if (enumerable != null)
				{
					enumerables.Add(new Tuple<string, IEnumerable>(property.Name + "[ ]", enumerable));
				}
			}

			return enumerables;
		}

		private static string _GetFiller(int level)
		{
			return new string(' ', level * 8);
		}

		private static IEnumerable<Tuple<string, object>> _GetNonSimpleChildObjects(object parent)
		{
			var allProperties = parent.GetType().GetProperties();

			var nonSimpleProperties = allProperties.Where(pi => _IsNonSimpleType(pi.PropertyType)).OrderBy(pi => pi.Name);

			List<Tuple<string, object>> childObjects = new List<Tuple<string, object>>();

			foreach (var property in nonSimpleProperties)
			{
				var childObject = property.GetValue(parent);

				if (childObject != null)
				{
					childObjects.Add(new Tuple<string, object>(property.Name, childObject));
				}
			}

			return childObjects;
		}

		private static IEnumerable<Tuple<string, string>> _GetSimplePropertyValues(object target)
		{
			var allProperties = target.GetType().GetProperties();

			var simpleProperties = allProperties.Where(pi => _IsSimpleType(pi.PropertyType)).OrderBy(pi => pi.Name);

			List<Tuple<string, string>> primitivePropertyValues = new List<Tuple<string, string>>();

			foreach (var simpleProperty in simpleProperties)
			{
				string propertyName = simpleProperty.Name;
				string propertyValue = _FormatValue(simpleProperty.GetValue(target));

				primitivePropertyValues.Add(new Tuple<string, string>(propertyName, propertyValue));
			}

			return primitivePropertyValues;
		}

		private static bool _IsEnumerableType(Type type)
		{
			return type != typeof(string) && !_IsRowVersionType(type) && type.GetInterfaces().Contains(typeof(IEnumerable));
		}

		private static bool _IsNonSimpleType(Type type)
		{
			return !_IsSimpleType(type) && !_IsEnumerableType(type) && !_IsRowVersionType(type);
		}

		private static bool _IsPrimitiveType(Type type)
		{
			return type.IsPrimitive || type == typeof(string) || type == typeof(Guid);
		}

		private static bool _IsPrimitiveType<TTarget>()
		{
			return _IsPrimitiveType(typeof(TTarget));
		}

		private static bool _IsRowVersionType(Type type)
		{
			return type == typeof(byte[]);
		}

		private static bool _IsSimpleType(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}

			return
				type.IsPrimitive || type.IsEnum || _IsRowVersionType(type) || type == typeof(decimal) ||
				type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(object);
		}

		#endregion Private Members

	}

}
