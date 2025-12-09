using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects;

public class Receiver
{
	public string? Id { get; set; }

	public string? Name { get; set; }

	public string? Email { get; set; }

	public string? Language { get; set; }

	public string? Subject { get; set; }

	public string? Message { get; set; }

	public string? Reference { get; set; }

	public DateTimeOffset? CreatedDateTime { get; set; }

	public DateTimeOffset? ModifiedDateTime { get; set; }

	public IList<Activity>? Activities { get; set; }

	public dynamic? Context { get; set; }
}
