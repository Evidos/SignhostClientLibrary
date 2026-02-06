using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest;

/// <summary>
/// Interface abstracting the available Signhost API responses.
/// </summary>
public interface ISignhostApiReceiver
{
	/// <summary>
	/// Checks the validity of the postback checksum.
	/// </summary>
	/// <returns><c>true</c>, if postback checksum valid was validated, <c>false</c> otherwise.</returns>
	/// <param name="headers">HTTP response headers.</param>
	/// <param name="body">HTTP response body.</param>
	/// <param name="postbackTransaction">A transaction object.</param>
	bool IsPostbackChecksumValid(
		IDictionary<string, string[]> headers,
		string body,
		[NotNullWhen(true)] out Transaction? postbackTransaction);
}
