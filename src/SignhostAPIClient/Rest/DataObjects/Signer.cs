using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Signer
	{
		public DateTime? Expires { get; set; }

		public string Email { get; set; }

		public string SignRequestMessage { get; set; }

		public string Mobile { get; set; }

		public string Iban { get; set; }

		public string BSN { get; set; }

		public bool RequireScribble { get; set; }

		public bool RequireScribbleName { get; set; }

		public bool RequireEmailVerification { get; set; }

		public bool RequireSmsVerification { get; set; }

		public bool RequireIdealVerification { get; set; }

		public bool RequireDigidVerification { get; set; }

		public bool RequireKennisnetVerification { get; set; }

		public bool RequireSurfnetVerification { get; set; }

		public bool SendSignRequest { get; set; }

		public bool? SendSignConfirmation { get; set; }

		[Obsolete("Use transaction.DaysToRemind instead")]
		public int DaysToRemind { get; set; }

		public string Language { get; set; }

		public string ScribbleName { get; set; }

		public bool ScribbleNameFixed { get; set; }

		public string Reference { get; set; }

		public string ReturnUrl { get; set; }

		public string RejectReason { get; set; }

		public string SignUrl { get; set; }

		public IList<dynamic> Activities { get; set; } = new List<dynamic>();

		public dynamic Context { get; set; }
	}
}
