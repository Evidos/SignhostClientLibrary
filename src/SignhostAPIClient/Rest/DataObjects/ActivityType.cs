using System;
using System.Collections.Generic;
using System.Text;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// <see cref="Activity"/> type.
	/// </summary>
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
		ReceiptDownloaded = 403
	}
}
