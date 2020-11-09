using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

using GCloud.ResourceAccess.Excel.Contract;

using OfficeOpenXml;

namespace GCloud.ResourceAccess.Excel
{
	public class ExcelAdapter : IExcelAdapter
	{
		#region Members

		public WisdomDrop[] ExtractWisdomDropsFromExcel( string base64data )
		{
			var bytes = Convert.FromBase64String( base64data );
			using( var stream = new MemoryStream( bytes ) )
			{
				using( var package = new ExcelPackage( stream ) )
				{
					//get the first worksheet in the workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
					int colCount = worksheet.Dimension.End.Column;
					int rowCount = worksheet.Dimension.End.Row;

					var headerRange = worksheet.Cells[1, 1, 1, colCount];
					var colCtx = _GetColumnContextForHeader( headerRange );

					var drops = new List<WisdomDrop>();
					for( int rowIdx = 2; rowIdx <= rowCount; rowIdx++ )
					{
						var row = worksheet.Cells[rowIdx, 1, rowIdx, colCtx.MaxIdx];

						if( _CellIsEmtpy( row[row.Start.Row, colCtx.TipIdx] ) )
						{
							break;
						}

						var drop = _CreateWisdomDrop( colCtx, row );
						drops.Add( drop );
					}

					return drops.ToArray();
				}
			}
		}

		public WisdomDrop[] GetAll( string filePath )
		{
			FileInfo existingFile = new FileInfo( filePath );
			using( ExcelPackage package = new ExcelPackage( existingFile ) )
			{
				//get the first worksheet in the workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
				int colCount = worksheet.Dimension.End.Column;
				int rowCount = worksheet.Dimension.End.Row;

				var headerRange = worksheet.Cells[1, 1, 1, colCount];
				var colCtx = _GetColumnContextForHeader( headerRange );

				var drops = new List<WisdomDrop>();
				for( int rowIdx = 2; rowIdx <= rowCount; rowIdx++ )
				{
					var row = worksheet.Cells[rowIdx, 1, rowIdx, colCtx.MaxIdx];

					if( _CellIsEmtpy( row[row.Start.Row, colCtx.TipIdx] ) )
					{
						break;
					}

					var drop = _CreateWisdomDrop( colCtx, row );
					drops.Add( drop );
				}

				return drops.ToArray();
			}
		}

		private bool _CellHasValue( ExcelRangeBase cell, string target )
		{
			var result = cell.Value != null && cell.Value.ToString().Trim().Equals( target, StringComparison.InvariantCultureIgnoreCase );
			return result;
		}

		private bool _CellIsEmtpy( ExcelRangeBase cell )
		{
			return cell.Value?.ToString()?.Trim() == null;
		}

		private WisdomDrop _CreateWisdomDrop( ColumnContext colCtx, ExcelRange row )
		{
			var idString = _GetCellValue( row[row.Start.Row, colCtx.IdIdx] );
			var ratingString = _GetCellValue( row[row.Start.Row, colCtx.RatingIdx] );

			var result = new WisdomDrop
			{
				Tip = _GetCellValue( row[row.Start.Row, colCtx.TipIdx] ),
				Id = !string.IsNullOrEmpty( idString ) && Guid.TryParse( idString, out var parsedId )
					? parsedId
					: (Guid?)null,
				Category = _GetCellValue( row[row.Start.Row, colCtx.CategoryIdx] ),
				Rating = !string.IsNullOrEmpty( ratingString ) && int.TryParse( ratingString, out var parsedRating )
					? parsedRating
					: 0,
				KeyTopic = _GetCellValue( row[row.Start.Row, colCtx.KeyTopicIdx] )
			};

			return result;
		}

		private string _GetCellValue( ExcelRangeBase cell )
		{
			return cell.Value?.ToString().Trim();
		}

		private ColumnContext _GetColumnContextForHeader( ExcelRange headerRange )
		{
			var result = new ColumnContext();

			foreach( var cell in headerRange )
			{
				if( _CellHasValue( cell, "ID" ) )
				{
					result.IdIdx = cell.Start.Column;
				}
				else if( _CellHasValue( cell, "More" ) )
				{
					result.MoreIdx = cell.Start.Column;
				}
				else if( _CellHasValue( cell, "Rating" ) )
				{
					result.RatingIdx = cell.Start.Column;
				}
				else if( _CellHasValue( cell, "Tip" ) )
				{
					result.TipIdx = cell.Start.Column;
				}
				else if( _CellHasValue( cell, "Category" ) )
				{
					result.CategoryIdx = cell.Start.Column;
				}
				else if( _CellHasValue( cell, "Key Topic" ) )
				{
					result.KeyTopicIdx = cell.Start.Column;
				}
			}

			return result;
		}

		#endregion
	}

	internal class ColumnContext
	{
		#region Properties

		public int CategoryIdx { get; set; }

		public int IdIdx { get; set; }

		public int KeyTopicIdx { get; set; }

		public int MaxIdx => new[] { IdIdx, RatingIdx, TipIdx, CategoryIdx, KeyTopicIdx }.Max();

		public int MoreIdx { get; set; }

		public int RatingIdx { get; set; }

		public int TipIdx { get; set; }

		#endregion
	}
}