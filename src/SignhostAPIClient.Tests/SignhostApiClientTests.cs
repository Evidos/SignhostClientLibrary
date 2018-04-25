using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Xunit;
using Signhost.APIClient.Rest.DataObjects;
using FluentAssertions;
using System.Collections.Generic;
using RichardSzalay.MockHttp;
using System.Net;

namespace Signhost.APIClient.Rest.Tests
{
	public class SignHostApiClientTests
	{
		private SignHostApiClientSettings settings = new SignHostApiClientSettings(
				"AppKey",
				"AuthKey"
		) {
			Endpoint = "http://localhost/api/"
		};
		
		[Fact]
		public async void when_AddOrReplaceFileMetaToTransaction_is_called_then_the_request_body_should_contain_the_serialized_file_meta()
		{
			var mockHttp = new MockHttpMessageHandler();

			mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/transactionId/file/fileId")
				.WithContent(RequestBodies.AddOrReplaceFileMetaToTransaction)
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var fileSignerMeta = new FileSignerMeta
				{
					FormSets = new string[] { "SampleFormSet" }
				};

				var field = new Field
				{
					Type = "Check",
					Value = "I agree",
					Location = new Location
					{
						Search = "test"
					}
				};

				FileMeta fileMeta = new FileMeta
				{
					Signers = new Dictionary<string, FileSignerMeta>
					{
						{ "someSignerId", fileSignerMeta }
					},
					FormSets = new Dictionary<string, IDictionary<string, Field>>
					{
						{ "SampleFormSet", new Dictionary<string, Field>
							{
								{ "SampleCheck", field }
							}
						}
					}
				};

				await signhostApiClient.AddOrReplaceFileMetaToTransactionAsync(fileMeta, "transactionId", "fileId");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_a_GetTransaction_is_called_then_we_should_have_called_the_transaction_get_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.OK, new StringContent(APIResponses.GetTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var result = await signhostApiClient.GetTransactionAsync("transaction Id");
				result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_GetTransaction_is_called_and_the_authorization_is_bad_then_we_should_get_a_BadAuthorizationException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.Unauthorized, new StringContent("{'message': 'unauthorized' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<UnauthorizedAccessException>();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_GetTransaction_is_called_and_request_is_bad_then_we_should_get_a_BadRequestException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.BadRequest, new StringContent("{ 'message': 'Bad Request' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<ErrorHandling.BadRequestException>();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_GetTransaction_is_called_and_not_found_then_we_should_get_a_NotFoundException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.NotFound, new StringContent("{ 'Message': 'Not Found' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");

				getTransaction.Should().Throw<ErrorHandling.NotFoundException>().WithMessage("Not Found");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_GetTransaction_is_called_and_unkownerror_like_418_occures_then_we_should_get_a_SignhostException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond((HttpStatusCode)418, new StringContent("{ 'message': '418 I\\'m a teapot' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<ErrorHandling.SignhostRestApiClientException>()
					.WithMessage("*418*");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_GetTransaction_is_called_and_there_is_an_InternalServerError_then_we_should_get_a_InternalServerErrorException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.InternalServerError, new StringContent("{ 'message': 'Internal Server Error' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<ErrorHandling.InternalServerErrorException>();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_on_gone_transaction_we_shoud_get_a_GoneException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.Gone, new StringContent(APIResponses.GetTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<ErrorHandling.GoneException<Transaction>>();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_gone_is_expected_we_should_get_a_transaction()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.Gone, new StringContent(APIResponses.GetTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionResponseAsync("transaction Id");
				getTransaction.Should().NotThrow();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_a_CreateTransaction_is_called_then_we_should_have_called_the_transaction_Post_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.Respond(HttpStatusCode.OK, new StringContent(APIResponses.AddTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Signer testSigner = new Signer();
				testSigner.Email = "firstname.lastname@gmail.com";

				Transaction testTransaction = new Transaction();
				testTransaction.Signers.Add(testSigner);

				var result = await signhostApiClient.CreateTransactionAsync(testTransaction);
				result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_a_CreateTransaction_is_called_we_can_add_custom_http_headers()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.WithHeaders("X-Forwarded-For", "localhost")
				.With(matcher => matcher.Headers.UserAgent.ToString().Contains("SignhostClientLibrary"))
				.Respond(HttpStatusCode.OK, new StringContent(APIResponses.AddTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				settings.AddHeader = (AddHeaders a) => a("X-Forwarded-For", "localhost");

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Transaction testTransaction = new Transaction();

				var result = await signhostApiClient.CreateTransactionAsync(testTransaction);
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_CreateTransaction_is_called_with_invalid_email_then_we_should_get_a_BadRequestException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.WithHeaders("Content-Type", "application/json")
				.Respond(HttpStatusCode.BadRequest, new StringContent(" { 'message': 'Bad Request' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Signer testSigner = new Signer();
				testSigner.Email = "firstname.lastnamegmail.com";

				Transaction testTransaction = new Transaction();
				testTransaction.Signers.Add(testSigner);

				Func<Task> getTransaction = () => signhostApiClient.CreateTransactionAsync(testTransaction);
				getTransaction.Should().Throw<ErrorHandling.BadRequestException>();
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void when_a_function_is_called_with_a_wrong_endpoint_we_should_get_a_SignhostRestApiClientException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.BadGateway, new StringContent(" { 'Message': 'Bad Gateway' }"));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transaction Id");
				getTransaction.Should().Throw<ErrorHandling.SignhostRestApiClientException>()
					.WithMessage("Bad Gateway");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_a_DeleteTransaction_is_called_then_we_should_have_called_the_transaction_delete_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Delete, "http://localhost/api/transaction/transaction Id")
				.Respond(HttpStatusCode.OK, new StringContent(APIResponses.DeleteTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.DeleteTransactionAsync("transaction Id");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_a_DeleteTransaction_with_notification_is_called_then_we_should_have_called_the_transaction_delete_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Delete, "http://localhost/api/transaction/transaction Id")
				.WithHeaders("Content-Type", "application/json")
				//.With(matcher => matcher.Content.ToString().Contains("'SendNotifications': true"))
				.Respond(HttpStatusCode.OK, new StringContent(APIResponses.DeleteTransaction));

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.DeleteTransactionAsync(
					"transaction Id",
					new DeleteTransactionOptions { SendNotifications = true });
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_AddOrReplaceFileToTransaction_is_called_then_we_should_have_called_the_file_put_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Put, "http://localhost/api/transaction/transaction Id/file/file Id")
				.WithHeaders("Content-Type", "application/pdf")
				.WithHeaders("Digest", "SHA-256=47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				// Create a 0 sized file
				using (Stream file = System.IO.File.Create("unittestdocument.pdf")) {
					await signhostApiClient.AddOrReplaceFileToTransaction(file, "transaction Id", "file Id");
				}
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_AddOrReplaceFileToTransaction_is_called_default_digest_is_sha256()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Put, "http://localhost/api/transaction/transaction Id/file/file Id")
				.WithHeaders("Digest", "SHA-256=47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.AddOrReplaceFileToTransaction(new MemoryStream(), "transaction Id", "file Id");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_AddOrReplaceFileToTransaction_with_sha512_is_called_default_digest_is_sha512()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Put, "http://localhost/api/transaction/transaction Id/file/file Id")
				.WithHeaders("Digest", "SHA-512=z4PhNX7vuL3xVChQ1m2AB9Yg5AULVxXcg/SpIdNs6c5H0NE8XYXysP+DGNKHfuwvY7kxvUdBeoGlODJ6+SfaPg==")
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.AddOrReplaceFileToTransactionAsync(
					new MemoryStream(),
					"transaction Id",
					"file Id",
					new FileUploadOptions{
					DigestOptions = new FileDigestOptions
					{
						DigestHashAlgorithm = "SHA-512"
					}
				});
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_AddOrReplaceFileToTransaction_with_digest_value_is_used_as_is()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Put, "http://localhost/api/transaction/transaction Id/file/file Id")
				.WithHeaders("Digest", "SHA-1=AAEC")
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.AddOrReplaceFileToTransactionAsync(
					new MemoryStream(),
					"transaction Id",
					"file Id",
					new FileUploadOptions
					{
						DigestOptions = new FileDigestOptions
						{
							DigestHashAlgorithm = "SHA-1",
							DigestHashValue = new byte[] { 0x00, 0x01, 0x02 }
						}
					});
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_StartTransaction_is_called_then_we_should_have_called_the_transaction_put_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect("http://localhost/api/transaction/transaction Id/start")
				.Respond(HttpStatusCode.NoContent);

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				await signhostApiClient.StartTransactionAsync("transaction Id");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_GetReceipt_is_called_then_we_should_have_called_the_filereceipt_get_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect("http://localhost/api/file/receipt/transaction ID")
				.Respond(HttpStatusCode.OK);

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var receipt = await signhostApiClient.GetReceiptAsync("transaction ID");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task when_GetDocument_is_called_then_we_should_have_called_the_file_get_once()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, "http://localhost/api/transaction/*/file/file Id")
				.Respond(HttpStatusCode.OK, new StringContent(string.Empty));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var document = await signhostApiClient.GetDocumentAsync("transaction Id", "file Id");
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task When_a_transaction_json_is_returned_it_is_deserialized_correctly()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.Respond(HttpStatusCode.OK, new StringContent(RequestBodies.TransactionSingleSignerJson));

			using (var httpClient = mockHttp.ToHttpClient()) {

				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var result = await signhostApiClient.CreateTransactionAsync(new Transaction
				{
					Signers = new List<Signer>{
						new Signer
						{
							Verifications = new List<IVerification>
							{
								new PhoneNumberVerification
								{
									Number = "31615087075"
								}
							}
						}
					}
				});

				result.Id.Should().Be("50262c3f-9744-45bf-a4c6-8a3whatever");
				result.CancelledDateTime.Should().HaveYear(2017);
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
				result.Signers[0].Activities[0].Should().BeEquivalentTo(new Activity
				{
					Id = "Activity1",
					Code = ActivityType.Opened,
					CreatedDateTime = DateTimeOffset.Parse("2017-05-31T22:15:17.6409005+02:00")
				});
			}

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public async Task When_a_complete_transaction_flow_is_created_headers_are_not_set_multiple_times()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.WithHeaders("Application", "APPKey AppKey")
				.WithHeaders("Authorization", "APIKey AuthKey")
				.WithHeaders("X-Custom", "test")
				.Respond(new StringContent(RequestBodies.TransactionSingleSignerJson));
			mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/file/somefileid")
				.WithHeaders("Application", "APPKey AppKey")
				.WithHeaders("Authorization", "APIKey AuthKey")
				.WithHeaders("X-Custom", "test")
				.Respond(HttpStatusCode.Accepted, new StringContent(RequestBodies.AddOrReplaceFileMetaToTransaction));
			mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/file/somefileid")
				.WithHeaders("Application", "APPKey AppKey")
				.WithHeaders("Authorization", "APIKey AuthKey")
				.WithHeaders("X-Custom", "test")
				.Respond(HttpStatusCode.Created);
			mockHttp.Expect(HttpMethod.Put, "http://localhost/api/transaction/*/start")
				.WithHeaders("Application", "APPKey AppKey")
				.WithHeaders("Authorization", "APIKey AuthKey")
				.WithHeaders("X-Custom", "test")
				.Respond(HttpStatusCode.NoContent);

			using (var httpClient = mockHttp.ToHttpClient()) {

				settings.AddHeader = add => add("X-Custom", "test");
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var result = await signhostApiClient.CreateTransactionAsync(new Transaction());
				await signhostApiClient.AddOrReplaceFileMetaToTransactionAsync(new FileMeta(), result.Id, "somefileid");
				using (Stream file = System.IO.File.Create("unittestdocument.pdf")) {
					await signhostApiClient.AddOrReplaceFileToTransaction(file, result.Id, "somefileid");
				}
				await signhostApiClient.StartTransactionAsync(result.Id);
			}

			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task When_a_custom_verificationtype_is_provided_it_is_deserialized_correctly()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/c487be92-0255-40c7-bd7d-20805a65e7d9")
				.Respond(new StringContent(APIResponses.GetTransactionCustomVerificationType));

			SignHostApiClient.RegisterVerification<CustomVerification>();

			using (var httpClient = mockHttp.ToHttpClient()) {
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var result = await signhostApiClient.GetTransactionAsync("c487be92-0255-40c7-bd7d-20805a65e7d9");

				result.Signers[0].Verifications.Should().HaveCount(3);
				result.Signers[0].Verifications[0].Should().BeOfType<CustomVerification>();
				result.Signers[0].Verifications[1].Should().BeOfType<IPAddressVerification>()
					.Which.IPAddress.Should().Be("127.0.0.33");
				result.Signers[0].Verifications[2].Should().BeOfType<PhoneNumberVerification>()
					.Which.Number.Should().Be("123");
			}
		}

		[Fact]
		public async Task When_a_minimal_response_is_retrieved_list_and_dictionaries_are_not_null()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/c487be92-0255-40c7-bd7d-20805a65e7d9")
				.Respond(new StringContent(APIResponses.MinimalTransactionResponse));

			SignHostApiClient.RegisterVerification<CustomVerification>();

			using (var httpClient = mockHttp.ToHttpClient())
			{
				var signhostApiClient = new SignHostApiClient(settings, httpClient);

				var result = await signhostApiClient.GetTransactionAsync("c487be92-0255-40c7-bd7d-20805a65e7d9");

				result.Signers.Should().BeEmpty();
				result.Receivers.Should().BeEmpty();
				result.Files.Should().BeEmpty();
			}
		}

		public class CustomVerification
			: IVerification
		{
			public string Type => "CustomVerificationType";
		}
	}
}
