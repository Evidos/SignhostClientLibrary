using System.Net;
using Flurl.Http;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	public class ErrorHandling
	{
		public static void HandleError(HttpCall call)
		{
			string errorMessage = GetErrorMessage(call);

			switch (call.HttpStatus) {
				case HttpStatusCode.Unauthorized:
					call.Exception = new System.UnauthorizedAccessException(
						errorMessage, call.Exception);
					break;
				case HttpStatusCode.BadRequest:
					call.Exception = new BadRequestException(
						errorMessage, call.Exception);
					break;
				case HttpStatusCode.NotFound:
					call.Exception = new NotFoundException(
						errorMessage, call.Exception);
					break;
				case HttpStatusCode.InternalServerError:
					call.Exception = new InternalServerErrorException(
						errorMessage, call.Exception, call.Response.Headers.RetryAfter);
					break;
				default:
					call.Exception = new SignhostRestApiClientException(
						errorMessage, call.Exception);
					break;
			}
		}

		private static string GetErrorMessage(HttpCall call)
		{
			string responseJson = call?.Response?.Content?.ReadAsStringAsync()?.Result;
			if (responseJson != null) {
				var error = JsonConvert.DeserializeAnonymousType(responseJson, new { Message = string.Empty });
				return error.Message;
			}
			else {
				return "Unknown Signhost Error";
			}
		}
	}
}
