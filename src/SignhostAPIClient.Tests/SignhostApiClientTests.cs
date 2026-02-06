using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;
using SignhostAPIClient.Tests.JSON;
using Xunit;

namespace Signhost.APIClient.Rest.Tests;

public class SignhostApiClientTests
{
	private readonly SignhostApiClientSettings settings = new("AppKey", "Usertoken") {
		Endpoint = "http://localhost/api/",
	};

	private readonly SignhostApiClientSettings oauthSettings = new("AppKey") {
		Endpoint = "http://localhost/api/",
	};

	[Fact]
	public async Task When_AddOrReplaceFileMetaToTransaction_is_called_Then_the_request_body_should_contain_the_serialized_file_meta()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
			.WithContent(JsonResources.AddOrReplaceFileMetaToTransaction)
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var fileSignerMeta = new FileSignerMeta {
			FormSets = ["SampleFormSet"]
		};

		var field = new Field {
			Type = FileFieldType.Check,
			Value = "I agree",
			Location = new Location { Search = "test" },
		};

		FileMeta fileMeta = new FileMeta {
			Signers = new Dictionary<string, FileSignerMeta> {
				["someSignerId"] = fileSignerMeta,
			},
			FormSets = new Dictionary<string, IDictionary<string, Field>> {
				["SampleFormSet"] = new Dictionary<string, Field> {
					["SampleCheck"] = field,
				},
			},
		};

