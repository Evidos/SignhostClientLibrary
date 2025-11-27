using System;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Activity
	{
		public string Id { get; set; }

		public ActivityType Code { get; set; }

		[JsonProperty("Activity")]
		public string ActivityValue { get; set; }

		public string Info { get; set; }

		public DateTimeOffset CreatedDateTime { get; set; }
	}
}
