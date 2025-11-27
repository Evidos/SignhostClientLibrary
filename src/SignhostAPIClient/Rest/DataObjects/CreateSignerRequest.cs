using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Request object for creating a new signer in a transaction. Defines the signer's identity,
/// authentication/verification requirements, notification preferences, and signing behavior.
/// </summary>
/// <remarks>
/// <para><strong>Key Requirements:</strong></para>
/// <list type="bullet">
/// <item><description>Email address is mandatory</description></item>
/// <item><description>Either Authentications or Verifications must be provided (or both)</description></item>
/// <item><description>Signers with AllowDelegation enabled cannot have Authentications</description></item>
/// <item><description>SendSignRequest determines if sign request emails are sent automatically</description></item>
/// </list>
/// </remarks>
public class CreateSignerRequest
{
	/// <summary>
	/// Gets or sets the signer identifier (will be generated if not provided).
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// Gets or sets when the signer's access expires.
	/// </summary>
	public DateTimeOffset? Expires { get; set; }

	/// <summary>
	/// Gets or sets the signer's email address (required).
	/// </summary>
	public string Email { get; set; } = default!;

	/// <summary>
	/// Gets or sets the list of authentication methods that the signer has to authenticate with.
	/// The order in which the authentications are provided determine in which order the signer will have to perform the specified method.
	/// Authentications must be performed before the document(s) can be viewed.
	/// </summary>
	public IList<IVerification>? Authentications { get; set; }

	/// <summary>
	/// Gets or sets the list of verification methods that the signer has to verify with.
	/// The order in which the verifications are provided determine in which order the signer will have to perform the specified method.
	/// Verifications must be performed before the document(s) can be signed.
	/// </summary>
	/// <remarks>
	/// <para><strong>Critical Requirement:</strong> You <strong>must</strong> use one of the following verification methods as the <strong>last</strong> verification in the list:</para>
	/// <list type="bullet">
	/// <item><description>Consent</description></item>
	/// <item><description>PhoneNumber</description></item>
	/// <item><description>Scribble</description></item>
	/// <item><description>CSC Qualified*</description></item>
	/// </list>
	/// <para><strong>Important Notes:</strong></para>
	/// <list type="bullet">
	/// <item><description>CSC Qualified <strong>must always be the final verification</strong> if used</description></item>
	/// <item><description>The other three methods (Consent, PhoneNumber, Scribble) can be succeeded by other verification methods</description></item>
	/// </list>
	/// </remarks>
	public IList<IVerification>? Verifications { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to send sign request to this signer's email address.
	/// Default is true.
	/// </summary>
	public bool SendSignRequest { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to send a confirmation email to the signer after signing.
	/// Default value is the value of <see cref="SendSignRequest"/>.
	/// </summary>
	public bool? SendSignConfirmation { get; set; }

	/// <summary>
	/// Gets or sets the subject of the sign request email in plain text.
	/// Maximum of 64 characters allowed. If omitted, the default subject will be used.
	/// </summary>
	public string? SignRequestSubject { get; set; }

	/// <summary>
	/// Gets or sets the message of the sign request email in plain text. HTML is not allowed.
	/// Newlines can be created by including a \n.
	/// Required if <see cref="SendSignRequest"/> is true.
	/// </summary>
	public string? SignRequestMessage { get; set; }

	/// <summary>
	/// Gets or sets the number of days between automatic reminder emails sent to this signer.
	/// Set to -1 to disable reminders entirely for this signer.
	/// Set to 0 to use your organization's default reminder interval.
	/// Set to a positive number (e.g., 3, 7) to send reminders every N days.
	/// Default is 7.
	/// </summary>
	/// <remarks>
	/// Reminders are only sent if <see cref="SendSignRequest"/> is true and the signer hasn't completed signing yet.
	/// </remarks>
	public int? DaysToRemind { get; set; }

	/// <summary>
	/// Gets or sets the language for signer interface and emails.
	/// Supported values: de-DE, en-US, es-ES, fr-FR, it-IT, pl-PL, nl-NL.
	/// Default is nl-NL.
	/// </summary>
	public string? Language { get; set; }

	/// <summary>
	/// Gets or sets the custom reference for this signer.
	/// </summary>
	public string? Reference { get; set; }

	/// <summary>
	/// Gets or sets the custom introduction text shown to the signer during the signing process.
	/// This will be shown on the first screen to the signer and supports limited markdown markup.
	/// </summary>
	/// <remarks>
	/// <para>The following markup is supported:</para>
	/// <list type="bullet">
	/// <item><description># Headings</description></item>
	/// <item><description>*Emphasis* / _Emphasis_</description></item>
	/// <item><description>**Strong** / __Strong__</description></item>
	/// <item><description>1. Ordered and - Unordered lists</description></item>
	/// </list>
	/// </remarks>
	public string? IntroText { get; set; }

	/// <summary>
	/// Gets or sets the URL to redirect signer after signing.
	/// Default is https://signhost.com.
	/// </summary>
	public string? ReturnUrl { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this signer can delegate signing to another person.
	/// Cannot be used together with <see cref="Authentications"/>.
	/// Default is false.
	/// </summary>
	public bool AllowDelegation { get; set; }

	/// <summary>
	/// Gets or sets the custom signer data (dynamic JSON object).
	/// </summary>
	public dynamic? Context { get; set; }
}
