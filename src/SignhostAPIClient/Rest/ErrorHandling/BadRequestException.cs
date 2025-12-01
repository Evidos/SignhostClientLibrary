using System;

namespace Signhost.APIClient.Rest.ErrorHandling;

[Serializable]
public class BadRequestException
	: SignhostRestApiClientException
{
	public BadRequestException()
		: base()
	{
	}

	public BadRequestException(string message)
		: base(message)
	{
	}

	public BadRequestException(
		string message,
		Exception innerException)
		: base(message, innerException)
	{
		HelpLink = "https://api.signhost.com/Help";
	}

#if SERIALIZABLE
	protected BadRequestException(
		SerializationInfo info,
		StreamingContext context)
		: base(info, context)
	{
	}
#endif
}
