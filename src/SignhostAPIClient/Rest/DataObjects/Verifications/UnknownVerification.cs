using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects;

public class UnknownVerification
	: IVerification
{
	public string Type { get; set; } = default!;

	[JsonExtensionData]
	public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}
