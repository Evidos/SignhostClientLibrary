using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;
using Xunit;

namespace Signhost.APIClient.Rest.Tests
{
	public class ExceptionHandlingTests
	{
		private readonly SignHostApiClientSettings settings =
			new SignHostApiClientSettings("AppKey", "AuthKey") {
				Endpoint = "http://localhost/api/",
			};

		[Fact]
		public void When_GetTransaction_is_called_and_the_authorization_is_bad_then_we_should_get_a_BadAuthorizationException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.Unauthorized, new StringContent("{'message': 'unauthorized' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<UnauthorizedAccessException>();

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_request_is_bad_then_we_should_get_a_BadRequestException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.BadRequest, new StringContent("{ 'message': 'Bad Request' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<BadRequestException>();

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_not_found_then_we_should_get_a_NotFoundException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.NotFound, new StringContent("{ 'Message': 'Not Found' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<NotFoundException>().WithMessage("Not Found");

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_unkownerror_like_418_occurs_then_we_should_get_a_SignhostException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond((HttpStatusCode)418, new StringContent("{ 'message': '418 I\\'m a teapot' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<SignhostRestApiClientException>()
				.WithMessage("*418*");

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_there_is_an_InternalServerError_then_we_should_get_a_InternalServerErrorException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.InternalServerError, new StringContent("{ 'message': 'Internal Server Error' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<InternalServerErrorException>();

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_on_gone_transaction_we_shoud_get_a_GoneException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.Gone, new StringContent(APIResponses.GetTransaction));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<GoneException<Transaction>>();

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_CreateTransaction_is_called_with_invalid_email_then_we_should_get_a_BadRequestException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Post, "http://localhost/api/transaction")
				.WithHeaders("Content-Type", "application/json")
				.Respond(HttpStatusCode.BadRequest, new StringContent(" { 'message': 'Bad Request' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			var testSigner = new Signer();
			testSigner.Email = "firstname.lastnamegmail.com";

			var testTransaction = new Transaction();
			testTransaction.Signers.Add(testSigner);

			Func<Task> getTransaction = () => signhostApiClient.CreateTransactionAsync(testTransaction);
			getTransaction.Should().Throw<BadRequestException>();

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_a_function_is_called_with_a_wrong_endpoint_we_should_get_a_SignhostRestApiClientException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.BadGateway, new StringContent(" { 'Message': 'Bad Gateway' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<SignhostRestApiClientException>()
				.WithMessage("Bad Gateway");

			mockHttp.VerifyNoOutstandingExpectation();
		}

		[Fact]
		public void When_GetTransaction_is_called_and_credits_have_run_out_then_we_should_get_a_OutOfCreditsException()
		{
			using var mockHttp = new MockHttpMessageHandler();
			mockHttp
				.Expect(HttpMethod.Get, "http://localhost/api/transaction/transactionId")
				.Respond(HttpStatusCode.PaymentRequired, new StringContent("{ 'type': 'https://api.signhost.com/problem/subscription/out-of-credits' }"));

			using var httpClient = mockHttp.ToHttpClient();
			using var signhostApiClient = new SignHostApiClient(settings, httpClient);

			Func<Task> getTransaction = () => signhostApiClient.GetTransactionAsync("transactionId");
			getTransaction.Should().Throw<OutOfCreditsException>();

			mockHttp.VerifyNoOutstandingExpectation();
		}
	}
}
