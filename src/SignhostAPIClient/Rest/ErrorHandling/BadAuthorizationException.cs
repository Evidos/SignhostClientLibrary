using System;

namespace Signhost.APIClient.Rest.ErrorHandling;

// TO-DO: Use this instead of Unauthorized exception in v5
[Serializable]
public class BadAuthorizationException
	: SignhostRestApiClientException
{
	public BadAuthorizationException()
		 : base("API call returned a 401 error code. Please check your request headers.")
	{
		HelpLink = "https://api.signhost.com/Help";
	}

	public BadAuthorizationException(string message)
		: base(message)
	{
	}

	public BadAuthorizationException(
		string message,
		Exception innerException)
		: base(message, innerException)
	{
	}

#if SERIALIZABLE
	protected BadAuthorizationException(
		SerializationInfo info,
		StreamingContext context)
		: base(info, context)
	{
	}
#endif
}
