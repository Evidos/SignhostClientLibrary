using System;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	public class ForbiddenException
		: SignhostRestApiClientException
	{
		public ForbiddenException()
			: base()
		{
		}

		public ForbiddenException(string message)
			: base(message)
		{
		}

		public ForbiddenException(
			string message,
			Exception innerException)
			: base(message, innerException)
		{
		}

#if SERIALIZABLE
		protected ForbiddenException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}
