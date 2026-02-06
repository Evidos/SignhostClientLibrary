using System;

namespace Signhost.APIClient.Rest.ErrorHandling;

/// <summary>
/// An exception which indicates payment is required when an API action is called.
/// </summary>
[Serializable]
public class OutOfCreditsException
	: SignhostRestApiClientException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OutOfCreditsException"/> class.
	/// </summary>
	/// <param name="message">The exception message.</param>
	public OutOfCreditsException(string message)
		: base(message)
	{
	}
}
