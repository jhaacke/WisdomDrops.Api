using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GCloud.Core.Contract;

using GCloudApp.Models;
using GCloudApp.Translation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace GCloudApp.Controllers
{
	[Route( "api/[controller]" )]
	public class AffirmationsController : BaseApiController
	{
		#region Fields

		private readonly ICoreServices mCoreServices;

		#endregion

		#region Constructors

		public AffirmationsController( ICoreServices coreServices, ILoggerFactory loggerFactory, IConfiguration configuration )
			: base( loggerFactory, configuration )
		{
			mCoreServices = coreServices;
		}

		#endregion

		#region Properties

		#endregion

		#region Members

		// DELETE api/<controller>/5
		[HttpDelete( "{id}" )]
		public void Delete( int id )
		{
		}

		// GET: api/<controller>
		[HttpGet]
		public async Task<IActionResult> Get( GetAffirmationsRequest request )
		{
			return await ProcessRequest( async () =>
			{
				var affirmations = ( await mCoreServices.GetAllAffirmations() ).ToArray();
				var result = affirmations.Select( ApiTranslator.TranslateToApiAffirmation ).ToArray();

				if( request.IncludeCount == true )
				{
					var count = affirmations.Count();
					AddCustomResponseHeader( "X-Total-Count", count.ToString() );
				}

				return result;
			} );
		}

		// GET api/<controller>/5
		[HttpGet( "{id}" )]
		public async Task Get( string id )
		{
			await mCoreServices.SendAffirmationToSubscriber( id );
		}

		// POST api/<controller>
		[HttpPost]
		public void Post( [FromBody] string value )
		{
		}

		// PUT api/<controller>/5
		[HttpPut]
		public async Task Put()
		{
			await mCoreServices.ScheduleAffirmationsForDay();
		}

		[HttpPost]
		[Route( "spreadsheet" )]
		public async Task<UploadFromSpreadsheetResponse> UploadFromSpreadsheetAsync( [FromBody] Spreadsheet spreadsheet )
		{
			var result = new UploadFromSpreadsheetResponse
			{
				Success = true
			};

			try
			{
				var importResult = await mCoreServices.ImportAffirmationsFromSpreadsheet( spreadsheet.SpreadsheetData );
				result.Report = _BuildUploadReport( importResult );
			}
			catch( Exception ex )
			{
				result.ErrorMessage = ex.Message;
				result.Success = false;
			}

			return result;
		}

		#endregion

		private string _BuildUploadReport( AffirmationImportResult importResult )
		{
			var reportContent = new StringBuilder();
			Func<int, string> topicClause = count => _GetClause( "topic", "topics", count );
			Func<int, string> categoryClause = count => _GetClause( "category", "categories", count );
			Func<int, string> affirmationClause = count => _GetClause( "affirmation", "affirmations", count );

			if( _HasCount( importResult.TopicsDeleted ) )
			{
				reportContent.AppendLine( $"{topicClause( importResult.TopicsDeleted.Item1 )} flagged for delete. {topicClause( importResult.TopicsDeleted.Item2 )} successfully deleted." );
			}

			if( _HasCount( importResult.TopicsAdded ) )
			{
				reportContent.AppendLine( $"{topicClause( importResult.TopicsAdded.Item1 )} discovered to add. {topicClause( importResult.TopicsAdded.Item2 )} successfully added." );
			}

			if( _HasCount( importResult.CategoriesDeleted ) )
			{
				reportContent.AppendLine( $"{categoryClause( importResult.CategoriesDeleted.Item1 )} flagged for delete. {categoryClause( importResult.CategoriesDeleted.Item2 )} successfully deleted." );
			}

			if( _HasCount( importResult.CategoriesAdded ) )
			{
				reportContent.AppendLine( $"{categoryClause( importResult.CategoriesAdded.Item1 )} discovered to add. {categoryClause( importResult.CategoriesAdded.Item2 )} successfully added." );
			}

			if( _HasCount( importResult.AffirmationsDeleted ) )
			{
				reportContent.AppendLine( $"{affirmationClause( importResult.AffirmationsDeleted.Item1 )} flagged for delete. {affirmationClause( importResult.AffirmationsDeleted.Item2 )} successfully deleted." );
			}

			if( _HasCount( importResult.AffirmationsAdded ) )
			{
				reportContent.AppendLine( $"{affirmationClause( importResult.AffirmationsAdded.Item1 )} discovered to add. {affirmationClause( importResult.AffirmationsAdded.Item2 )} successfully added." );
			}

			if( _HasCount( importResult.AffirmationsUpdated ) )
			{
				reportContent.AppendLine( $"{affirmationClause( importResult.AffirmationsAdded.Item1 )} flagged to update. {affirmationClause( importResult.AffirmationsAdded.Item2 )} successfully updated." );
			}

			var result = reportContent.ToString();
			return result;
		}

		private string _GetClause( string singular, string plural, int count )
		{
			var result = count == 1
				? $"{count} {singular} was"
				: $"{count} {plural} were";
			return result;
		}

		private bool _HasCount( Tuple<int, int> source )
		{
			return source != null && source.Item1 > 0;
		}
	}
}
