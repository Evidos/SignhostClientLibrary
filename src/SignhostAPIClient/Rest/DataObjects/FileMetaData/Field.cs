namespace Signhost.APIClient.Rest.DataObjects;

public class Field
{
	public FileFieldType Type { get; set; }

	// TO-DO: Can be boolean, number, string, should be fixed in v5.
	public string? Value { get; set; }

	public Location Location { get; set; } = default!;
}
