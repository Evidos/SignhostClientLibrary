using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Receiver
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public string Language { get; set; }

		public string Message { get; set; }

		public string Reference { get; set; }

		public IList<dynamic> Activities { get; set; } = new List<dynamic>();

		public dynamic Context { get; set; }
	}
}
