namespace Signhost.APIClient.Rest.DataObjects;

public class EherkenningVerification
	: IVerification
{
	/// <summary>
	/// Gets or sets the Uid.
	/// </summary>
	public string Uid { get; set; }

	/// <summary>
	/// Gets or sets the entity concern ID / KVK number.
	/// </summary>
	public string EntityConcernIdKvkNr { get; set; }
}
