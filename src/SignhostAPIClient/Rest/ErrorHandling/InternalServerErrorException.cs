using System;
using System.Net.Http.Headers;

namespace Signhost.APIClient.Rest.ErrorHandling;

[Serializable]
public class InternalServerErrorException
	: SignhostRestApiClientException
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
		string message, RetryConditionHeaderValue? retryAfter)
		: base(message)
	{
		HelpLink = "https://api.signhost.com/Help";

		if (retryAfter is not null) {
			if (retryAfter.Date is not null) {
				RetryAfter = retryAfter.Date;
			}

			if (retryAfter.Delta is not null) {
				RetryAfter = DateTimeOffset.Now + retryAfter.Delta;
			}
		}
	}

#if SERIALIZABLE
	protected InternalServerErrorException(
		SerializationInfo info,
		StreamingContext context)
		: base(info, context)
	{
	}
#endif

	private DateTimeOffset? RetryAfter { get; set; }
}
