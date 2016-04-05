using System;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	public class InternalServerErrorException
		: Exception
	{
		public InternalServerErrorException()
			: base()
		{
		}

		public InternalServerErrorException(string message)
			: base(message)
		{
		}

		public InternalServerErrorException(
			string message, Exception innerException, RetryConditionHeaderValue retryAfter)
			: base(message, innerException)
		{
			HelpLink = "https://api.signhost.com/Help";
			Data.Add("Retry After", retryAfter);
		}

		protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
