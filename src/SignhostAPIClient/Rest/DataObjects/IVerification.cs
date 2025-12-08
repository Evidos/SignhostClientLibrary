using Newtonsoft.Json;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects
{
	// TO-DO: Split to verification and authentication in v5
	[JsonConverter(typeof(JsonVerificationConverter))]
	public interface IVerification
	{
		string Type { get; }
	}
}
