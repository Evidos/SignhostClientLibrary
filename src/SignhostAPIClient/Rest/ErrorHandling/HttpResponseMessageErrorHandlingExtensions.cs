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

			string errorMessage = "Unknown Signhost error";

			if (response.Content != null) {
				string responsejson = await response.Content.ReadAsStringAsync()
					.ConfigureAwait(false);

				var error = JsonConvert.DeserializeAnonymousType(responsejson, new { Message = string.Empty });
				errorMessage = error.Message;
			}

			switch (response.StatusCode) {
				case HttpStatusCode.Unauthorized:
					throw new System.UnauthorizedAccessException(
						errorMessage);
				case HttpStatusCode.BadRequest:
					throw new BadRequestException(
						errorMessage);
				case HttpStatusCode.NotFound:
					throw new NotFoundException(
						errorMessage);
				case HttpStatusCode.InternalServerError:
					throw new InternalServerErrorException(
						errorMessage, response.Headers.RetryAfter);
				default:
					throw new SignhostRestApiClientException(
						errorMessage);
			}

			System.Diagnostics.Debug.Fail("Should not be reached");
		}
	}
}
