using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Signhost.APIClient.Rest.JsonConverters;

namespace Signhost.APIClient.Rest.DataObjects;

/// <summary>
/// Verification object for eIDAS.
/// </summary>
public class EidasLoginVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the uid.
	/// </summary>
	public string? Uid { get; set; }

	/// <summary>
	/// Gets or sets the level.
	/// </summary>
	[JsonConverter(typeof(LevelEnumConverter))]
	public Level? Level { get; set; }

	/// <summary>
	/// Gets or sets the first name.
	/// </summary>
	public string? FirstName { get; set; }

	/// <summary>
	/// Gets or sets the last name.
	/// </summary>
	public string? LastName { get; set; }

	/// <summary>
	/// Gets or sets the date of birth.
	/// </summary>
	public DateTime? DateOfBirth { get; set; }

	/// <summary>
	/// Gets or sets the eIDAS attributes.
	/// </summary>
	public IDictionary<string, string>? Attributes { get; set; }
}
