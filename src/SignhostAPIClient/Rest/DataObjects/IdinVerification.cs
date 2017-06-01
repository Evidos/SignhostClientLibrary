using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class IdinVerification
		: IVerification
	{
		public string Type { get; } = "iDIN";

		public string AccountHolderName { get; set; }

		public string AccountHolderAddress1 { get; set; }

		public string AccountHolderAddress2 { get; set; }

		public DateTime AccountHolderDateOfBirth { get; set; }

		public IDictionary<string, string> Attributes { get; set; }
	}
}
