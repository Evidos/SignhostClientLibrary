using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class FileMeta
	{
		public int? DisplayOrder { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public IDictionary<string, FileSignerMeta> Signers { get; set; }

		public IDictionary<string, IDictionary<string, Field>> FormSets { get; set; }
	}
}
