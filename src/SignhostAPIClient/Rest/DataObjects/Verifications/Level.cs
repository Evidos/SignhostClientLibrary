namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Level of Assurance of eIDAS Login verification.
/// </summary>
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
