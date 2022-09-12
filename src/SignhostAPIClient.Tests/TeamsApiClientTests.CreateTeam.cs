using System.Net.Http;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.Tests.Responses;
using Xunit;
using System;
using Signhost.APIClient.Rest.ErrorHandling;
using Signhost.APIClient.Rest.DataObjects;
using System.Linq;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class CreateTeam
		{
			[Fact]
			public async Task When_CreateTeam_is_called__Then_we_should_have_called_team_post_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Post, "http://localhost/api/Team")
					.Respond(HttpStatusCode.OK,
						new StringContent(Resources.CreateTeam));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamsResponse = await signhostApiClient.Teams
					.CreateTeamAsync("Team-8d3b01");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_CreateTeam_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Post, "http://localhost/api/Team")
					.Respond(HttpStatusCode.OK,
						new StringContent(Resources.CreateTeam));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var createdTeam = await signhostApiClient.Teams
					.CreateTeamAsync("Team-8d3b01");

				// Assert
				createdTeam.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				createdTeam.Name.Should().Be("Team-8d3b01");
				createdTeam.Members.Count().Should().Be(1);
				createdTeam.InvitedMembers.Count().Should().Be(0);
				var member = createdTeam.Members.First();
				member.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_GetTeamById_is_called_and_request_is_bad__Then_we_should_get_a_BadRequestException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Post, "http://localhost/api/Team")
					.Respond(
						HttpStatusCode.BadRequest,
						new StringContent("{'message': 'bad request' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				// Act
				Func<Task<Team>> createTeam = () => signhostApiClient.Teams
					.CreateTeamAsync("Team-8d3b01");

				// Assert
				createTeam.Should().Throw<BadRequestException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_GetTeamById_is_called_and_request_is_forbidden__Then_we_should_get_a_ForbiddenException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Post, "http://localhost/api/Team")
					.Respond(
						HttpStatusCode.Forbidden,
						new StringContent(
							"{'message': 'Team name should not start with _$.'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task<Team>> createTeam = () => signhostApiClient.Teams
					.CreateTeamAsync("_$Team-8d3b01");

				// Assert
				createTeam.Should().Throw<ForbiddenException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
