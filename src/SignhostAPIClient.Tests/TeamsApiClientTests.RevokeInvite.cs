using System.Net.Http;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.Tests.Responses;
using Xunit;
using System;
using Signhost.APIClient.Rest.ErrorHandling;
using System.Linq;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class RevokeInvite
		{
			[Fact]
			public async Task When_AcceptInvite_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.RevokeInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.RevokeInviteAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_RevokeInvite_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.RevokeInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamWithRevokedInvite = await signhostApiClient.Teams
					.RevokeInviteAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				teamWithRevokedInvite.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				teamWithRevokedInvite.Name.Should().Be("Team-8d3b01");
				teamWithRevokedInvite.Members.Count().Should().Be(1);
				teamWithRevokedInvite.InvitedMembers.Count().Should().Be(0);
				var member = teamWithRevokedInvite.Members.First();
				member.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_AcceptInvite_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.NotFound,
						new StringContent("{'message': 'Invite not found'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> revokeInvite = () => signhostApiClient.Teams
					.RevokeInviteAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				revokeInvite.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_RevokeInvite_is_called_and_forbidden__Then_ForbiddenException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.Forbidden,
						new StringContent("{'message': 'forbidden'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> revokeInvite = () => signhostApiClient.Teams
					.RevokeInviteAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				revokeInvite.Should().Throw<ForbiddenException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
