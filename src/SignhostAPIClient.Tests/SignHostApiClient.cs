using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Flurl.Http.Testing;
using Xunit;
using Signhost.APIClient.Rest.DataObjects;
using FluentAssertions;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.Tests
{
	public class SignHostApiClientTests
	{
		private SignHostApiClientSettings settings = new SignHostApiClientSettings(
				"AppKey",
				"AuthKey");

		[Fact]
		public async void when_AddOrReplaceFileMetaToTransaction_is_called_then_the_request_body_should_contain_the_serialized_file_meta()
		{
			using (HttpTest httpTest = new HttpTest()) {
				var signhostApiClient = new SignHostApiClient(settings);

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

				await signhostApiClient.AddOrReplaceFileMetaToTransaction(fileMeta, "transactionId", "fileId");

				httpTest.CallLog[0].RequestBody.Should().Contain(RequestBodies.AddOrReplaceFileMetaToTransaction);
			}
		}

		[Fact]
		public async Task when_a_GetTransaction_is_called_then_we_should_have_called_the_transaction_get_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, APIResponses.GetTransaction);

				var signhostApiClient = new SignHostApiClient(settings);

				var result = await signhostApiClient.GetTransaction("transaction Id");
				result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*")
					.WithVerb(HttpMethod.Get)
					.Times(1);
			}
		}

		[Fact]
		public void when_GetTransaction_is_called_and_the_authorization_is_bad_then_we_should_get_a_BadAuthorizationException()
		{
			using (HttpTest httpTest = new HttpTest()) {

				httpTest.RespondWithJson(401, new { message = "unauthorized" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<UnauthorizedAccessException>();
			}
		}

		[Fact]
		public void when_GetTransaction_is_called_and_request_is_bad_then_we_should_get_a_BadRequestException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(400, new { message = "Bad Request" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.BadRequestException>();
			}
		}

		[Fact]
		public void when_GetTransaction_is_called_and_not_found_then_we_should_get_a_NotFoundException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(404, new { message = "Not Found" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.NotFoundException>();
			}
		}

		[Fact]
		public void when_GetTransaction_is_called_and_unkownerror_like_418_occures_then_we_should_get_a_SignhostException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(418, new { message = "418 I'm a teapot" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.SignhostRestApiClientException>()
					.WithMessage("*418*");
			}
		}

		[Fact]
		public void when_GetTransaction_is_called_and_there_is_an_InternalServerError_then_we_should_get_a_InternalServerErrorException()
		{
			using (HttpTest httpTest = new HttpTest())
			{

				httpTest.RespondWithJson(500, new { message = "Internal Server Error" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.InternalServerErrorException>();
			}
		}

		[Fact]
		public async Task when_a_CreateTransaction_is_called_then_we_should_have_called_the_transaction_Post_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, APIResponses.AddTransaction);

				var signhostApiClient = new SignHostApiClient(settings);

				Signer testSigner = new Signer();
				testSigner.Email = "firstname.lastname@gmail.com";

				Transaction testTransaction = new Transaction();
				testTransaction.Signers.Add(testSigner);

				var result = await signhostApiClient.CreateTransaction(testTransaction);
				result.Id.Should().Be("c487be92-0255-40c7-bd7d-20805a65e7d9");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction")
					.WithVerb(HttpMethod.Post)
					.WithContentType("application/json")
					.Times(1);
			}
		}

		[Fact]
		public void when_CreateTransaction_is_called_with_invalid_email_then_we_should_get_a_BadRequestException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(400, new { message = "Bad Request" });

				var signhostApiClient = new SignHostApiClient(settings);

				Signer testSigner = new Signer();
				testSigner.Email = "firstname.lastnamegmail.com";

				Transaction testTransaction = new Transaction();
				testTransaction.Signers.Add(testSigner);

				Func<Task> getTransaction = () => signhostApiClient.CreateTransaction(testTransaction);
				getTransaction.ShouldThrow<ErrorHandling.BadRequestException>();

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction")
					.WithVerb(HttpMethod.Post)
					.WithContentType("application/json")
					.Times(1);
			}
		}

		[Fact]
		public void when_a_function_is_called_with_a_wrong_endpoint_we_should_get_a_SignhostRestApiClientException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(502, new { message = "Bad Gateway" });

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.SignhostRestApiClientException>();
			}
		}

		[Fact]
		public void when_a_DeleteTransaction_is_called_then_we_should_have_called_the_transaction_delete_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, APIResponses.DeleteTransaction);

				var signhostApiClient = new SignHostApiClient(settings);

				signhostApiClient.DeleteTransaction("transaction Id");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*")
					.WithVerb(HttpMethod.Delete)
					.Times(1);
			}
		}

		[Fact]
		public async Task when_AddOrReplaceFileToTransaction_is_called_then_we_should_have_called_the_file_put_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, string.Empty);

				var signhostApiClient = new SignHostApiClient(settings);

				using (Stream file = System.IO.File.Create("unittestdocument.pdf"))
				{
					await signhostApiClient.AddOrReplaceFileToTransaction(file, "transaction Id", "file Id");
				}

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*/file/*")
					.WithVerb(HttpMethod.Put)
					.WithContentType("application/pdf")
					.Times(1);
			}
		}

		[Fact]
		public void when_StartTransaction_is_called_then_we_should_have_called_the_transaction_put_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, string.Empty);

				var signhostApiClient = new SignHostApiClient(settings);

				signhostApiClient.StartTransaction("transaction Id");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*/start")
					.WithVerb(HttpMethod.Put)
					.Times(1);
			}
		}

		[Fact]
		public void when_GetReceipt_is_called_then_we_should_have_called_the_filereceipt_get_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, string.Empty);

				var signhostApiClient = new SignHostApiClient(settings);

				var receipt = signhostApiClient.GetReceipt("transaction ID");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}file/receipt/*")
					.WithVerb(HttpMethod.Get)
					.Times(1);
			}
		}

		[Fact]
		public void when_GetDocument_is_called_then_we_should_have_called_the_file_get_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(200, string.Empty);

				var signhostApiClient = new SignHostApiClient(settings);

				var document = signhostApiClient.GetDocument("transaction Id", "file Id");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*/file/*")
					.WithVerb(HttpMethod.Get)
					.Times(1);
			}
		}
	}
}
