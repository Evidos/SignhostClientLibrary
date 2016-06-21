using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Transaction
	{
		public string Id { get; set; }

		public int Status { get; set; }

		public bool Seal { get; set; }

		public IList<Signer> Signers { get; set; } = new List<Signer>();

		public IList<Receiver> Receivers { get; set; } = new List<Receiver>();

		public string Reference { get; set; }

		public string PostbackUrl { get; set; }

		public int SignRequestMode { get; set; }

		public int DaysToExpire { get; set; }

		public bool SendEmailNotifications { get; set; }

		[Obsolete("Does not produce usable values")]
		public File File { get; set; }

		public dynamic Context { get; set; }
	}
}
