using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class FilesInsomniaDocument
	{
		[JsonProperty("Links")]
		public Link[] Links { get; set; }

		[JsonProperty("DisplayName")]
		public string DisplayName { get; set; }
	}
}
