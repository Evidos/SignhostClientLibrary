namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Request object for creating a receiver in a transaction.
/// Receiver configuration for getting copies of signed documents.
/// </summary>
public class CreateReceiverRequest
{
	/// <summary>
	/// Gets or sets the receiver's name.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets the receiver's email address (required).
	/// </summary>
	public string Email { get; set; } = default!;

	/// <summary>
	/// Gets or sets the language for receiver communications.
	/// Supported values: de-DE, en-US, es-ES, fr-FR, it-IT, pl-PL, nl-NL.
	/// Default is nl-NL.
	/// </summary>
	public string? Language { get; set; }

	/// <summary>
	/// Gets or sets the custom subject for notification email.
	/// Maximum of 64 characters allowed. Omitting this parameter will enable the default subject.
	/// </summary>
	public string? Subject { get; set; }

	/// <summary>
	/// Gets or sets the custom message for notification email (required).
	/// Newlines can be created by including a \n in the message. HTML is not allowed.
	/// </summary>
	public string Message { get; set; } = default!;

	/// <summary>
	/// Gets or sets the custom reference for this receiver.
	/// </summary>
	public string? Reference { get; set; }

	/// <summary>
	/// Gets or sets the custom receiver data (JSON object only).
	/// </summary>
	public dynamic? Context { get; set; }
}
