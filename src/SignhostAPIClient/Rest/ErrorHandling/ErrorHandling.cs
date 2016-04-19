using System.Net;
using Flurl.Http;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	public class ErrorHandling
	{
		public static void HandleError(HttpCall call)
		{
			string responseJson = call.Response.Content.ReadAsStringAsync().Result;
			var error = JsonConvert.DeserializeAnonymousType(responseJson, new { Message = string.Empty });

			switch (call.HttpStatus) {
				case HttpStatusCode.Unauthorized:
					call.Exception = new System.UnauthorizedAccessException(
						error.Message, call.Exception);
					break;
				case HttpStatusCode.BadRequest:
					call.Exception = new BadRequestException(
						error.Message, call.Exception);
					break;
				case HttpStatusCode.NotFound:
					call.Exception = new NotFoundException(
						error.Message, call.Exception);
					break;
				case HttpStatusCode.InternalServerError:
					call.Exception = new InternalServerErrorException(
						error.Message, call.Exception, call.Response.Headers.RetryAfter);
					break;
				default:
					call.Exception = new SignhostRestApiClientException(
						error.Message, call.Exception);
					break;
			}
		}
	}
}
