using System.Net;
using System.Net.Http;

namespace Signhost.APIClient.Rest;

public class ApiResponse<TValue>
{
	private readonly HttpResponseMessage httpResponse;

	public ApiResponse(HttpResponseMessage httpResponse, TValue? value)
	{
		this.httpResponse = httpResponse;
		this.Value = value;
	}

	public TValue? Value { get; private set; }

	public HttpStatusCode HttpStatusCode => httpResponse.StatusCode;

	public void EnsureAvailableStatusCode()
	{
		if (HttpStatusCode == HttpStatusCode.Gone) {
			throw new ErrorHandling.GoneException<TValue>(
				httpResponse.ReasonPhrase ?? "No reason phrase provided",
				Value)
			{
				// TO-DO: Make async in v5
				ResponseBody = httpResponse.Content.ReadAsStringAsync()
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult(),
			};
		}
	}
}
