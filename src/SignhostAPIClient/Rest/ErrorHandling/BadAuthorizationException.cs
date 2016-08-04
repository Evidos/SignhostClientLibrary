using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest.ErrorHandling
{
	public partial class BadAuthorizationException
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

		public BadAuthorizationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
