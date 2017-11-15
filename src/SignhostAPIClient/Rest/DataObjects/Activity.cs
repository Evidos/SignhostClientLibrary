using System;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class Activity
	{
		public string Id { get; set; }

		public ActivityType Code { get; set; }

		public string Info { get; set; }

		public DateTimeOffset CreatedDateTime { get; set; }
	}
}
