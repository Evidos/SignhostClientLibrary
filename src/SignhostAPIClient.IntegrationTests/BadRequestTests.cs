using System.IO;
using System.Threading.Tasks;
using System;
using FluentAssertions;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;
using Xunit;

namespace Signhost.APIClient.Rest.IntegrationTests;

public class BadRequestTests
	: IntegrationTestBase
{
	[Theory]
	[MemberData(nameof(BadCreateRequestTestCases))]
	public async Task Given_invalid_create_request_When_transaction_is_created_Then_BadRequestException_is_thrown(
		CreateTransactionRequest badRequest,
		string expectedErrorMessage)
	{
		// Act
		Func<Task> act = () => Client.CreateTransactionAsync(badRequest);

		// Assert
		var exception = await act.Should()
			.ThrowAsync<BadRequestException>();
		exception.Which.Message.Should().Be(expectedErrorMessage);
		exception.Which.ResponseBody.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task Given_invalid_file_name_When_file_is_uploaded_Then_BadRequestException_is_thrown()
	{
		// Arrange
		CreateTransactionRequest transaction = new() {
			SendEmailNotifications = false,
			Signers = [
				new() {
					Id = "signer1",
					Email = "test@example.com",
					SendSignRequest = false,
				}
			]
		};

		var createdTransaction = await Client.CreateTransactionAsync(transaction);
		string pdfPath = Path.Combine("TestFiles", "small-example-pdf-file.pdf");
		await using var fileStream = File.OpenRead(pdfPath);

		// Act
		Func<Task> act = () => Client.AddOrReplaceFileToTransactionAsync(
			fileStream,
			createdTransaction.Id,
			"invalid|filename.pdf",
			new FileUploadOptions());

		// Assert
		var exception = await act.Should()
			.ThrowAsync<BadRequestException>();
		exception.Which.Message.Should().Be("invalid|filename.pdf is not a valid file name.");
		exception.Which.ResponseBody.Should().NotBeNullOrEmpty();
	}

	public static TheoryData<CreateTransactionRequest, string> BadCreateRequestTestCases =>
		new() {
			{
				new CreateTransactionRequest {
					SendEmailNotifications = false,
					DaysToExpire = 1,
					Signers = [
						new() {
							Email = "test@example.com",
							SendSignRequest = true,
							SignRequestMessage = "Please sign.",
							DaysToRemind = 30,
						}
					],
				},
				"The days to remind must be lower than the days to expire."
			},
			{
				new CreateTransactionRequest {
					SendEmailNotifications = false,
					Signers = [
						new() {
							Email = "test@example.com",
							SendSignRequest = true,
							SignRequestMessage = null,
						}
					],
				},
				"The sign request message is null or empty."
			},
			{
				new CreateTransactionRequest {
					SendEmailNotifications = false,
					Signers = [
						new() {
							Email = "test@example.com",
							SendSignRequest = true,
							SignRequestMessage = "Please sign.",
							SignRequestSubject = "Subject\twith\ttabs",
						}
					],
				},
				"Signer Subject: Input contains one or more invalid characters."
			},
			{
				new CreateTransactionRequest {
					SendEmailNotifications = false,
					Signers = [
						new() {
							Id = "duplicate-signer-id",
							Email = "signer1@example.com",
							SendSignRequest = false,
						},
						new() {
							Id = "duplicate-signer-id",
							Email = "signer2@example.com",
							SendSignRequest = false,
						}
					],
				},
				"The signer id is already used by another signer."
			},
		};
}
