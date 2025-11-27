using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Request object for creating a new transaction.
/// Transaction creation data including signers, receivers, files, and configuration.
/// </summary>
public class CreateTransactionRequest
{
	/// <summary>
	/// Gets or sets a value indicating whether to seal the transaction (no signers required).
	/// When true, the transaction is automatically completed without requiring signatures.
	/// </summary>
	public bool Seal { get; set; }

	/// <summary>
	/// Gets or sets the custom reference identifier for the transaction.
	/// </summary>
	public string? Reference { get; set; }

	/// <summary>
	/// Gets or sets the URL to receive status notifications about the transaction.
	/// </summary>
	public string? PostbackUrl { get; set; }

	/// <summary>
	/// Gets or sets the number of days until the transaction expires.
	/// If 0, uses organization default. Maximum value depends on organization settings.
	/// </summary>
	public int DaysToExpire { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to send email notifications to signers and receivers.
	/// </summary>
	public bool SendEmailNotifications { get; set; }

	/// <summary>
	/// Gets or sets the mode for sign request delivery.
	/// 0: No sign requests, 1: Send immediately, 2: Send when ready (default when signers have SendSignRequest enabled).
	/// </summary>
	public int SignRequestMode { get; set; }

	/// <summary>
	/// Gets or sets the language code for transaction interface and emails.
	/// Supported values: de-DE, en-US, es-ES, fr-FR, it-IT, pl-PL, nl-NL.
	/// </summary>
	public string? Language { get; set; }

	/// <summary>
	/// Gets or sets the custom JSON object for additional transaction data.
	/// Only JSON objects are allowed (no arrays or primitives).
	/// </summary>
	public dynamic? Context { get; set; }

	/// <summary>
	/// Gets or sets the list of signers for the transaction.
	/// </summary>
	public IList<CreateSignerRequest> Signers { get; set; } =
		new List<CreateSignerRequest>();

	/// <summary>
	/// Gets or sets the list of receivers who get copies of completed documents.
	/// </summary>
	public IList<CreateReceiverRequest> Receivers { get; set; } =
		new List<CreateReceiverRequest>();
}
