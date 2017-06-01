using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Transaction
	{
		public Transaction()
		{
		}

		[JsonConstructor]
		private Transaction(IReadOnlyDictionary<string, FileEntry> files)
		{
			Files = files;
		}

		public string Id { get; set; }

		public IReadOnlyDictionary<string, FileEntry> Files { get; private set; }

		public TransactionStatus Status { get; set; }

		public bool Seal { get; set; }

		public IList<Signer> Signers { get; set; } = new List<Signer>();

		public IList<Receiver> Receivers { get; set; } = new List<Receiver>();

		public string Reference { get; set; }

		public string PostbackUrl { get; set; }

		public int SignRequestMode { get; set; }

		public int DaysToExpire { get; set; }

		public bool SendEmailNotifications { get; set; }

		public dynamic Context { get; set; }
	}
}
