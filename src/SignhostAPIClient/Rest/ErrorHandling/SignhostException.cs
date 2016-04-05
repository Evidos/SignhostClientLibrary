using System;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	public class SignhostException
		: SignhostRestApiClientException
	{
		public SignhostException()
			 : base()
		{
		}

		public SignhostException(string message)
			: base(message)
		{
		}

		public SignhostException(string message, Exception innerException)
			: base(message, innerException)
		{
			HelpLink = "https://api.signhost.com/Help";
		}

		protected SignhostException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
