using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Centralized JSON serialization options for Signhost API.
	/// </summary>
	public static class SignhostJsonSerializerOptions
	{
		/// <summary>
		/// Gets the default JSON serializer options.
		/// </summary>
		public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			Converters = {
				new JsonStringEnumConverter(),
			},
		};
	}
}
