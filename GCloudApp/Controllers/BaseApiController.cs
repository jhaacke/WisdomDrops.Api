using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GCloudApp.Controllers
{
	public class BaseApiController : Controller
	{
		#region Fields

		private static readonly string mGenericServerErrorMessage = "An unexpected error occurred. Please try again.";

		private readonly ILoggerFactory mLoggerFactory;

		private ILogger mLogger;

		#endregion

		#region Constructors

		public BaseApiController(
			ILoggerFactory loggerFactory,
			IConfiguration configuration )
		{
			Configuration = configuration;
			mLoggerFactory = loggerFactory;

			JsonSettings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		#endregion

		#region Properties

		protected IConfiguration Configuration { get; }

		protected JsonSerializerSettings JsonSettings { get; }

		protected ILogger Logger => mLogger ?? ( mLogger = mLogger = mLoggerFactory.CreateLogger( "ServiceLayer" ) );

		#endregion

		#region Members

		protected void AddCustomResponseHeader( string header, string value )
		{
			Request.HttpContext.Response.Headers.Add( "Access-Control-Expose-Headers", header );
			Request.HttpContext.Response.Headers.Add( header, value );
		}

		protected IActionResult ProcessRequest<T>( Func<T> func )
		{
			var result = ProcessRequest( () => Task.FromResult( func() ) ).Result;
			return result;
		}

		protected async Task<IActionResult> ProcessRequest( Func<Task> func )
		{
			Func<Task<Object>> wrapper = async () =>
			{
				await func();
				return null;
			};

			var result = await ProcessRequest( wrapper );
			return result;
		}

		protected async Task<IActionResult> ProcessRequest( Action action )
		{
			Func<Task<Object>> wrapper = () =>
			{
				action();
				return Task.FromResult<Object>( null );
			};

			var result = await ProcessRequest( wrapper );
			return result;
		}

		protected async Task<IActionResult> ProcessRequest<T>( Func<Task<T>> func )
		{
			IActionResult result = null;

			if( ModelState.IsValid )
			{
				try
				{
					Logger.LogDebug( "Processing a request..." );

					var payload = await func();
					result = payload != null
						? (IActionResult)new JsonResult( payload, JsonSettings )
						: new OkResult();

					Logger.LogDebug( "Request processing complete." );
				}
				catch( Exception ex )
				{
					Logger.LogError( ex, "An exception was caught by the API.\r\nMessage: {0}\r\nStack Trace:\r\n{1}", ex.Message, ex.StackTrace );

					if( ex is AggregateException && ex.InnerException != null )
					{
						ex = ex.InnerException;
					}

					// if( ex is ValidationFailedException )
					// {
					// 	var modelStateDict = _CreateModelStateForValidationFailedException( ex as ValidationFailedException );
					// 	result = new BadRequestObjectResult( modelStateDict );
					// }
					// else if( ex is UnauthorizedAccessException )
					if( ex is UnauthorizedAccessException )
					{
						result = new UnauthorizedResult();
					}
					else if( ex is NotFoundException )
					{
						result = new NotFoundResult();
					}
					else
					{
						result = new StatusCodeResult( 500 );
					}
				}
			}
			else
			{
				result = new BadRequestObjectResult( ModelState );
			}

			return result;
		}

		#endregion
	}
}