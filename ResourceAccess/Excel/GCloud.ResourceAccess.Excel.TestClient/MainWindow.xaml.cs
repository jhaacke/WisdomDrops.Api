using System.Windows;

namespace GCloud.ResourceAccess.Excel.TestClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructors

		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ExcelResourceAccessViewModel();
		}

		#endregion
	}
}