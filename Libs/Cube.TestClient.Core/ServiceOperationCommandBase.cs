using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cube.TestClient.Core
{
	public abstract class ServiceOperationCommandBase<TServiceContract> : IServiceOperationCommand
	{
		#region Fields

		private bool mIsProcessing;

		private DataTable mOperationResults;

		#endregion

		#region Properties

		public bool IsProcessing
		{
			get
			{
				return mIsProcessing;
			}
			set
			{
				if( !value.Equals( mIsProcessing ) )
				{
					mIsProcessing = value;
					OnPropertyChanged();
				}
			}
		}

		public DataTable OperationResults
		{
			get
			{
				return mOperationResults;
			}
			set
			{
				if( mOperationResults != value )
				{
					mOperationResults = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		#region Members

		public bool CanExecute( object parameter )
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute( object parameter )
		{
			DataTable resultsTable;

			var stopwatch = new Stopwatch();

			try
			{
				IsProcessing = true;
				var service = DoCreateService();
				stopwatch.Start();
				resultsTable = DoProcessRequest( service );
			}
			catch( Exception ex )
			{
				resultsTable = _GetDataForExecutionException( ex );
			}

			stopwatch.Stop();
			IsProcessing = false;
			OperationResults = resultsTable;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected abstract TServiceContract DoCreateService();

		protected abstract DataTable DoProcessRequest( TServiceContract service );

		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			var handler = PropertyChanged;
			if( handler != null )
			{
				handler( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		private DataTable _GetDataForExecutionException( Exception ex )
		{
			var result = new DataTable();
			result.Columns.Add( "Exception", typeof(string) );
			result.Rows.Add( ex.ToString() );

			if( ex is FileLoadException )
			{
				var innerException = ex.InnerException as ReflectionTypeLoadException;
				if( innerException != null )
				{
					result.Rows.Add( "Inner Exception" );
					foreach( var loaderException in innerException.LoaderExceptions )
					{
						result.Rows.Add( loaderException.Message );
					}
				}
			}

			return result;
		}

		#endregion
	}
}