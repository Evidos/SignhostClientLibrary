using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest.DataObjects
{
	public class FileMeta
	{
		public int? DisplayOrder { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public IDictionary<string, FileSignerMeta> Signers { get; set; }
	}
}
