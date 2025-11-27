namespace Signhost.APIClient.Rest.DataObjects;

public class Field
{
	// TO-DO: Make enum in v5.
	public string Type { get; set; } = default!;

	// TO-DO: Can be boolean, number, string, should be fixed in v5.
	public string? Value { get; set; }

	public Location Location { get; set; } = default!;
}
