namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// <see cref="Activity"/> type.
	/// </summary>
	// TO-DO: Remove unused activity types in v5.
	public enum ActivityType
	{
		/// <summary>
		/// The invitation mail was sent.
		/// </summary>
		InvitationSent = 101,

		/// <summary>
		/// The invitation mail was received.
		/// </summary>
		InvitationReceived = 102,

		/// <summary>
		/// The sign url was opened.
		/// </summary>
		Opened = 103,

		/// <summary>
		/// An invitation reminder mail was sent.
		/// </summary>
		InvitationReminderResent = 104,

		/// <summary>
		/// The document was opened.
		/// The <see cref="Activity.Info"/> contains the fileId of the opened
		/// document.
		/// </summary>
		DocumentOpened = 105,

		/// <summary>
		/// Consumer Signing identity approved.
		/// </summary>
		IdentityApproved = 110,

		/// <summary>
		/// Consumer Signing identity failed.
		/// </summary>
		IdentityFailed = 111,

		/// <summary>
		/// Cancelled.
		/// </summary>
		Cancelled = 201,

		/// <summary>
		/// The signer rejected the sign request.
		/// </summary>
		Rejected = 202,

		/// <summary>
		/// The signer signed the documents.
		/// </summary>
		Signed = 203,

		/// <summary>
		/// The signer delegated signing to a different signer.
		/// </summary>
		SignerDelegated = 204,

		/// <summary>
		/// Signed document sent.
		/// </summary>
		SignedDocumentSent = 301,

		/// <summary>
		/// Signed document opened.
		/// </summary>
		SignedDocumentOpened = 302,

		/// <summary>
		/// Signed document downloaded.
		/// </summary>
		SignedDocumentDownloaded = 303,

		/// <summary>
		/// Receipt sent.
		/// </summary>
		ReceiptSent = 401,

		/// <summary>
		/// Receipt opened.
		/// </summary>
		ReceiptOpened = 402,

		/// <summary>
		/// Receipt downloaded.
		/// </summary>
		ReceiptDownloaded = 403,

		/// <summary>
		/// Finished.
		/// </summary>
		Finished = 500,

		/// <summary>
		/// Deleted.
		/// </summary>
		Deleted = 600,

		/// <summary>
		/// Expired.
		/// </summary>
		Expired = 700,

		/// <summary>
		/// Email bounce - hard.
		/// </summary>
		EmailBounceHard = 901,

		/// <summary>
		/// Email bounce - soft.
		/// </summary>
		EmailBounceSoft = 902,

		/// <summary>
		/// Email bounce - blocked.
		/// </summary>
		EmailBounceBlocked = 903,

		/// <summary>
		/// Email bounce - undetermined.
		/// </summary>
		EmailBounceUndetermined = 904,

		/// <summary>
		/// Operation failed.
		/// </summary>
		Failed = 999,
	}
}
