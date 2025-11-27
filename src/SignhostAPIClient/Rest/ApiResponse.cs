using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

	public async Task EnsureAvailableStatusCodeAsync(
		CancellationToken cancellationToken = default)
	{
		if (HttpStatusCode == HttpStatusCode.Gone) {
			throw new ErrorHandling.GoneException<TValue>(
				httpResponse.ReasonPhrase ?? "No reason phrase provided",
				Value)
			{
				ResponseBody = await httpResponse.Content
#if NETFRAMEWORK || NETSTANDARD2_0
					.ReadAsStringAsync()
#else
					.ReadAsStringAsync(cancellationToken)
#endif
					.ConfigureAwait(false),
			};
		}
	}
}
