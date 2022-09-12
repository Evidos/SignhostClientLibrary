namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Represents a Team to be created.
	/// </summary>
	/// The name of the Team to be created is wrapped within this structure to
	/// ensure forward compatibility.
	/// </remarks>
	public class TeamToCreate
	{
		/// <summary>
		/// Gets or sets the name of the Team to create.
		/// </summary>
		/// <value>
		/// String value of the Team name. Must not start with _$.
		/// </value>
		public string Name { get; set; }
	}
}
