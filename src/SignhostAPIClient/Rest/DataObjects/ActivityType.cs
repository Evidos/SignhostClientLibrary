namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// <see cref="Activity"/> type codes as defined in the Signhost API.
/// </summary>
public enum ActivityType
{
	/// <summary>
	/// Invitation sent (code 101).
	/// </summary>
	InvitationSent = 101,

	/// <summary>
	/// Sign URL was opened (code 103).
	/// </summary>
	Opened = 103,

	/// <summary>
	/// Invitation reminder sent (code 104).
	/// </summary>
	InvitationReminderResent = 104,

	/// <summary>
	/// Document was opened.
	/// The <see cref="Activity.Info"/> property contains the file ID of
	/// the opened document (code 105).
	/// </summary>
	DocumentOpened = 105,

	/// <summary>
	/// The signer rejected the sign request (code 202).
	/// </summary>
	Rejected = 202,

	/// <summary>
	/// The signer signed the documents (code 203).
	/// </summary>
	Signed = 203,

	/// <summary>
	/// The signer delegated signing to a different signer (code 204).
	/// </summary>
	SignerDelegated = 204,

	/// <summary>
	/// Signed document sent (code 301).
	/// </summary>
	SignedDocumentSent = 301,

	/// <summary>
	/// Signed document opened (code 302).
	/// </summary>
	SignedDocumentOpened = 302,

	/// <summary>
	/// Signed document downloaded (code 303).
	/// </summary>
	SignedDocumentDownloaded = 303,

	/// <summary>
	/// Receipt sent (code 401).
	/// </summary>
	ReceiptSent = 401,

	/// <summary>
	/// Receipt opened (code 402).
	/// </summary>
	ReceiptOpened = 402,

	/// <summary>
	/// Receipt downloaded (code 403).
	/// </summary>
	ReceiptDownloaded = 403,

	/// <summary>
	/// Transaction failed due to this entity, e.g. email bounce (code 999).
	/// </summary>
	Failed = 999,
}
