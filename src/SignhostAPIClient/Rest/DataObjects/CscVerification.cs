using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Cloud Signature Consortium (CSC) verification.
/// </summary>
public class CscVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the provider identifier.
	/// </summary>
	public string Provider { get; set; }

	/// <summary>
	/// Gets or sets the certificate issuer.
	/// </summary>
	public string Issuer { get; set; }

	/// <summary>
	/// Gets or sets the certificate subject.
	/// </summary>
	public string Subject { get; set; }

	/// <summary>
	/// Gets or sets the certificate thumbprint.
	/// </summary>
	public string Thumbprint { get; set; }

	/// <summary>
	/// Gets or sets additional user data.
	/// </summary>
	public Dictionary<string, string> AdditionalUserData { get; set; }
}
