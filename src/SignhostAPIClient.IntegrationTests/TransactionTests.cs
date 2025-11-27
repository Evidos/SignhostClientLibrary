using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Signhost.APIClient.Rest.DataObjects;
using Xunit;

namespace Signhost.APIClient.Rest.IntegrationTests;

public class TransactionTests
	: IDisposable
{
	private readonly SignhostApiClient client;
	private readonly TestConfiguration config;

	public TransactionTests()
	{
		config = TestConfiguration.Instance;

		if (!config.IsConfigured) {
			throw new InvalidOperationException(
				"Integration tests are not configured");
		}

		var settings = new SignhostApiClientSettings(config.AppKey, config.UserToken) {
			Endpoint = config.ApiBaseUrl
		};

		client = new SignhostApiClient(settings);
	}

	[Fact]
	public async Task Given_complex_transaction_When_created_and_started_Then_all_properties_are_correctly_persisted()
	{
		// Arrange
		var testReference = $"IntegrationTest-{DateTime.UtcNow:yyyyMMddHHmmss}";
		var testPostbackUrl = "https://example.com/postback";
		var signerEmail = "john.doe@example.com";
		var signerReference = "SIGNER-001";
		var signerIntroText = "Please review and sign this document carefully.";
		var signerExpires = DateTimeOffset.UtcNow.AddDays(15);
		var receiverEmail = "receiver@example.com";
		var receiverName = "Jane Receiver";
		var receiverReference = "RECEIVER-001";

		var transaction = new Transaction {
			Seal = false,
			Reference = testReference,
			PostbackUrl = testPostbackUrl,
			DaysToExpire = 30,
			SendEmailNotifications = false,
			SignRequestMode = 2,
			Language = "en-US",
			Context = new {
				TestContext = "integration-test",
			},
			Signers = [
				new Signer {
					Id = "signer1",
					Email = signerEmail,
					Reference = signerReference,
					IntroText = signerIntroText,
					Expires = signerExpires,
					SendSignRequest = false,
					SendSignConfirmation = false,
					DaysToRemind = 7,
					Language = "en-US",
					SignRequestMessage = "Please sign this document.",
					SignRequestSubject = "Document for Signature",
					ReturnUrl = "https://example.com/return",
					AllowDelegation = false,
					Context = new {
						SignerContext = "test-signer",
					},
					Verifications = [
						new ScribbleVerification {
							RequireHandsignature = true,
							ScribbleName = "John Doe",
							ScribbleNameFixed = true
						}
					],
					Authentications = [
						new PhoneNumberVerification {
							Number = "+31612345678",
							SecureDownload = true,
						}
					]
				}
			],
			Receivers = [
				new Receiver {
					Name = receiverName,
					Email = receiverEmail,
					Language = "en-US",
					Message = "The document has been signed.",
					Subject = "Signed Document",
					Reference = receiverReference,
					Context = new {
						ReceiverContext = "test-receiver",
					}
				}
			]
		};

		var pdfPath = Path.Combine("TestFiles", "small-example-pdf-file.pdf");
		if (!File.Exists(pdfPath)) {
			throw new FileNotFoundException($"Test PDF file not found at: {pdfPath}");
		}

		// Act - Create transaction
		var createdTransaction = await client.CreateTransactionAsync(transaction);

		// Assert - Creation properties
		createdTransaction.Should().NotBeNull();
		createdTransaction.Id.Should().NotBeNullOrEmpty();
		createdTransaction.Status.Should().Be(TransactionStatus.WaitingForDocument);
		createdTransaction.Seal.Should().BeFalse();
		createdTransaction.Reference.Should().Be(testReference);
		createdTransaction.PostbackUrl.Should().Be(testPostbackUrl);
		createdTransaction.DaysToExpire.Should().Be(30);
		createdTransaction.SendEmailNotifications.Should().BeFalse();
		createdTransaction.SignRequestMode.Should().Be(2);
		createdTransaction.Language.Should().Be("en-US");
		createdTransaction.CreatedDateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
		createdTransaction.CanceledDateTime.Should().BeNull();
		createdTransaction.CancellationReason.Should().BeNull();

		// Assert - Context
		((object)createdTransaction.Context).Should().NotBeNull();
		string transactionContextJson = createdTransaction.Context.ToString();
		transactionContextJson.Should().Contain("integration-test");

		// Assert - Signers
		createdTransaction.Signers.Should().HaveCount(1);
		var createdSigner = createdTransaction.Signers[0];
		createdSigner.Id.Should().Be("signer1");
		createdSigner.Email.Should().Be(signerEmail);
		createdSigner.Reference.Should().Be(signerReference);
		createdSigner.IntroText.Should().Be(signerIntroText);
		createdSigner.Expires.Should().HaveValue();
		createdSigner.Expires.Should().BeCloseTo(signerExpires, TimeSpan.FromMinutes(1));
		createdSigner.SendSignRequest.Should().BeFalse();
		createdSigner.SendSignConfirmation.Should().BeFalse();
		createdSigner.DaysToRemind.Should().Be(7);
		createdSigner.Language.Should().Be("en-US");
		createdSigner.SignRequestMessage.Should().Be("Please sign this document.");
		createdSigner.SignRequestSubject.Should().Be("Document for Signature");
		createdSigner.ReturnUrl.Should().Be("https://example.com/return");
		createdSigner.AllowDelegation.Should().BeFalse();
		createdSigner.CreatedDateTime.Should().HaveValue();
		createdSigner.ModifiedDateTime.Should().HaveValue();
		createdSigner.SignedDateTime.Should().BeNull();
		createdSigner.RejectDateTime.Should().BeNull();
		createdSigner.SignerDelegationDateTime.Should().BeNull();
		createdSigner.RejectReason.Should().BeNull();
		createdSigner.Activities.Should().NotBeNull();

		// Assert - Signer Context
		((object)createdSigner.Context).Should().NotBeNull();
		string signerContextJson = createdSigner.Context.ToString();
		signerContextJson.Should().Contain("test-signer");

		// Assert - Signer Verifications
		createdSigner.Verifications.Should().HaveCount(1);
		var verification = createdSigner.Verifications[0].Should().BeOfType<ScribbleVerification>().Subject;
		verification.ScribbleName.Should().Be("John Doe");
		verification.RequireHandsignature.Should().BeTrue();
		verification.ScribbleNameFixed.Should().BeTrue();

		// Assert - Signer Authentications
		createdSigner.Authentications.Should().HaveCount(1);
		var authentication = createdSigner.Authentications[0].Should().BeOfType<PhoneNumberVerification>().Subject;
		authentication.Number.Should().Be("+31612345678");
		authentication.SecureDownload.Should().BeTrue();

		// Assert - Receivers
		createdTransaction.Receivers.Should().HaveCount(1);
		var createdReceiver = createdTransaction.Receivers[0];
		createdReceiver.Name.Should().Be(receiverName);
		createdReceiver.Email.Should().Be(receiverEmail);
		createdReceiver.Language.Should().Be("en-US");
		createdReceiver.Message.Should().Be("The document has been signed.");
		createdReceiver.Subject.Should().Be("Signed Document");
		createdReceiver.Reference.Should().Be(receiverReference);
		createdReceiver.Id.Should().NotBeNullOrEmpty();
		createdReceiver.CreatedDateTime.Should().HaveValue();
		createdReceiver.ModifiedDateTime.Should().HaveValue();
		createdReceiver.Activities.Should().BeNull(
			because: "actual API inconsistency - Receiver Activities are null rather than an empty list");

		// Assert - Receiver Context
		((object)createdReceiver.Context).Should().NotBeNull();
		string receiverContextJson = createdReceiver.Context.ToString();
		receiverContextJson.Should().Contain("test-receiver");

		// Act - Upload file
		await using var fileStream = File.OpenRead(pdfPath);
		await client.AddOrReplaceFileToTransactionAsync(
			fileStream,
			createdTransaction.Id,
			"test-document.pdf",
			new FileUploadOptions());

		// Act - Start transaction
		await client.StartTransactionAsync(createdTransaction.Id);

		// Act - Retrieve final state
		var finalTransaction = await client.GetTransactionAsync(createdTransaction.Id);

		// Assert - Final transaction state
		finalTransaction.Should().NotBeNull();
		finalTransaction.Id.Should().Be(createdTransaction.Id);
		finalTransaction.Status.Should().BeOneOf(
			TransactionStatus.WaitingForSigner,
			TransactionStatus.InProgress);
		finalTransaction.Reference.Should().Be(testReference);
		finalTransaction.PostbackUrl.Should().Be(testPostbackUrl);
		finalTransaction.DaysToExpire.Should().Be(30);
		finalTransaction.SendEmailNotifications.Should().BeFalse();
		finalTransaction.Language.Should().Be("en-US");

		// Assert - Files
		finalTransaction.Files.Should().NotBeNull();
		finalTransaction.Files.Should().ContainKey("test-document.pdf");
		var fileEntry = finalTransaction.Files["test-document.pdf"];
		fileEntry.Should().NotBeNull();
		fileEntry.DisplayName.Should().Be("test-document.pdf");
		fileEntry.Links.Should().NotBeNull().And.NotBeEmpty();

		// Assert - Signer in final state
		finalTransaction.Signers.Should().HaveCount(1);
		var finalSigner = finalTransaction.Signers[0];
		finalSigner.Id.Should().Be("signer1");
		finalSigner.Email.Should().Be(signerEmail);
		finalSigner.Reference.Should().Be(signerReference);
		finalSigner.ModifiedDateTime.Should().HaveValue();
		finalSigner.ModifiedDateTime.Should().BeOnOrAfter(finalSigner.CreatedDateTime.Value);
		finalSigner.Expires.Should().HaveValue();
		finalSigner.SignUrl.Should().NotBeNullOrEmpty();
		finalSigner.ShowUrl.Should().NotBeNullOrEmpty();
		finalSigner.ReceiptUrl.Should().NotBeNullOrEmpty();
		finalSigner.DelegateSignUrl.Should().BeNullOrEmpty();
		finalSigner.DelegateReason.Should().BeNullOrEmpty();
		finalSigner.DelegateSignerEmail.Should().BeNullOrEmpty();
		finalSigner.DelegateSignerName.Should().BeNullOrEmpty();

		// Assert - Receiver in final state
		finalTransaction.Receivers.Should().HaveCount(1);
		var finalReceiver = finalTransaction.Receivers[0];
		finalReceiver.Email.Should().Be(receiverEmail);
		finalReceiver.Name.Should().Be(receiverName);
		finalReceiver.Reference.Should().Be(receiverReference);
	}

	public void Dispose()
	{
		client?.Dispose();
		GC.SuppressFinalize(this);
	}
}
