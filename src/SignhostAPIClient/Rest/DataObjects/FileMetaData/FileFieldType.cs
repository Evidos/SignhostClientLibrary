using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileFieldType
{
	Seal,
	Signature,
	Check,
	Radio,
	SingleLine,
	Number,
	Date,
}
