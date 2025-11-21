using System;

namespace Signhost.APIClient.Rest.ErrorHandling;

[Serializable]
public class NotFoundException
	: SignhostRestApiClientException
{
	public NotFoundException()
		 : base()
	{
	}

	public NotFoundException(string message)
		: base(message)
	{
	}

	public NotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
		HelpLink = "https://api.signhost.com/Help";
	}

#if SERIALIZABLE
	protected NotFoundException(
		SerializationInfo info,
		StreamingContext context)
		: base(info, context)
	{
	}
#endif
}
