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
using System.Web;

namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		public class InviteMember
		{
			private static readonly string urlEncodedEmail =
				HttpUtility.UrlEncode("testing2@evidos.com");

			[Fact]
			public async Task When_InviteMember_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.InviteMember));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"testing2@evidos.com");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_InviteMember_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.InviteMember));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamWithInvitedMember = await signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp", "testing2@evidos.com");

				// Assert
				teamWithInvitedMember.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				teamWithInvitedMember.Name.Should().Be("Team-8d3b01");
				teamWithInvitedMember.Members.Count().Should().Be(1);
				teamWithInvitedMember.InvitedMembers.Count().Should().Be(1);
				var existing = teamWithInvitedMember.Members.First();
				existing.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				existing.Name.Should().Be("John Doe");
				existing.EmailAddress.Should().Be("testing@evidos.nl");
				var invited = teamWithInvitedMember.InvitedMembers.First();
				invited.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");
				invited.Name.Should().Be("James Smith");
				invited.EmailAddress.Should().Be("testing2@evidos.com");
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_InviteMember_is_called_and_request_is_bad__Then_BadRequestException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.BadRequest,
						new StringContent("{'message': 'bad request' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> inviteMember = () => signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"testing2@evidos.com");

				// Assert
				inviteMember.Should().Throw<BadRequestException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_InviteMember_is_called_and_request_is_forbidden__Then_ForbiddenException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.Forbidden,
						new StringContent("{'message': 'forbidden'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> getTeamById = () => signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"testing2@evidos.com");

				// Assert
				getTeamById.Should().Throw<ForbiddenException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_InviteMember_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.NotFound,
						new StringContent("{'message': 'Member not found with that email address'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> getTeamById = () => signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"testing2@evidos.com");

				// Assert
				getTeamById.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_InviteMember_is_called_and_unkownerror_like_418_occurs_then_we_should_get_a_SignhostException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Put,
						$"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Invite/{urlEncodedEmail}")
					.Respond(
						HttpStatusCode.UnprocessableEntity,
						new StringContent("{'message': 'coffee is tasty'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> getTeamById = () => signhostApiClient.Teams
					.InviteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"testing2@evidos.com");

				// Assert
				getTeamById.Should().Throw<SignhostRestApiClientException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
