using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class SurfnetVerification
		: IVerification
	{
		public string Type => "SURFnet";

		public string Uid { get; set; }

		public IDictionary<string, string> Attributes { get; set; }
	}
}
