using System.Net.Http;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.Tests.Responses;
using Xunit;
using System;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class Delete
		{
			[Fact]
			public async Task When_DeleteTeam_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(HttpStatusCode.NoContent);
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				await signhostApiClient.Teams
					.DeleteTeamAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteTeam_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(
						HttpStatusCode.NotFound,
						new StringContent("{'message': 'Team not found' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> delete = () => signhostApiClient.Teams
					.DeleteTeamAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				delete.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteTeam_is_called_and_unkownerror_occurs_then_we_should_get_a_SignhostException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp")
					.Respond(
						HttpStatusCode.NotAcceptable,
						new StringContent("{'message': 'does not matter'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> delete  = () => signhostApiClient.Teams
					.DeleteTeamAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				delete.Should().Throw<SignhostRestApiClientException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
