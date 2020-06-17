using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class PostbackTransaction
		: Transaction
	{
		public PostbackTransaction()
		{
		}

		[JsonConstructor]
		private PostbackTransaction(
			IReadOnlyDictionary<string, FileEntry> files,
			DateTimeOffset? createdDateTime,
			DateTimeOffset? canceledDateTime,
			string cancelationReason)
			: base(files, createdDateTime, canceledDateTime, cancelationReason)
		{
		}

		[JsonProperty("Checksum")]
		public string Checksum { get; set; }
	}
}
