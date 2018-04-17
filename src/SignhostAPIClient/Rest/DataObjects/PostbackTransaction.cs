using System;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class PostbackTransaction 
		: Transaction
	{
		[JsonProperty("Checksum")]
		public string Checksum { get; set; }
	}
}
