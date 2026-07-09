using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest.ErrorHandling;

/// <summary>
/// Error handling around <see cref="HttpResponseMessage"/>s.
/// </summary>
public static class HttpResponseMessageErrorHandlingExtensions
{
	private const string OutOfCreditsApiProblemType =
		"https://api.signhost.com/problem/subscription/out-of-credits";

	private const string DefaultErrorMessage = "Unknown Signhost error";
	private const string DefaultErrorType = "Unknown Signhost Error type";

	private static readonly JsonSerializerOptions Options = new () {
		PropertyNameCaseInsensitive = true,
	};

	/// <summary>
	/// Throws an exception if the <see cref="HttpResponseMessage.StatusCode"/>
	/// has an error code.
	/// </summary>
	/// <param name="responseTask"><see cref="HttpResponseMessage"/></param>
	/// <returns>Returns <see cref="HttpResponseMessage"/> if the call is succesful.</returns>
	/// <param name="expectedStatusCodes">List of <see cref="HttpStatusCode"/> which should
	/// not be handled as an error.</param>
	/// <exception cref="BadAuthorizationException">
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

		string errorType = DefaultErrorType;
		string errorMessage = DefaultErrorMessage;
		string responseBody = string.Empty;

		if (response.Content is not null) {
			responseBody = await response.Content.ReadAsStringAsync()
				.ConfigureAwait(false);

			var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody, Options);

			errorType = error?.Type ?? DefaultErrorType;
			errorMessage = error?.Message
				?? error?.Detail
				?? DefaultErrorMessage;
		}

		SignhostRestApiClientException exception = response.StatusCode switch {
			HttpStatusCode.Unauthorized => new BadAuthorizationException(errorMessage),
			HttpStatusCode.BadRequest => new BadRequestException(errorMessage),
			HttpStatusCode.NotFound => new NotFoundException(errorMessage),

			HttpStatusCode.InternalServerError =>
				new InternalServerErrorException(errorMessage, response.Headers.RetryAfter),

			HttpStatusCode.PaymentRequired
			when errorType == OutOfCreditsApiProblemType =>
				new OutOfCreditsException(errorMessage),

			_ => new SignhostRestApiClientException(errorMessage),
		};

		exception.ResponseBody = responseBody;

		throw exception;
	}

	private class ErrorResponse
	{
		/// <summary>
		/// The problem type URI (RFC 7807). Used to identify specific error
		/// categories such as out-of-credits.
		/// </summary>
		[JsonPropertyName("type")]
		public string? Type { get; set; }

		/// <summary>
		/// Human-readable error message returned by the Signhost API
		/// in its legacy error format, e.g. <c>{"Message":"..."}</c>.
		/// </summary>
		[JsonPropertyName("message")]
		public string? Message { get; set; }

		/// <summary>
		/// Human-readable explanation of the problem as defined by RFC 7807
		/// problem details, e.g. <c>{"detail":"..."}</c>. Used as a fallback
		/// when <see cref="Message"/> is absent.
		/// </summary>
		[JsonPropertyName("detail")]
		public string? Detail { get; set; }
	}
}
