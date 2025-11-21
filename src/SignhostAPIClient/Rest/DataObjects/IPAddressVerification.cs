namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Adds a consent verification screen
/// </summary>
public class IPAddressVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the IP Address used by the signer while signing the documents.
	/// </summary>
	public string IPAddress { get; set; }
}
