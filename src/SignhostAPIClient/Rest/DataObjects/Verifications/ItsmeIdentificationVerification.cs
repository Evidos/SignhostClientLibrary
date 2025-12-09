using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Verification object for itsme Identification.
/// </summary>
public class ItsmeIdentificationVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the phonenumber.
	/// </summary>
	public string PhoneNumber { get; set; } = default!;

	public IDictionary<string, object>? Attributes { get; set; }
}
