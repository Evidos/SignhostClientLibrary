using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
			throw new InvalidOperationException("Integration tests are not configured.");
		}

		SignhostApiClientSettings settings = new(config.AppKey, config.UserToken) {
			Endpoint = config.ApiBaseUrl,
		};

		client = new(settings);
	}

	[Fact]
	public async Task Given_complex_transaction_When_created_and_started_Then_all_properties_are_correctly_persisted()
	{
		// Arrange
		string testReference = $"IntegrationTest-{DateTime.UtcNow:yyyyMMddHHmmss}";
		string testPostbackUrl = "https://example.com/postback";
		string signerEmail = "john.doe@example.com";
		string signerReference = "SIGNER-001";
		string signerIntroText = "Please review and sign this document carefully.";
		var signerExpires = DateTimeOffset.UtcNow.AddDays(15);
		string receiverEmail = "receiver@example.com";
		string receiverName = "Jane Receiver";
		string receiverReference = "RECEIVER-001";

		CreateTransactionRequest transaction = new() {
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
				new() {
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
				new() {
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

		string pdfPath = Path.Combine("TestFiles", "small-example-pdf-file.pdf");
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
		createdTransaction.CreatedDateTime.Should()
			.BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
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
		createdSigner.Expires.Should()
			.BeCloseTo(signerExpires, TimeSpan.FromMinutes(1));
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
		var verification = createdSigner.Verifications[0]
			.Should().BeOfType<ScribbleVerification>().Subject;
		verification.ScribbleName.Should().Be("John Doe");
		verification.RequireHandsignature.Should().BeTrue();
		verification.ScribbleNameFixed.Should().BeTrue();

		// Assert - Signer Authentications
		createdSigner.Authentications.Should().HaveCount(1);
		var authentication = createdSigner.Authentications[0]
			.Should().BeOfType<PhoneNumberVerification>().Subject;
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

	[Fact]
	public async Task Given_complex_file_metadata_When_added_to_transaction_Then_all_properties_are_accepted()
	{
		// Arrange - Create a simple transaction
		string testReference = $"MetadataTest-{DateTime.UtcNow:yyyyMMddHHmmss}";
		CreateTransactionRequest transaction = new() {
			Reference = testReference,
			SendEmailNotifications = false,
			Signers = [
				new() {
					Id = "signer1",
					Email = "test@example.com",
					SendSignRequest = false,
				},
				new() {
					Id = "signer2",
					Email = "test2@example.com",
					SendSignRequest = false,
				}
			]
		};

		var createdTransaction = await client.CreateTransactionAsync(transaction);
		createdTransaction.Should().NotBeNull();

		// Arrange - Create a very complex FileMeta
		FileMeta fileMeta = new() {
			DisplayOrder = 1,
			DisplayName = "Complex Test Document",
			Description = "This is a test document with complex metadata",
			SetParaph = true,
			Signers = new Dictionary<string, FileSignerMeta> {
				["signer1"] = new() {
					FormSets = ["FormSet1", "FormSet2"],
				},
				["signer2"] = new() {
					FormSets = ["FormSet2", "FormSet3"],
				},
			},
			FormSets = new Dictionary<string, IDictionary<string, Field>> {
				["FormSet1"] = new Dictionary<string, Field> {
					// Test all FileFieldType enum values
					["SealField"] = new Field {
						Type = FileFieldType.Seal,
						Value = null,
						Location = new Location {
							Search = "seal_placeholder",
							Occurence = 1,
							PageNumber = 1,
						},
					},
					["SignatureField"] = new Field {
						Type = FileFieldType.Signature,
						Value = null,
						Location = new Location {
							Top = 100,
							Left = 50,
							Width = 200,
							Height = 60,
							PageNumber = 1,
						},
					},
					["CheckField"] = new Field {
						Type = FileFieldType.Check,
						Value = "I agree to the terms",
						Location = new Location {
							Search = "checkbox_location",
							Occurence = 1,
						},
					},
				},
				["FormSet2"] = new Dictionary<string, Field> {
					// Test different value types: string, number, boolean, null
					["RadioFieldString"] = new Field {
						Type = FileFieldType.Radio,
						Value = "Option A",
						Location = new Location {
							Top = 200,
							Left = 100,
							Right = 300,
							Bottom = 220,
							PageNumber = 2,
						},
					},
					["SingleLineFieldString"] = new Field {
						Type = FileFieldType.SingleLine,
						Value = "John Doe",
						Location = new Location {
							Search = "name_field",
							Width = 150,
							Height = 20,
						}
					},
					["NumberFieldInteger"] = new Field {
						Type = FileFieldType.Number,
						Value = 42,
						Location = new Location {
							Top = 300,
							Left = 50,
							PageNumber = 2,
						},
					},
					["NumberFieldDecimal"] = new Field {
						Type = FileFieldType.Number,
						Value = 123.45,
						Location = new Location {
							Top = 320,
							Left = 50,
							Width = 100,
							Height = 20,
							PageNumber = 2,
						},
					},
					["DateField"] = new Field {
						Type = FileFieldType.Date,
						Value = "2025-11-28",
						Location = new Location {
							Search = "date_placeholder",
							Occurence = 2,
							PageNumber = 3,
						},
					},
				},
				["FormSet3"] = new Dictionary<string, Field> {
					// Test boolean values and all Location properties
					["CheckFieldTrue"] = new Field {
						Type = FileFieldType.Check,
						Value = true,
						Location = new Location {
							Top = 400,
							Right = 200,
							Bottom = 420,
							Left = 50,
							PageNumber = 3,
						},
					},
					["CheckFieldFalse"] = new Field {
						Type = FileFieldType.Check,
						Value = false,
						Location = new Location {
							Top = 450,
							Right = 200,
							Bottom = 470,
							Left = 50,
							Width = 150,
							Height = 20,
							PageNumber = 3,
						},
					},
					["SingleLineFieldNull"] = new Field {
						Type = FileFieldType.SingleLine,
						Value = null,
						Location = new Location {
							Search = "optional_field",
							Occurence = 1,
							Top = 500,
							Left = 50,
							Width = 200,
							Height = 25,
							PageNumber = 4,
						},
					},
					["RadioFieldNumber"] = new Field {
						Type = FileFieldType.Radio,
						Value = 1,
						Location = new Location {
							PageNumber = 4,
							Top = 550,
							Left = 50,
						},
					},
				},
			},
		};

		// Act
		Func<Task> act = () => client.AddOrReplaceFileMetaToTransactionAsync(
			fileMeta,
			createdTransaction.Id,
			"test-document.pdf");

		// Assert
		await act.Should().NotThrowAsync();
	}

	public void Dispose()
	{
		client?.Dispose();
		GC.SuppressFinalize(this);
	}
}
