using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Signhost.APIClient.Rest
{
	public static class ConfigureHttpClientExtensions
	{
		public static FlurlClient WithSignhostAuth(
			this Url client,
			string applicationKey,
			string apiKey)
		{
			return client.WithHeaders(new
			{
				Application = $"APPKey {applicationKey}",
				Authorization = $"APIKey {apiKey}"
			});
		}

		public static Task<HttpResponseMessage> PutStreamAsync(this Url url, Stream fileStream, string contentType)
		{
			return new FlurlClient(url, false).PutStreamAsync(fileStream, contentType);
		}

		public static Task<HttpResponseMessage> PutStreamAsync(this FlurlClient client, Stream fileStream, string contentType)
		{
			StreamContent content = new StreamContent(fileStream);
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
			return client.SendAsync(HttpMethod.Put, content);
		}
	}
}
