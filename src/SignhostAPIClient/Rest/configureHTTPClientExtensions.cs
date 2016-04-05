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

		public static Task<HttpResponseMessage> PutStreamAsync(this Url url, Stream fileStream)
		{
			return new FlurlClient(url, false).PutStreamAsync(fileStream);
		}

		public static Task<HttpResponseMessage> PutStreamAsync(this FlurlClient client, Stream fileStream)
		{
			return client.SendAsync(HttpMethod.Put, new StreamContent(fileStream));
		}
	}

}
