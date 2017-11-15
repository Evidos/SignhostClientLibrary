using System;
using System.Collections.Generic;
using System.Text;

namespace Signhost.APIClient.Rest.DataObjects
{
	public enum TransactionStatus
	{
		/// <summary>
		/// Transaction has not yet been started and is waiting for its
		/// documents.
		/// </summary>
		WaitingForDocument = 5,

		/// <summary>
		/// The transaction was started and is waiting for one or more signers
		/// to sign the transaction.
		/// </summary>
		WaitingForSigner = 10,

		/// <summary>
		/// In progress
		/// </summary>
		InProgress = 20,

		/// <summary>
		/// The transaction was succesfully completed.
		/// </summary>
		Signed = 30,

		/// <summary>
		/// The transaction was rejected by one or more of the signers.
		/// </summary>
		Rejected = 40,

		/// <summary>
		/// The transaction was not signed before it expired.
		/// </summary>
		Expired = 50,

		/// <summary>
		/// The transaction was cancelled by the sender.
		/// </summary>
		Cancelled = 60,

		/// <summary>
		/// The transaction could not be completed.
		/// </summary>
		Failed = 70
	}
}
