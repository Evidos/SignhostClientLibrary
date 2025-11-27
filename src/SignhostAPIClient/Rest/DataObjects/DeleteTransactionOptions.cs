namespace Signhost.APIClient.Rest.DataObjects;

public class DeleteTransactionOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether
	/// e-mail notifications should be send to the awaiting signers.
	/// </summary>
	public bool SendNotifications { get; set; }

	/// <summary>
	/// Gets or sets the reason of cancellation.
	/// </summary>
	public string? Reason { get; set; }
}
