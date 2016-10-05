using System;
using System.Net.Http.Headers;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	public partial class InternalServerErrorException
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
			string message, Exception innerException, RetryConditionHeaderValue retryAfter)
			: base(message, innerException)
		{
			HelpLink = "https://api.signhost.com/Help";

			if (retryAfter != null) {
				if (retryAfter.Date != null) {
					RetryAfter = retryAfter.Date;
				}

				if (retryAfter.Delta != null) {
					RetryAfter = DateTime.Now + retryAfter.Delta;
				}
			}
		}

		private DateTimeOffset? RetryAfter { get; set; }
	}
}
