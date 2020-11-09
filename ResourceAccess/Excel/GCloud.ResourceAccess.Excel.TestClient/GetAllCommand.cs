using System.Configuration;
using System.Data;

using Cube.TestClient.Core;

using GCloud.ResourceAccess.Excel.Contract;

namespace GCloud.ResourceAccess.Excel.TestClient
{
	public class GetAllCommand : ServiceOperationCommandBase<IExcelAdapter>
	{
		#region Constructors

		public GetAllCommand()
		{
			ExcelFilePath = ConfigurationManager.AppSettings["DefaultExcelFilePath"];
		}

		#endregion

		#region Properties

		public string ExcelFilePath { get; set; }

		#endregion

		#region Members

		protected override IExcelAdapter DoCreateService()
		{
			return new ExcelAdapter();
		}

		protected override DataTable DoProcessRequest( IExcelAdapter service )
		{
			var result = service.GetAll( ExcelFilePath );
			return result.DumpListToTable();
		}

		#endregion
	}
}