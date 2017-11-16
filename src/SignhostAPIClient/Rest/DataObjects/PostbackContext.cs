using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Signhost.APIClient.Rest.DataObjects
{
	public partial class PostbackContext
	{
		[JsonProperty("Environment")]
		public string Environment { get; set; }

		[JsonProperty("Context")]
		public bool Context { get; set; }
	}
}
