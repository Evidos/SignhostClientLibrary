using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Extension methods around JSON Deserialization.
	/// </summary>
	internal static class HttpContentJsonExtensions
	{
		/// <summary>
		/// Reads the JSON content and returns the deserialized value.
		/// </summary>
		/// <typeparam name="T">Type to deserialize to</typeparam>
		/// <param name="httpContent"><see cref="HttpContent"/> to read.</param>
		/// <returns>A deserialized value of <see cref="T"/>
		/// or default(T) if no content is available.</returns>
		internal static async Task<T> FromJsonAsync<T>(this HttpContent httpContent)
		{
			if (httpContent == null) {
				return default(T);
			}

			var json = await httpContent.ReadAsStringAsync()
				.ConfigureAwait(false);
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
