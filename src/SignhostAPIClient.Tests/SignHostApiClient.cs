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
				httpTest.RespondWith(APIResponses.GetTransaction, 200);

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

				httpTest.RespondWithJson(new { message = "unauthorized" }, 401);

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
				httpTest.RespondWithJson(new { message = "Bad Request" }, 400);

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
				httpTest.RespondWithJson(new { message = "Not Found" }, 404);

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
				httpTest.RespondWithJson(new { message = "418 I'm a teapot" }, 418);

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

				httpTest.RespondWithJson(new { message = "Internal Server Error" }, 500);

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.InternalServerErrorException>();
			}
		}

		[Fact]
		public async Task when_a_CreateTransaction_is_called_then_we_should_have_called_the_transaction_Post_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(APIResponses.AddTransaction, 200);

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
		public async Task when_CreateTransaction_is_called_with_invalid_email_then_we_should_get_a_BadRequestException()
		{
			using (HttpTest httpTest = new HttpTest())
			{
				httpTest.RespondWithJson(new { message = "Bad Request" }, 400);

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
				httpTest.RespondWithJson(new { message = "Bad Gateway" }, 502);

				var signhostApiClient = new SignHostApiClient(settings);

				Func<Task> getTransaction = () => signhostApiClient.GetTransaction("transaction Id");
				getTransaction.ShouldThrow<ErrorHandling.SignhostRestApiClientException>();
			}
		}

		[Fact]
		public void when_a_DeleteTransaction_is_called_then_we_should_have_called_the_transaction_delete_once()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(APIResponses.DeleteTransaction, 200);

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
				httpTest.RespondWith(string.Empty, 200);

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
				httpTest.RespondWith(string.Empty, 200);

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
				httpTest.RespondWith(string.Empty, 200);

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
				httpTest.RespondWith(string.Empty, 200);

				var signhostApiClient = new SignHostApiClient(settings);

				var document = signhostApiClient.GetDocument("transaction Id", "file Id");

				httpTest.ShouldHaveCalled($"{settings.Endpoint}transaction/*/file/*")
					.WithVerb(HttpMethod.Get)
					.Times(1);
			}
		}

		[Fact]
		public async Task When_a_transaction_json_is_returned_it_is_deserialized_correctly()
		{
			using (HttpTest httpTest = new HttpTest()) {
				httpTest.RespondWith(RequestBodies.TransactionSingleSignerJson, 200);

				var signhostApiClient = new SignHostApiClient(settings);

				var result = await signhostApiClient.CreateTransaction(new Transaction
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
					.And.Subject.ShouldBeEquivalentTo(new PhoneNumberVerification {
					Number = "+31615123456"
				});
				result.Signers[0].Activities.Should().HaveCount(3);
				result.Signers[0].Activities[0].ShouldBeEquivalentTo(new Activity
				{
					Id = "Activity1",
					Code = ActivityType.Opened,
					CreatedDateTime = DateTimeOffset.Parse("2017-05-31T22:15:17.6409005+02:00")
				});
			}
		}
	}
}
