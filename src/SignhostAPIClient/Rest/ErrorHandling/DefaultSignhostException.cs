using System;
using System.Runtime.Serialization;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	[Serializable]
	public class DefaultSignhostException : Exception
	{
		public DefaultSignhostException(string message)
			: base(message)
		{
			HelpLink = "https://api.signhost.com/Help";
		}

		public DefaultSignhostException(
			string message,
			Exception innerException)
			: base(message, innerException)
		{
		}

#if SERIALIZABLE
		protected DefaultSignhostException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}
