using System.Threading;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Interface for use of Signhost Team api calls.
	/// </summary>
	public interface ITeamsApiClient
	{
		/// <summary>
		/// Creates a new Team and adds the current User as a member to that
		/// team.
		/// </summary>
		/// <param name="teamName">
		/// Name of the team. Should not start with _$. The actual name of the
		/// Team that is assigned to the Team may differ. Do not assume that
		/// the value of this parameter will become the exact value of the Name
		/// of the cerated Team, use the response value instead.
		/// </param>
		/// <param name="idempotencyKey">
		/// Pass in an idempotency key to prevent multiple requests with equal
		/// payload (and idempotency key values) to result in multiple created
		/// teams. See
		/// <see href="https://datatracker.ietf.org/doc/html/draft-idempotency-header-01">here</see>.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		/// A Team instance that with the <see cref="Team.Name" /> property
		/// assigned a value that may slightly differ from the
		/// <see cref="teamName" /> value.
		/// </returns>
		/// <exception cref="BadRequestException">
		/// Thrown when input data is incorrect:
		/// * When the idempotency key length exceeds 64 chars
		/// * When the <see cref="teamName"> is empty
		/// * When the <see cref="teamName"> starts with _$
		/// </exception>
		/// <exception cref="SignhostRestApiClientException">
		/// * When a previous POST request with the same idempotencykey is still
		/// 	being processed
		/// * When another POST request with a different payload but equal
		/// 	idempotency key is sent
		/// * When a team with given <see cref="teamName" /> already exists.
		/// </exception>
		Task<Team> CreateTeamAsync(
			string teamName,
			string idempotencyKey = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns a list of Teams the User is a member of or is invited as a
		/// member.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		/// A list of Teams that the User is member of or is invited to. When
		/// the User is no member of a team and is not invited to any team, the
		/// returned list will be empty.
		/// </returns>
		Task<TeamsResponse> GetTeamsAsync(
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns a Team with given <see cref="teamId" />.
		/// </summary>
		/// <param name="teamId">
		/// Id of the team to retrieve the members and invited members for.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		/// Information about the Team including members and invited members
		/// and their details.
		/// </returns>
		/// <exception cref="ForbiddenException">
		/// When the User is not a member of the requested Team or the User is
		/// not invited for the requested Team.
		/// </exception>
		Task<Team> GetTeamByIdAsync(
			string teamId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Invite a User with given <see cref="emailAddress" /> to the Team
		/// with given <see cref="teamName" />.
		/// </summary>
		/// <param name="teamId">
		/// Id of the team to invite the member to.
		/// </param>
		/// <param name="emailAddress">
		/// Email address of the User to invite into the Team. The email address
		/// value will be urlEncoded, so the input value does not need to encode
		/// it. Url encoding is necessary since there will be an '@' character
		/// (%40) in the email address value.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		/// An updated Team instance where the invited user is included in the
		/// <see cref="Team.InvitedMembers" /> property as an item.
		/// </returns>
		/// <exception cref="ForbiddenException">
		/// When the User performing the request is not a member of the Team.
		/// </exception>
		/// <exception cref="NotFoundException">
		/// * When the Team could not be found.
		/// * No member with the given <see cref="emailAddress" /> could be
		/// 	found.
		/// </exception>
		/// <exception cref="BadRequestException">
		/// When the Team does not contain any members.
		/// </exception>
		/// <exception cref="SignhostRestApiClientException">
		/// * When the <see cref="emailAddress" /> is not formatted according
		/// 	RFC 5321 and RFC 5322
		/// * When the User is already a member of the Team.
		/// </exception>
		Task<Team> InviteMemberAsync(
			string teamId,
			string emailAddress,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Accepts an invite for given a Team with given <see cref="teamId" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="SignHostApiClientSettings" /> determine the User for
		/// which the Invite of the given team is applied to.
		/// </remarks>
		/// <param name="teamId">
		/// Id of the Team to accept the Invite for.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <exception cref="NotFoundException">
		/// * User is not invited for the given Team.
		/// * When the Team could not be found.
		/// </exception>
		/// <exception cref="SignhostRestApiClientException">
		/// When the User is already a member of the given Team.
		/// </exception>
		/// <returns>
		/// Team instance with the invited User now part as a member instead of
		/// an invited member.
		/// </returns>
		Task<Team> AcceptInviteAsync(
			string teamId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Rejects the invite of the Team with given <see cref="teamId" />.
		/// </summary>
		/// <param name="teamId">
		/// Id of the Team to reject the Invite for.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <exception cref="NotFoundException">
		/// * When the Team could not be found.
		/// * When the User is not invited for the given Team.
		/// </exception>
		/// <returns>
		/// Team instance with the invited User deleted from the list of
		/// <see cref="Team.InvitedMembers">invited members</see>.
		/// </returns>
		Task<Team> RejectInviteAsync(
			string teamId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Revokes an Invite for Team with given <see cref="teamId" /> for User
		/// with given <see cref="userId" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="userId" /> refers to the invited User, not the User
		/// performing the revocation of the Invite. The User performing the
		/// revocation of the Invite is specified in the
		/// <see cref="SignHostApiClientSettings" />.
		/// </remarks>
		/// <param name="teamId">
		/// Id of the Team to revoke the Invite for.
		/// </param>
		/// <param name="userId">
		/// UserId of the User whose Invite will be revoked.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <exception cref="NotFoundException">
		/// * When the User is not invited for the given Team
		/// * When the User is not found.
		/// * When the Team could not be found.
		/// </exception>
		/// <returns>
		/// Team instance with the invited User deleted from the list of
		/// <see cref="Team.InvitedMembers">invited members</see>.
		/// </returns>
		Task<Team> RevokeInviteAsync(
			string teamId,
			string userId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes the member with given <see cref="userId" /> from the Team
		/// with given <see cref="teamId" />.
		/// </summary>
		/// <param name="teamId">
		/// Id of the Team to delete the member from.
		/// </param>
		/// <param name="userId">
		/// UserId of the User who will be deleted from the Team.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <exception cref="NotFoundException">
		/// * When the User is not a member of the Team.
		/// * When the User is not found.
		/// * When the Team could not be found.
		/// </exception>
		/// <exception cref="ForbiddenException">
		/// When the User performing the request is not a member of the Team.
		/// </exception>
		/// <exception cref="BadRequestException">
		/// When the Team does not contain any members.
		/// </exception>
		/// <returns>
		/// a Team instance where the member is deleted from the
		/// <see cref="Team.Members">list of members</see>.
		/// </returns>
		Task<Team> DeleteMemberAsync(
			string teamId,
			string userId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes Team with given <see cref="teamId" />.
		/// </summary>
		/// <param name="teamId">
		/// Id of the Team to delete.
		/// </param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <exception cref="NotFoundException">
		/// When the Team could not be found.
		/// </exception>
		/// <exception cref="ForbiddenException">
		/// When the User performing the request is not a member of the Team.
		/// </exception>
		/// <exception cref="BadRequestException">
		/// When the Team does not contain any members.
		/// </exception>
		/// <returns>
		/// An awaitable task instance.
		/// </returns>
		Task DeleteTeamAsync(
			string teamId,
			CancellationToken cancellationToken = default);
	}
}
