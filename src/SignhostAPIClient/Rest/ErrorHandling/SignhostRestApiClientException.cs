using System;

namespace Signhost.APIClient.Rest.ErrorHandling;

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

	public SignhostRestApiClientException(
		string message,
		Exception innerException)
		: base(message, innerException)
	{
		HelpLink = "https://api.signhost.com/Help";
	}

#if SERIALIZABLE
		protected SignhostRestApiClientException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
#endif

	/// <summary>
	/// Gets or sets the response body returned from the Signhost REST API.
	/// </summary>
	public string ResponseBody { get; set; }
}
