using System.Net.Http;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.Tests.Responses;
using Xunit;
using System.Linq;
using System;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class GetTeams
		{
			[Fact]
			public async Task When_GetTeams_is_called__Then_we_should_have_called_teams_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team")
					.Respond(HttpStatusCode.OK, new StringContent(Resources.GetTeams));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				// Act
				var teamsResponse = await signhostApiClient.Teams.GetTeamsAsync();

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_GetTeams_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team")
					.Respond(HttpStatusCode.OK, new StringContent(Resources.GetTeams));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				// Act
				var teamsResponse = await signhostApiClient.Teams.GetTeamsAsync();

				// Assert
				teamsResponse.Teams.Count().Should().Be(1);
				var team = teamsResponse.Teams.First();
				team.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				team.Name.Should().Be("Team-8d3b01");
				team.Members.Count().Should().Be(1);
				team.InvitedMembers.Count().Should().Be(0);
				var member = team.Members.First();
				member.Id.Should().Be("BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
			}

			[Fact]
			public void When_GetTeams_is_called_and_the_authorization_is_bad_then_we_should_get_a_BadAuthorizationException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team")
					.Respond(
						HttpStatusCode.Unauthorized,
						new StringContent("{'message': 'no authorization'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task<TeamsResponse>> getTeams = () =>
					signhostApiClient.Teams.GetTeamsAsync();

				// Assert
				getTeams.Should().Throw<UnauthorizedAccessException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_GetTeams_is_called_and_there_is_an_internal_server_error_we_should_get_a_InternalServerErrorException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Get, "http://localhost/api/Team")
					.Respond(
						HttpStatusCode.InternalServerError,
						new StringContent("{'message': 'error'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task<TeamsResponse>> getTeams = () =>
					signhostApiClient.Teams.GetTeamsAsync();

				// Assert
				getTeams.Should().Throw<InternalServerErrorException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
