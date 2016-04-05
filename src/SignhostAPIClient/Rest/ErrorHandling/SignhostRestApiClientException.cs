using System;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	public class SignhostRestApiClientException
		: Exception
	{
		public SignhostRestApiClientException()
			 : base()
		{
		}

		public SignhostRestApiClientException(string message)
			: base(message)
		{
		}

		public SignhostRestApiClientException(string message, Exception innerException)
			: base(message, innerException)
		{
			HelpLink = "https://api.signhost.com/Help";
		}

		protected SignhostRestApiClientException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
