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
		public class AcceptInvite
		{
			[Fact]
			public async Task When_AcceptInvite_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.AcceptInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.AcceptInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_AcceptInvite_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Put,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.AcceptInvite));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.AcceptInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				team.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				team.Name.Should().Be("Team-8d3b01");
				team.Members.Count().Should().Be(2);
				team.InvitedMembers.Count().Should().Be(0);
				var existingMember = team.Members.FirstOrDefault(x =>
					x.Id == "BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				existingMember.Name.Should().Be("John Doe");
				existingMember.EmailAddress.Should().Be("testing@evidos.nl");
				var newMember = team.Members.FirstOrDefault(x =>
					x.Id == "BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");
				newMember.Name.Should().Be("James Smith");
				newMember.EmailAddress.Should().Be("testing2@evidos.com");
			}

			[Fact]
			public void When_AcceptInvite_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(HttpStatusCode.NotFound,
						new StringContent("{'message': 'User is not invited' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				Func<Task> acceptInvite = () => signhostApiClient.Teams
					.AcceptInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				acceptInvite.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_AcceptInvite_is_called_and_unkownerror_like_409_occurs_then_we_should_get_a_SignhostException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite")
					.Respond(HttpStatusCode.Conflict,
						new StringContent("{'message': 'Currently processing creation of team' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settingsOtherUser, httpClient);

				// Act
				Func<Task> acceptInvite  = () => signhostApiClient.Teams
					.AcceptInviteAsync("DZkAmRhY5MLnoz39R6KmTp");

				// Assert
				acceptInvite.Should().Throw<SignhostRestApiClientException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
