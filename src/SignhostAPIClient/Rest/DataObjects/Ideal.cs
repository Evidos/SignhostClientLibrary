namespace Signhost.APIClient.Rest.DataObjects
{
	public class Ideal
		: IVerification
	{
		public string Type { get; } = "iDeal";

		public string Iban { get; set; }

		public string AccountHolderName { get; set; }

		public string AccountHolderCity { get; set; }
	}
}
