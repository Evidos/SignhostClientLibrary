using System;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	/// <summary>
	/// Thrown when a transaction is deleted / cancelled.
	/// </summary>
	[Serializable]
	public class GoneException<TResult>
		: SignhostRestApiClientException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GoneException"/> class.
		/// </summary>
		public GoneException()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GoneException"/> class.
		/// </summary>
		/// <param name="message">Additional information</param>
		public GoneException(string message)
			: base(message)
		{
		}

		public GoneException(string message, TResult result)
			: base(message)
		{
			Result = result;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GoneException"/> class.
		/// </summary>
		/// <param name="message">Additional information</param>
		/// <param name="innerException">Inner exception</param>
		public GoneException(string message, Exception innerException)
			: base(message, innerException)
		{
			HelpLink = "https://api.signhost.com/Help";
		}

#if SERIALIZABLE
		protected GoneException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
#endif

		/// <summary>
		/// Gets the api / transaction details which are still available.
		/// Please note that this no longer contains the full transaction
		/// details as most data is removed.
		/// </summary>
		public TResult Result { get; private set; }
	}
}
