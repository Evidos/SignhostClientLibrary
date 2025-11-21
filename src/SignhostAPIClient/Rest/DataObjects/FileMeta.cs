using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.DataObjects;

public class FileMeta
{
	public int? DisplayOrder { get; set; }

	public string DisplayName { get; set; }

	public string Description { get; set; }

	public IDictionary<string, FileSignerMeta> Signers { get; set; }

	public IDictionary<string, IDictionary<string, Field>> FormSets { get; set; }

	/// <summary>
	/// Gets or sets whether to use the scribble signature as a paraph
	/// on each non-signed page.
	/// Don't use this setting unless you are really sure this is what you
	/// want and know the side-effects.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public bool? SetParaph { get; set; }
}
