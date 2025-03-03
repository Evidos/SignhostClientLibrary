using System;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	/// <summary>
	/// An exception which indicates payment is required when an API action is called.
	/// </summary>
	[Serializable]
	public class RateLimitException
		: SignhostRestApiClientException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RateLimitException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		public RateLimitException(string message)
			: base(message)
		{
		}
	}
}
