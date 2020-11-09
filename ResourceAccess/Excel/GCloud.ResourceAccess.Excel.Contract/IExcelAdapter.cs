namespace GCloud.ResourceAccess.Excel.Contract
{
	public interface IExcelAdapter
	{
		#region Members

		WisdomDrop[] GetAll( string filePath );

		WisdomDrop[] ExtractWisdomDropsFromExcel( string base64data );

		#endregion
	}
}