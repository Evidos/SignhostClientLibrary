using Newtonsoft.Json;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects
{
	[JsonConverter(typeof(JsonVerificationConverter))]
	public interface IVerification
	{
		string Type { get; }
	}
}
