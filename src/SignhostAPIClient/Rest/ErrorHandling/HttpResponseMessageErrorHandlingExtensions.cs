using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	/// <summary>
	/// Error handling around <see cref="HttpResponseMessage"/>s.
	/// </summary>
	public static class HttpResponseMessageErrorHandlingExtensions
	{
		private const string OutOfCreditsApiProblemType =
			"https://api.signhost.com/problem/subscription/out-of-credits";

		/// <summary>
		/// Throws an exception if the <see cref="HttpResponseMessage.StatusCode"/>
		/// has an error code.
		/// </summary>
		/// <param name="responseTask"><see cref="HttpResponseMessage"/></param>
		/// <returns>Returns <see cref="HttpResponseMessage"/> if the call is succesful.</returns>
		/// <param name="expectedStatusCodes">List of <see cref="HttpStatusCode"/> which should
		/// not be handled as an error.</param>
		/// <exception cref="UnauthorizedAccessException">
		/// When the api authentication failed.
		/// </exception>
		/// <exception cref="BadRequestException">
		/// When the API request was an invalid request for your account.
		/// </exception>
		/// <exception cref="OutOfCreditsException">
		/// When your organisation has run out of credits.
		/// </exception>
		/// <exception cref="NotFoundException">
		/// When the request resource (ie transaction id or file id) was not found.
		/// </exception>
		/// <exception cref="InternalServerErrorException">
		/// When the API was unable to proces the request at the moment,
		/// a RetryAfter property is set if available.
		/// </exception>
		/// <exception cref="SignhostRestApiClientException">
		/// An other unknown API error occured.
		/// </exception>
		public static async Task<HttpResponseMessage> EnsureSignhostSuccessStatusCodeAsync(
			this Task<HttpResponseMessage> responseTask,
			params HttpStatusCode[] expectedStatusCodes)
		{
			var response = await responseTask.ConfigureAwait(false);

			if (response.IsSuccessStatusCode) {
				return response;
			}

			if (expectedStatusCodes.Contains(response.StatusCode)) {
				return response;
			}

			string errorType = string.Empty;
			string errorMessage = "Unknown Signhost error";
			string responseBody = string.Empty;

			if (response.Content != null) {
				responseBody = await response.Content.ReadAsStringAsync()
					.ConfigureAwait(false);

				var error = JsonConvert.DeserializeAnonymousType(
					responseBody,
					new {
						Type = string.Empty,
						Message = string.Empty,
					});

				errorType = error.Type;
				errorMessage = error.Message;
			}

			// TO-DO: Use switch pattern in v5
			Exception exception = null;
			switch (response.StatusCode) {
				case HttpStatusCode.Unauthorized:
					exception = new UnauthorizedAccessException(
						errorMessage);
					break;

				case HttpStatusCode.BadRequest:
					exception = new BadRequestException(
						errorMessage);
					break;

				case HttpStatusCode.PaymentRequired
				when errorType == OutOfCreditsApiProblemType:
					exception = new OutOfCreditsException(
						errorMessage);
					break;

				case HttpStatusCode.NotFound:
					exception = new NotFoundException(
						errorMessage);
					break;

				case HttpStatusCode.InternalServerError:
					exception = new InternalServerErrorException(
						errorMessage, response.Headers.RetryAfter);
					break;

				default:
					exception = new SignhostRestApiClientException(
						errorMessage);
					break;
			}

			if (exception is SignhostRestApiClientException signhostException) {
				signhostException.ResponseBody = responseBody;
			}

			throw exception;
		}
	}
}
