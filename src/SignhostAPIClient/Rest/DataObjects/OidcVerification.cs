namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// OpenID Connect identification.
/// </summary>
public class OidcVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the OIDC provider name.
	/// </summary>
	public string ProviderName { get; set; }
}