		await signhostApiClient.AddOrReplaceFileMetaToTransactionAsync(
			fileMeta,
			"transactionId",
			"fileId"
		);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_GetTransaction_is_called_Then_we_should_have_called_the_transaction_get_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.OK, new StringContent(JsonResources.GetTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var result = await signhostApiClient.GetTransactionAsync("transactionId");
		result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_the_authorization_is_bad_Then_we_should_get_a_BadAuthorizationException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "unauthorized"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.Unauthorized, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<BadAuthorizationException>();
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_request_is_bad_Then_we_should_get_a_BadRequestException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "Bad Request"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.BadRequest, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<BadRequestException>();
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_credits_have_run_out_Then_we_should_get_a_OutOfCreditsException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"type": "https://api.signhost.com/problem/subscription/out-of-credits"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.PaymentRequired, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<OutOfCreditsException>();
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_not_found_Then_we_should_get_a_NotFoundException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "Not Found"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.NotFound, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");

		var exception = await getTransaction.Should().ThrowAsync<NotFoundException>();
		exception.Which.Message.Should().Be("Not Found");
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_unkownerror_like_418_occures_Then_we_should_get_a_SignhostException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "418 I'm a teapot"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond((HttpStatusCode)418, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<SignhostRestApiClientException>();
		exception.Which.Message.Should().ContainAll("418");
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_there_is_an_InternalServerError_Then_we_should_get_a_InternalServerErrorException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "Internal Server Error"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.InternalServerError, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<InternalServerErrorException>();
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_on_gone_transaction_we_shoud_get_a_GoneException()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.Gone, new StringContent(JsonResources.GetTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<GoneException<Transaction>>();
		exception.Which.ResponseBody.Should().Be(JsonResources.GetTransaction);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetTransaction_is_called_and_gone_is_expected_we_should_get_a_transaction()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.Gone, new StringContent(JsonResources.GetTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionResponseAsync("transactionId");
		await getTransaction.Should().NotThrowAsync();

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_CreateTransaction_is_called_Then_we_should_have_called_the_transaction_Post_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Post, "http://localhost/api/transaction")
			.Respond(HttpStatusCode.OK, new StringContent(JsonResources.AddTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		CreateSignerRequest testSigner = new() {
			Email = "firstname.lastname@gmail.com",
		};

		CreateTransactionRequest testTransaction = new();
		testTransaction.Signers.Add(testSigner);

		var result = await signhostApiClient.CreateTransactionAsync(testTransaction);
		result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_CreateTransaction_is_called_we_can_add_custom_http_headers()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Post, "http://localhost/api/transaction")
			.WithHeaders("X-Forwarded-For", "localhost")
			.With(matcher => matcher.Headers.UserAgent.ToString().Contains("SignhostClientLibrary"))
			.Respond(HttpStatusCode.OK, new StringContent(JsonResources.AddTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		settings.AddHeader = ah => ah("X-Forwarded-For", "localhost");

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		CreateTransactionRequest testTransaction = new();

		var result = await signhostApiClient.CreateTransactionAsync(testTransaction);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_CreateTransaction_is_called_with_invalid_email_Then_we_should_get_a_BadRequestException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "Bad Request"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Post, "http://localhost/api/transaction")
			.WithHeaders("Content-Type", "application/json")
			.Respond(HttpStatusCode.BadRequest, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		CreateSignerRequest testSigner = new() {
			Email = "firstname.lastnamegmail.com",
		};

		CreateTransactionRequest testTransaction = new();
		testTransaction.Signers.Add(testSigner);

		Func<Task> getTransaction = () => signhostApiClient.CreateTransactionAsync(testTransaction);
		var exception = await getTransaction.Should().ThrowAsync<BadRequestException>();
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_function_is_called_with_a_wrong_endpoint_we_should_get_a_SignhostRestApiClientException()
	{
		MockHttpMessageHandler mockHttp = new();
		const string expectedResponseBody = """
			{
				"message": "Bad Gateway"
			}
		""";
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.BadGateway, new StringContent(expectedResponseBody));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
		var exception = await getTransaction.Should().ThrowAsync<SignhostRestApiClientException>();
		exception.Which.Message.Should().Be("Bad Gateway");
		exception.Which.ResponseBody.Should().Be(expectedResponseBody);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_DeleteTransaction_is_called_Then_we_should_have_called_the_transaction_delete_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Delete, "http://localhost/api/transaction/transactionId")
			.Respond(HttpStatusCode.OK, new StringContent(JsonResources.DeleteTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.DeleteTransactionAsync("transactionId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_DeleteTransaction_with_notification_is_called_Then_we_should_have_called_the_transaction_delete_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Delete, "http://localhost/api/transaction/transactionId")
			.WithHeaders("Content-Type", "application/json")
			//.With(matcher => matcher.Content.ToString().Contains("'SendNotifications': true"))
			.Respond(HttpStatusCode.OK, new StringContent(JsonResources.DeleteTransaction));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.DeleteTransactionAsync(
			"transactionId",
			new DeleteTransactionOptions { SendNotifications = true });

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_AddOrReplaceFileToTransaction_is_called_Then_we_should_have_called_the_file_put_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
			.WithHeaders("Content-Type", "application/pdf")
			.WithHeaders("Digest", "SHA-256=47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		// Create a 0 sized file
		using Stream file = File.Create("unittestdocument.pdf");
		await signhostApiClient.AddOrReplaceFileToTransactionAsync(
			file,
			"transactionId",
			"fileId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_AddOrReplaceFileToTransaction_is_called_default_digest_is_sha256()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
			.WithHeaders("Digest", "SHA-256=47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.AddOrReplaceFileToTransactionAsync(
			new MemoryStream(),
			"transactionId",
			"fileId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_AddOrReplaceFileToTransaction_with_sha512_is_called_default_digest_is_sha512()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
			.WithHeaders(
				"Digest",
				"SHA-512=z4PhNX7vuL3xVChQ1m2AB9Yg5AULVxXcg/SpIdNs6c5H0NE8XYXysP+DGNKHfuwvY7kxvUdBeoGlODJ6+SfaPg=="
			)
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.AddOrReplaceFileToTransactionAsync(
			new MemoryStream(),
			"transactionId",
			"fileId",
			new FileUploadOptions {
				DigestOptions = new FileDigestOptions {
					DigestHashAlgorithm = DigestHashAlgorithm.SHA512,
				},
			}
		);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_AddOrReplaceFileToTransaction_with_digest_value_is_used_as_is()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
			.WithHeaders("Digest", "SHA-256=AAEC")
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.AddOrReplaceFileToTransactionAsync(
			new MemoryStream(),
			"transactionId",
			"fileId",
			new FileUploadOptions {
				DigestOptions = new FileDigestOptions {
					DigestHashAlgorithm = DigestHashAlgorithm.SHA256,
					DigestHashValue = [0x00, 0x01, 0x02],
				},
			}
		);

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_StartTransaction_is_called_Then_we_should_have_called_the_transaction_put_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp.Expect("http://localhost/api/transaction/transactionId/start")
			.Respond(HttpStatusCode.NoContent);

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		await signhostApiClient.StartTransactionAsync("transactionId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetReceipt_is_called_Then_we_should_have_called_the_filereceipt_get_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp.Expect("http://localhost/api/file/receipt/transactionId")
			.Respond(HttpStatusCode.OK);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var receipt = await signhostApiClient.GetReceiptAsync("transactionId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_GetDocument_is_called_Then_we_should_have_called_the_file_get_once()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp.Expect(HttpMethod.Get, "http://localhost/api/transaction/*/file/fileId")
			.Respond(HttpStatusCode.OK, new StringContent(string.Empty));

		using var httpClient = mockHttp.ToHttpClient();

		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var document = await signhostApiClient.GetDocumentAsync("transactionId", "fileId");

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Fact]
	public async Task When_a_transaction_json_is_returned_it_is_deserialized_correctly()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Post, "http://localhost/api/transaction")
			.Respond(
				HttpStatusCode.OK,
				new StringContent(JsonResources.TransactionSingleSignerJson)
			);

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var result = await signhostApiClient.CreateTransactionAsync(new() {
			Signers = [
				new() {
						Email = "test@example.com",
						Verifications = [
							new PhoneNumberVerification {
								Number = "31615087075"
							}
						]
					}
			]
		});

		result.Id.Should().Be("50262c3f-9744-45bf-a4c6-8a3whatever");
		result.CanceledDateTime.Should().HaveYear(2017);
		result.Status.Should().Be(TransactionStatus.WaitingForDocument);
		result.Signers.Should().HaveCount(1);
		result.Receivers.Should().HaveCount(0);
		result.Reference.Should().Be("Contract #123");
		result.SignRequestMode.Should().Be(2);
		result.DaysToExpire.Should().Be(14);
		result.Signers[0].Id.Should().Be("Signer1");
		result.Signers[0].Email.Should().Be("test1@example.com");
		result.Signers[0].Verifications.Should().HaveCount(1);
		result.Signers[0].Verifications[0].Should().BeOfType<PhoneNumberVerification>()
			  .And.Subject.Should().BeEquivalentTo(new PhoneNumberVerification {
				  Number = "+31615123456"
			  });
		result.Signers[0].Activities.Should().HaveCount(3);
		result.Signers[0].Activities[0].Should().BeEquivalentTo(new Activity {
			Id = "Activity1",
			Code = ActivityType.Opened,
			CreatedDateTime = DateTimeOffset.Parse("2017-05-31T22:15:17.6409005+02:00")
		});

		mockHttp.VerifyNoOutstandingExpectation();
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task When_a_complete_transaction_flow_is_created_headers_are_not_set_multiple_times(
		bool isOauth)
	{
		MockedRequest AddHeaders(MockedRequest request)
		{
			if (!isOauth) {
				request = request.WithHeaders("Authorization", "APIKey Usertoken");
			}

			return request
				.WithHeaders("Application", "APPKey AppKey")
				.WithHeaders("X-Custom", "test");
		}

		MockHttpMessageHandler mockHttp = new();
		AddHeaders(mockHttp.Expect(HttpMethod.Post, "http://localhost/api/transaction"))
			.Respond(new StringContent(JsonResources.TransactionSingleSignerJson));
		AddHeaders(mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/file/somefileid"))
			.Respond(HttpStatusCode.Accepted, new StringContent(JsonResources.AddOrReplaceFileMetaToTransaction));
		AddHeaders(mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/file/somefileid"))
			.Respond(HttpStatusCode.Created);
		AddHeaders(mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/start"))
			.Respond(HttpStatusCode.NoContent);

		using var httpClient = mockHttp.ToHttpClient();
		var clientSettings = isOauth ? oauthSettings : settings;
		clientSettings.AddHeader = add => add("X-Custom", "test");
		SignhostApiClient signhostApiClient = new(clientSettings, httpClient);

		var result = await signhostApiClient
			.CreateTransactionAsync(new CreateTransactionRequest());
		await signhostApiClient
			.AddOrReplaceFileMetaToTransactionAsync(new FileMeta(), result.Id, "somefileid");

		using Stream file = File.Create("unittestdocument.pdf");
		await signhostApiClient.AddOrReplaceFileToTransactionAsync(file, result.Id, "somefileid");
		await signhostApiClient.StartTransactionAsync(result.Id);

		mockHttp.VerifyNoOutstandingExpectation();
		mockHttp.VerifyNoOutstandingRequest();
	}

	[Fact]
	public async Task When_a_minimal_response_is_retrieved_list_and_dictionaries_are_not_null()
	{
		MockHttpMessageHandler mockHttp = new();
		mockHttp
			.Expect(HttpMethod.Get, "http://localhost/api/transaction/c487be92-0255-40c7-bd7d-20805a65e7d9")
			.Respond(new StringContent(JsonResources.MinimalTransactionResponse));

		using var httpClient = mockHttp.ToHttpClient();
		SignhostApiClient signhostApiClient = new(settings, httpClient);

		var result = await signhostApiClient.GetTransactionAsync("c487be92-0255-40c7-bd7d-20805a65e7d9");

		result.Signers.Should().BeEmpty();
		result.Receivers.Should().BeEmpty();
		result.Files.Should().BeEmpty();
	}
}
