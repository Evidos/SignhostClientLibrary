namespace Signhost.APIClient.Rest.DataObjects;

public class PhoneNumberVerification
	: IVerification
{
	public string Number { get; set; } = default!;

	public bool? SecureDownload { get; set; }
}
