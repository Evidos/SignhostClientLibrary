using System.Net.Http;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.Tests.Responses;
using Xunit;
using System.Linq;
using System;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class GetTeamById
		{
			[Fact]
			public async Task When_GetTeamById_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.GetTeamById));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamsResponse = await signhostApiClient.Teams
					.GetTeamByIdAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_GetTeamById_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Get,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.GetTeamById));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.GetTeamByIdAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				team.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				team.Name.Should().Be("Team-8d3b01");
				team.Members.Count().Should().Be(1);
				team.InvitedMembers.Count().Should().Be(0);
				var member = team.Members.First();
				member.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
			}

			[Fact]
			public void When_GetTeamById_called_by_nonmember__Then_ForbiddenException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(
						HttpStatusCode.Forbidden,
						new StringContent(
							"{'message': 'User with id BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH is not a member of team with Id DZkAmRhY5MLnoz39R6KmTp'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> getTeamById = () => signhostApiClient.Teams
					.GetTeamByIdAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				getTeamById.Should().Throw<ForbiddenException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}