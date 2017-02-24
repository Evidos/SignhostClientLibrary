using System;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Idin
		: IVerification
	{
		public string Type { get; } = "iDIN";

		public string AccountHolderName { get; set; }

		public string AccountHolderAddress1 { get; set; }

		public string AccountHolderAddress2 { get; set; }

		public DateTime AccountHolderDateOfBirth { get; set; }
	}
}
