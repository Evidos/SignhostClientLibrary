namespace Signhost.APIClient.Rest.DataObjects
{
	public class PhoneNumberVerification
		: IVerification
	{
		public string Type => "PhoneNumber";

		public string Number { get; set; }

		public bool? SecureDownload { get; set; }
	}
}
