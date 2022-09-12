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
		protected Transaction(
			IReadOnlyDictionary<string, FileEntry> files,
			DateTimeOffset? createdDateTime,
			DateTimeOffset? canceledDateTime,
			string cancelationReason)
		{
			Files = files ?? new Dictionary<string, FileEntry>();
			CreatedDateTime = createdDateTime;
			CancelledDateTime = canceledDateTime;
			CancellationReason = cancelationReason;
		}

		public string Id { get; set; }

		/// <summary>
		/// Gets the <see cref="DateTimeOffset"/> when the <see cref="Transaction"/> was created.
		/// </summary>
		public DateTimeOffset? CreatedDateTime { get; }

		/// <summary>
		/// Gets the <see cref="DateTimeOffset"/> when the <see cref="Transaction"/> was cancelled.
		/// Returns null if the transaction was not cancelled.
		/// </summary>
		public DateTimeOffset? CancelledDateTime { get; }

		/// <summary>
		/// Gets the cancellation reason when the <see cref="Transaction" /> was cancelled.
		/// </summary>
		public string CancellationReason { get; }

		public IReadOnlyDictionary<string, FileEntry> Files { get; private set; }

		public TransactionStatus Status { get; set; }

		public bool Seal { get; set; }

		public IList<Signer> Signers { get; set; } = new List<Signer>();

		public IList<Receiver> Receivers { get; set; } = new List<Receiver>();

		public string Reference { get; set; }

		public string PostbackUrl { get; set; }

		public int SignRequestMode { get; set; }

		public int DaysToExpire { get; set; }

		public string Language { get; set; }

		public bool SendEmailNotifications { get; set; }

		/// <summary>
		/// Gets or sets the teamId of the Team this Transaction will be
		/// assigned to.
		/// </summary>
		/// <value>a string representing the id of the Team.</value>
		/// <remarks>
		/// When set to null, no Team will be assigned and therefore only the
		/// user that cerated the Transaction can manage it (i.e. GET and PUT
		/// files). When set to a teamId of a Team, all members of that Team
		/// can then manage the Transaction.
		/// </remarks>
		public string TeamId { get; set; }

		public dynamic Context { get; set; }
	}
}
