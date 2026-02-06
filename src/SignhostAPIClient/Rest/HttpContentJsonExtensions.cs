using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest;

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
	internal static async Task<T?> FromJsonAsync<T>(this HttpContent httpContent)
	{
		if (httpContent is null) {
			return default;
		}

		string json = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
		return JsonSerializer.Deserialize<T>(json, SignhostJsonSerializerOptions.Default);
	}
}
