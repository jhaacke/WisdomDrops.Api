using System;
using System.Configuration;
using System.Linq;

using GCloud.ResourceAccess.Excel;

namespace GCloud.ResourceAccess.TestConsole
{
	class Program
	{
		static void Main( string[] args )
		{
			var excelPath = @"D:\temp\WisdomDrops.xlsx";

			var adapter = new ExcelAdapter();
			adapter.GetAll( excelPath ).ToList()
				.ForEach( drop => Console.WriteLine( $"ID:{drop.Id?.ToString()?? "NULL"} Tip:{drop.Tip} Rating:{drop.Rating}" ) );

			Console.ReadKey();
		}
	}
}
