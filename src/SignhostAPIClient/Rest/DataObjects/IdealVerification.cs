namespace Signhost.APIClient.Rest.DataObjects
{
	public class IdealVerification
		: IVerification
	{
		public string Iban { get; set; }

		public string AccountHolderName { get; set; }

		public string AccountHolderCity { get; set; }
	}
}
