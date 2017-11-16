using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class Files
	{
		[JsonProperty("InsomniaDocument")]
		public FilesInsomniaDocument InsomniaDocument { get; set; }
	}
}
