using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GCloud.Core.Contract;

using GCloudApp.Models;
using GCloudApp.Translation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GCloudApp.Controllers
{
    [Route("api/[controller]")]
    public class SubscribersController : BaseApiController
	{
		private readonly ICoreServices mCoreServices;

		public SubscribersController( ICoreServices coreServices, ILoggerFactory loggerFactory, IConfiguration configuration )
			: base(loggerFactory, configuration)
		{
			mCoreServices = coreServices;
		}

		// GET: api/<controller>
		[HttpGet]
		public async Task<IActionResult> Get( GetSubscribersRequest request )
		{
			return await ProcessRequest( async () =>
			{
				var subscribers = (await mCoreServices.GetAllSubscribers()).ToArray();
				var result = subscribers.Select( ApiTranslator.TranslateToApiSubscriber ).ToArray();

				if( request.IncludeCount == true )
				{
					var count = subscribers.Count();
					AddCustomResponseHeader( "X-Total-Count", count.ToString() );
				}

				return result;
			} );
		}

		// GET: api/Subscribers/5
		// [HttpGet("{id}", Name = "Get")]
  //       public string Get(int id)
  //       {
  //           return "value";
  //       }

        // POST: api/Subscribers
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Subscribers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
