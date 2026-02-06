using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

public class Signer
{
	public string? Id { get; set; }

	public DateTimeOffset? Expires { get; set; }

	public string Email { get; set; } = default!;

	public string? IntroText { get; set; }

	public string? SignRequestSubject { get; set; }

	public string? SignRequestMessage { get; set; }

	public IList<IVerification> Authentications { get; set; } = default!;

	public IList<IVerification> Verifications { get; set; } = default!;

	public bool SendSignRequest { get; set; }

	public bool? SendSignConfirmation { get; set; }

	public int? DaysToRemind { get; set; }

	public string? Language { get; set; }

	public string? Reference { get; set; }

	public string? ReturnUrl { get; set; }

	public string? RejectReason { get; set; }

	public string SignUrl { get; set; } = default!;

	public bool AllowDelegation { get; set; }

	public string? DelegateSignUrl { get; set; }

	public string? DelegateReason { get; set; }

	public string? DelegateSignerEmail { get; set; }

	public string? DelegateSignerName { get; set; }

	public DateTimeOffset? SignedDateTime { get; set; }

	public DateTimeOffset? RejectDateTime { get; set; }

	public DateTimeOffset? CreatedDateTime { get; set; }

	public DateTimeOffset? SignerDelegationDateTime { get; set; }

	public DateTimeOffset? ModifiedDateTime { get; set; }

	public string ShowUrl { get; set; } = default!;

	public string ReceiptUrl { get; set; } = default!;

	public IList<Activity> Activities { get; set; } = new List<Activity>();

	public dynamic? Context { get; set; }
}
