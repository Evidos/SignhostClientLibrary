using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Signhost.APIClient.Rest;

/// <summary>
/// See the <see cref="JsonContent{T}" /> helper class.
/// </summary>
internal static class JsonContent
{
	/// <summary>
	/// Creates a new <see cref="JsonContent{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type to serialize.</typeparam>
	/// <param name="value">Value to serialize.</param>
	/// <returns><see cref="HttpContent"/>.</returns>
	internal static JsonContent<T> From<T>(T value)
	{
		return new JsonContent<T>(value);
	}
}

/// <summary>
/// A <see cref="HttpContent"/> class for application/json.
/// </summary>
/// <typeparam name="T">The type to serialize</typeparam>
internal class JsonContent<T>
	: StringContent
{
	/// <summary>
	/// Initializes a new instance of the <see cref="JsonContent{T}"/> class.
	/// </summary>
	/// <param name="value">Value to serialize.</param>
	public JsonContent(T value)
		: base(ToJson(value))
	{
		Headers.ContentType = new MediaTypeHeaderValue("application/json");
	}

	private static string ToJson(T value)
	{
		return JsonSerializer.Serialize(value, SignhostJsonSerializerOptions.Default);
	}
}
