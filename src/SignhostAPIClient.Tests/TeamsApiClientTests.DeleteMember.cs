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
		public class DeleteMember
		{
			[Fact]
			public async Task When_DeleteMember_is_called__Then_we_should_have_called_team_id_get_once()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.DeleteMember));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var team = await signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public async Task When_DeleteMember_is_returned__Then_it_is_deserialized_correctly()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(
						HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.OK,
						new StringContent(Resources.DeleteMember));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				var teamWithoutMember = await signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				teamWithoutMember.Id.Should().Be("DZkAmRhY5MLnoz39R6KmTp");
				teamWithoutMember.Name.Should().Be("Team-8d3b01");
				teamWithoutMember.Members.Count().Should().Be(1);
				teamWithoutMember.InvitedMembers.Count().Should().Be(0);
				var member = teamWithoutMember.Members.First();
				member.Id.Should().Be(
					"BqTsn4rTX4iy6m4hLT8FGVBF9swfFvtgzk2rJxQzHU96");
				member.Name.Should().Be("John Doe");
				member.EmailAddress.Should().Be("testing@evidos.nl");
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteMember_is_called_and_request_is_bad__Then_BadRequestException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.BadRequest,
						new StringContent("{'message': 'bad request' }"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> deleteMember = () => signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				deleteMember.Should().Throw<BadRequestException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteMember_is_called_and_request_is_forbidden__Then_ForbiddenException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.Forbidden,
						new StringContent("{'message': 'You are not a member of this team'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> deleteMember = () => signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				deleteMember.Should().Throw<ForbiddenException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteMember_is_called_and_not_found__Then_NotFoundException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.NotFound,
						new StringContent("{'message': 'Member not found'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> deleteMember = () => signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				deleteMember.Should().Throw<NotFoundException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}

			[Fact]
			public void When_DeleteMember_is_called_and_unkownerror_like_418_occurs_then_we_should_get_a_SignhostException()
			{
				// Arrange
				var mockHttp = new MockHttpMessageHandler();
				mockHttp
					.Expect(HttpMethod.Delete,
						"http://localhost/api/Team/DZkAmRhY5MLnoz39R6KmTp/Member/BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH")
					.Respond(
						HttpStatusCode.UnprocessableEntity, //418 dont exist
						new StringContent("{'message': 'no sugar, no milk'"));
				using var httpClient = mockHttp.ToHttpClient();
				var signhostApiClient = new SignHostApiClient(
					settings, httpClient);

				// Act
				Func<Task> deleteMember = () => signhostApiClient.Teams
					.DeleteMemberAsync(
						"DZkAmRhY5MLnoz39R6KmTp",
						"BqTsn4rTX4iy6m4hLT8FGVTmpr4rsMvvgjexnkyaPJhH");

				// Assert
				deleteMember.Should().Throw<SignhostRestApiClientException>();
				mockHttp.VerifyNoOutstandingExpectation();
			}
		}
	}
}
