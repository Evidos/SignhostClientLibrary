namespace Signhost.APIClient.Rest.DataObjects;

public class ScribbleVerification
	: IVerification
{
	public bool RequireHandsignature { get; set; }

	public bool ScribbleNameFixed { get; set; }

	public string? ScribbleName { get; set; }
}
