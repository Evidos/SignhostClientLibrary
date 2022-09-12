namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Represent a member of a Team or an invited member of a Team.
	/// </summary>
	public class TeamMember
	{
		/// <summary>
		/// Gets or sets the Id of the Team member.
		/// </summary>
		/// <value>
		/// String value that uniquely identifies the member.
		/// </value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the full name of the member.
		/// </summary>
		/// <value>
		/// The first and last name in the format `{FirstName} {LastName}`.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the email address of the member.
		/// </summary>
		/// <value>
		/// The email address the User is known with.
		/// </value>
		public string EmailAddress { get; set; }
	}
}
