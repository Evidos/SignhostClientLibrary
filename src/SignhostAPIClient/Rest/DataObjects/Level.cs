using Newtonsoft.Json;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Level of Assurance.
	/// </summary>
	[JsonConverter(typeof(LevelEnumConverter))]
	public enum Level
	{
		/// <summary>
		/// Unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Low.
		/// </summary>
		Low,

		/// <summary>
		/// Substantial.
		/// </summary>
		Substantial,

		/// <summary>
		/// High.
		/// </summary>
		High,
	}
}
