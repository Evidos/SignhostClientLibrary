using System.Text.Json.Serialization;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects;

public class Field
{
	public FileFieldType Type { get; set; }

	/// <summary>
	/// The value content for the field. Can be a string, number, or boolean.
	/// </summary>
	[JsonConverter(typeof(JsonObjectConverter))]
	public object? Value { get; set; }

	public Location Location { get; set; } = default!;
}
