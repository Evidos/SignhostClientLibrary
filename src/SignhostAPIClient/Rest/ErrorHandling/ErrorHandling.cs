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
					throw new System.UnauthorizedAccessException(
						errorMessage, call.Exception);
				case HttpStatusCode.BadRequest:
					throw new BadRequestException(
						errorMessage, call.Exception);
				case HttpStatusCode.NotFound:
					throw new NotFoundException(
						errorMessage, call.Exception);
				case HttpStatusCode.InternalServerError:
					throw new InternalServerErrorException(
						errorMessage, call.Exception, call.Response.Headers.RetryAfter);
				default:
					throw new SignhostRestApiClientException(
						errorMessage, call.Exception);
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
