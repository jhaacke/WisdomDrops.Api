namespace GCloud.ResourceAccess.Excel.TestClient
{
	public class ExcelResourceAccessViewModel
	{
		#region Fields

		private GetAllCommand mGetAllCommand;

		#endregion

		#region Properties

		public GetAllCommand GetAllCommand => mGetAllCommand ?? ( mGetAllCommand = new GetAllCommand() );

		#endregion
	}
}