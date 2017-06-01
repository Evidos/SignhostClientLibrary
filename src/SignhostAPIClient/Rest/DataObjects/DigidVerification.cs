namespace Signhost.APIClient.Rest.DataObjects
{
	public class DigidVerification
		: IVerification
	{
		public string Type => "DigiD";

		public string Bsn { get; set; }
	}
}
