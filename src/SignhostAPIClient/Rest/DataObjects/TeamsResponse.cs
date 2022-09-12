using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Response of a call to retrieve all Teams.
	/// </summary>
	/// <remarks>
	/// The list of Teams are wrapped within this structure to ensure forward
	/// compatibility.
	/// </remarks>
	public class TeamsResponse
	{
		/// <summary>
		/// gets or sets a list of teams the authenticated user is a member of.
		/// </summary>
		public IEnumerable<Team> Teams { get; set; }
	}
}
