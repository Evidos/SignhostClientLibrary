using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects;

public class Signer
{
	public Signer()
	{
	}

	[JsonConstructor]
	private Signer(IReadOnlyList<Activity> activities)
	{
		Activities = activities;
	}

	public string Id { get; set; }

	public DateTimeOffset? Expires { get; set; }

	public string Email { get; set; }

	public string IntroText { get; set; }

	public string SignRequestSubject { get; set; }

	public string SignRequestMessage { get; set; }

	public IList<IVerification> Authentications { get; set; }
		= new List<IVerification>();

	public IList<IVerification> Verifications { get; set; }
		= new List<IVerification>();

	public bool SendSignRequest { get; set; }

	public bool? SendSignConfirmation { get; set; }

	public int? DaysToRemind { get; set; }

	public string Language { get; set; }

	public string ScribbleName { get; set; }

	public bool ScribbleNameFixed { get; set; }

	public string Reference { get; set; }

	public string ReturnUrl { get; set; }

	public string RejectReason { get; set; }

	public string SignUrl { get; set; }

	public bool AllowDelegation { get; set; }

	public string DelegateSignUrl { get; set; }

	public string DelegateReason { get; set; }

	public string DelegateSignerEmail { get; set; }

	public string DelegateSignerName { get; set; }

	public IReadOnlyList<Activity> Activities { get; private set; } =
		new List<Activity>().AsReadOnly();

	public dynamic Context { get; set; }
}
