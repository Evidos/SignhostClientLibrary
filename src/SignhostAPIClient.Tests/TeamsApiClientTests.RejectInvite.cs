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
		public class RejectInvite
		{
			[Fact]
			public async Task When_RejectInvite_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.RejectInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.RejectInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_RejectInvite_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(HttpStatusCode.OK,
						new StringContent(Resources.RejectInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamWithRejectedInvite = await signhostApiClient.Teams
					.RejectInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				teamWithRejectedInvite.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				teamWithRejectedInvite.Name.Should().Be("Team-8d3b01");
				teamWithRejectedInvite.Members.Count().Should().Be(1);
				teamWithRejectedInvite.InvitedMembers.Count().Should().Be(0);
				var member = teamWithRejectedInvite.Members.First();
				member.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_RejectInvite_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(
						HttpStatusCode.NotFound,
						new StringContent("{'message': 'Invite not found'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				Func<Task> rejectInvite = () => signhostApiClient.Teams
					.RejectInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				rejectInvite.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}