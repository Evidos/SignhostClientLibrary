using System.Text.Json.Serialization;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects;

[JsonConverter(typeof(VerificationConverter))]
public interface IVerification
{
}
