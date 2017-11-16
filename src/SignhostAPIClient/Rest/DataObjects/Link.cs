using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class Link
	{
		[JsonProperty("Rel")]
		public string Rel { get; set; }

		[JsonProperty("Type")]
		public string Type { get; set; }

		[JsonProperty("Link")]
		public string LinkLink { get; set; }
	}
}
