using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace Cube.TestClient.Core
{
	public interface IServiceOperationCommand : ICommand, INotifyPropertyChanged
	{
		#region Properties

		bool IsProcessing { get; set; }

		/*
		DelegateCommand CancelCommand
		{
			get;
		}

		bool CommitTransaction
		{
			get;
			set;
		}

		string HintText
		{
			get;
		}

		bool LoopCalls
		{
			get;
			set;
		}

		int LoopCount
		{
			get;
			set;
		}

		TimeSpan OperationElapsed
		{
			get;
			set;
		}
*/

		DataTable OperationResults { get; set; }

		#endregion

		/*
		int TabIndex
		{
			get;
			set;
		}

		bool UseTransaction
		{
			get;
			set;
		}
*/
	}
}