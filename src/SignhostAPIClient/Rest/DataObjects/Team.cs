using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Represents a team created by a user.
	/// </summary>
	public class Team
	{
		/// <summary>
		/// Gets or sets the id of the team.
		/// </summary>
		/// <value>a string representing the unique id of this team.</value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the team.
		/// </summary>
		/// <value>a string representing the name of this team.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the the members of this Team.
		/// </summary>
		/// <value>
		/// All the members of the Team that became a member either by creating
		/// the Team or by accepting an Invite for this Team.
		/// </value>
		public IEnumerable<TeamMember> Members { get; set; }

		/// <summary>
		/// Gets or sets the invited members of this Team.
		/// </summary>
		/// <value>
		/// All the Users that are invited but have not yet accepted their
		/// Invite for this Team.
		/// </value>
		public IEnumerable<TeamMember> InvitedMembers { get; set; }
	}
}
