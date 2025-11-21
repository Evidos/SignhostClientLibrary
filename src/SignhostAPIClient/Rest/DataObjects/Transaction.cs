using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Transaction
	{
		public string Id { get; set; }

		/// <summary>
		/// Gets the <see cref="DateTimeOffset"/> when the <see cref="Transaction"/> was created.
		/// </summary>
		public DateTimeOffset? CreatedDateTime { get; set; }

		/// <summary>
		/// Gets the <see cref="DateTimeOffset"/> when the <see cref="Transaction"/> was cancelled.
		/// Returns null if the transaction was not cancelled.
		/// </summary>
		public DateTimeOffset? CanceledDateTime { get; set; }

		/// <summary>
		/// Gets the cancellation reason when the <see cref="Transaction" /> was cancelled.
		/// </summary>
		public string CancellationReason { get; set; }

		public IReadOnlyDictionary<string, FileEntry> Files { get; set; } =
			new Dictionary<string, FileEntry>();

		public TransactionStatus Status { get; set; }

		public bool Seal { get; set; }

		public IList<Signer> Signers { get; set; }
			= new List<Signer>();

		public IList<Receiver> Receivers { get; set; }
			= new List<Receiver>();

		public string Reference { get; set; }

		public string PostbackUrl { get; set; }

		public int SignRequestMode { get; set; }

		public int DaysToExpire { get; set; }

		public string Language { get; set; }

		public bool SendEmailNotifications { get; set; }

		public dynamic Context { get; set; }
	}
}
