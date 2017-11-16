using System;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class Postback
	{
		[JsonProperty("Id")]
		public string Id { get; set; }

		[JsonProperty("Status")]
		public long Status { get; set; }

		[JsonProperty("Files")]
		public Files Files { get; set; }

		[JsonProperty("Seal")]
		public bool Seal { get; set; }

		[JsonProperty("Signers")]
		public Signer[] Signers { get; set; }

		[JsonProperty("Receivers")]
		public Receiver[] Receivers { get; set; }

		[JsonProperty("Reference")]
		public string Reference { get; set; }

		[JsonProperty("PostbackUrl")]
		public string PostbackUrl { get; set; }

		[JsonProperty("SignRequestMode")]
		public long SignRequestMode { get; set; }

		[JsonProperty("DaysToExpire")]
		public long DaysToExpire { get; set; }

		[JsonProperty("SendEmailNotifications")]
		public bool SendEmailNotifications { get; set; }

		[JsonProperty("CreatedDateTime")]
		public DateTimeOffset CreatedDateTime { get; set; }

		[JsonProperty("ModifiedDateTime")]
		public DateTimeOffset ModifiedDateTime { get; set; }

		[JsonProperty("CanceledDateTime")]
		public object CanceledDateTime { get; set; }

		[JsonProperty("Context")]
		public PostbackContext Context { get; set; }

		[JsonProperty("Checksum")]
		public string Checksum { get; set; }
	}
}
