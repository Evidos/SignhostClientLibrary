using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Interface abstracting the available Signhost API calls.
	/// </summary>
	public class TeamsApiClient
		: ITeamsApiClient
	{
		private readonly HttpClient httpClient;

		/// <summary>
		/// Initializes a new instance of the <see cref="TeamsApiClient"/>
		/// class.
		/// </summary>
		/// <param name="httpClient">
		/// An HttpClient instance where the correct headers are set.
		/// </param>
		public TeamsApiClient(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		/// <inheritdoc />
		public async Task<Team> AcceptInviteAsync(
			string teamId,
			CancellationToken cancellationToken = default)
		{
			var path = "Team".JoinPaths(teamId, "Invite");
			var result = await httpClient
				.PutAsync(path, null, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);
			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);
			return team;
		}

		/// <inheritdoc />
		public async Task<Team> CreateTeamAsync(
			string teamName,
			string idempotencyKey = null,
			CancellationToken cancellationToken = default)
		{
			var teamToCreate = new TeamToCreate {
				Name = teamName,
			};
			var request = new HttpRequestMessage(HttpMethod.Post, "Team") {
				Content = JsonContent.From(teamToCreate),
			};
			if (idempotencyKey != null) {
				request.Headers.Add("Idempotency-Key", idempotencyKey);
			}

			var result = await httpClient
				.SendAsync(request, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.Created)
				.ConfigureAwait(false);

			return await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<Team> DeleteMemberAsync(
			string teamId,
			string memberId,
			CancellationToken cancellationToken = default)
		{
			var path = "Team".JoinPaths(teamId, "Member", memberId);
			var result = await httpClient
				.DeleteAsync(path, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);

			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);

			return team;
		}

		/// <inheritdoc />
		public async Task DeleteTeamAsync(
			string teamId,
			CancellationToken cancellationToken = default)
		{
			var path = "Team".JoinPaths(teamId);
			await httpClient
				.DeleteAsync(path, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.NoContent)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<Team> GetTeamByIdAsync(
			string teamId,
			CancellationToken cancellationToken = default)
		{
			var result = await httpClient
				.GetAsync("Team".JoinPaths(teamId), cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);

			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);

			return team;
		}

		/// <inheritdoc />
		public async Task<TeamsResponse> GetTeamsAsync(
			CancellationToken cancellationToken = default)
		{
			var result = await httpClient
				.GetAsync("Team", cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);

			var teams = await result.Content
				.FromJsonAsync<TeamsResponse>()
				.ConfigureAwait(false);

			return teams;
		}

		/// <inheritdoc />
		public async Task<Team> InviteMemberAsync(
			string teamId,
			string emailAddress,
			CancellationToken cancellationToken = default)
		{
			var path = "Team".JoinPaths(teamId, "Invite", emailAddress);
			var result = await httpClient
				.PutAsync(path, null, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);
			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);
			return team;
		}

		/// <inheritdoc />
		public async Task<Team> RejectInviteAsync(
			string teamId,
			CancellationToken cancellationToken = default)
		{
			var result = await httpClient
				.DeleteAsync(
					"Team".JoinPaths(teamId, "Invite"),
					cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);

			// TODO: makes no sense
			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);
			return team;
		}

		/// <inheritdoc />
		public async Task<Team> RevokeInviteAsync(
			string teamId,
			string userId,
			CancellationToken cancellationToken = default)
		{
			var path = "Team".JoinPaths(teamId, "Invite", userId);
			var result = await httpClient
				.DeleteAsync(path, cancellationToken)
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.OK)
				.ConfigureAwait(false);
			var team = await result.Content
				.FromJsonAsync<Team>()
				.ConfigureAwait(false);
			return team;
		}
	}
}
