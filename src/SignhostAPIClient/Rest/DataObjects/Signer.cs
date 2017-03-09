using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Signer
	{
		public Signer()
		{
		}

		[JsonConstructor]
		private Signer(IReadOnlyList<IVerification> verifications)
		{
			Verifications = verifications;
		}

		public string Id { get; set; }

		public DateTime? Expires { get; set; }

		public string Email { get; set; }

		public string SignRequestMessage { get; set; }

		public string Mobile { get; set; }

		public IReadOnlyList<IVerification> Verifications { get; private set; }
			= new List<IVerification>().AsReadOnly();

		[Obsolete("Iban is obsolete, switch to Verification object")]
		public string Iban { get; set; }

		public string BSN { get; set; }

		public bool RequireScribble { get; set; }

		public bool RequireScribbleName { get; set; }

		[Obsolete("No longer supported")]
		public bool RequireEmailVerification { get; set; }

		public bool RequireSmsVerification { get; set; }

		[Obsolete("RequireIdealVerification is obsolete, switch to Verification object")]
		public bool RequireIdealVerification { get; set; }

		public bool RequireDigidVerification { get; set; }

		public bool RequireKennisnetVerification { get; set; }

		public bool RequireSurfnetVerification { get; set; }

		public bool SendSignRequest { get; set; }

		public bool? SendSignConfirmation { get; set; }

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
