using Newtonsoft.Json;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Extension methods around JSON Deserialization.
	/// </summary>
	internal static class JsonStringExtensions
	{
		/// <summary>
		/// Tries to desrialize it into a value of <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T">Type to try to deserialize to.</typeparam>
		/// <param name="json">The input string to try to desrialize to.</param>
		/// <param name="result">
		/// A deserialized value of <see cref="T"/>
		/// or default(T) if the deserializing was unsuccessfull.
		/// </param>
		/// <returns>A Boolean indicating if the desrializing was succesfull.</returns>
		internal static bool TryParseAsJson<T>(
			this string json,
			out T result)
		{
			if (json is null) {
				throw new System.ArgumentNullException(nameof(json));
			}

			try {
				result = JsonConvert.DeserializeObject<T>(json);
				return true;
			}
			catch (JsonException) {
				// When a JsonException occured this is most likely because the input
				// wasn't valid JSON. So we can't deserialize and thus can discard the exception.
				result = default;
				return false;
			}
		}
	}
}
