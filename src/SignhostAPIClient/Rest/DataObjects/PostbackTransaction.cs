using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class PostbackTransaction
		: Transaction
	{
		[JsonPropertyName("Checksum")]
		public string Checksum { get; set; }
	}
}
