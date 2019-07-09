using System;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	[Obsolete("Unused will be removed")]
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

#if SERIALIZABLE
		protected SignhostException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}
