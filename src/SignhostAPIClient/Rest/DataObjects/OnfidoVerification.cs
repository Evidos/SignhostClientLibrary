using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Onfido identity verification.
/// </summary>
public class OnfidoVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the Onfido workflow identifier.
	/// </summary>
	public Guid? WorkflowId { get; set; }

	/// <summary>
	/// Gets or sets the Onfido workflow run identifier.
	/// </summary>
	public Guid? WorkflowRunId { get; set; }

	/// <summary>
	/// Gets or sets the Onfido API version.
	/// </summary>
	public int? Version { get; set; }

	/// <summary>
	/// Gets or sets raw Onfido attributes (availability not guaranteed).
	/// </summary>
	public Dictionary<string, object>? Attributes { get; set; }
}
