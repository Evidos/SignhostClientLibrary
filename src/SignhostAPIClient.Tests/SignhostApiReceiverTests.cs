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
using SignhostAPIClient.Tests.JSON;

namespace Signhost.APIClient.Rest.Tests
{
	public class SignhostApiReceiverTests
	{
		private SignhostApiReceiverSettings receiverSettings = new SignhostApiReceiverSettings("SharedSecret");

		[Fact]
		public void when_IsPostbackChecksumValid_is_called_with_valid_postback_in_body_then_true_is_returned()
		{
			// Arrange
			IDictionary<string, string[]> headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
			string body = JsonResources.MockPostbackValid;

			// Act
			SignhostApiReceiver signhostApiReceiver = new SignhostApiReceiver(receiverSettings);
			bool result = signhostApiReceiver.IsPostbackChecksumValid(headers, body, out Transaction transaction);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public void when_IsPostbackChecksumValid_is_called_with_invalid_postback_in_body_then_false_is_returned()
		{
			// Arrange
			IDictionary<string, string[]> headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
			string body = JsonResources.MockPostbackInvalid;

			// Act
			SignhostApiReceiver signhostApiReceiver = new SignhostApiReceiver(receiverSettings);
			bool result = signhostApiReceiver.IsPostbackChecksumValid(headers, body, out Transaction transaction);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public void when_IsPostbackChecksumValid_is_called_with_valid_postback_in_header_then_true_is_returned()
		{
			// Arrange
			IDictionary<string, string[]> headers = new Dictionary<string, string[]> {
				{ "Content-Type", new[] { "application/json" }},
				{"Checksum", new[] {"cdc09eee2ed6df2846dcc193aedfef59f2834f8d"}}
			};
			string body = JsonResources.MockPostbackValid;

			// Act
			SignhostApiReceiver signhostApiReceiver = new SignhostApiReceiver(receiverSettings);
			bool result = signhostApiReceiver.IsPostbackChecksumValid(headers, body, out Transaction transaction);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public void when_IsPostbackChecksumValid_is_called_with_invalid_postback_in_header_then_false_is_returned()
		{
			// Arrange
			IDictionary<string, string[]> headers = new Dictionary<string, string[]> {
				{ "Content-Type", new[] { "application/json" }},
				{"Checksum", new[] {"70dda90616f744797972c0d2f787f86643a60c83"}}
			};
			string body = JsonResources.MockPostbackValid;

			// Act
			SignhostApiReceiver signhostApiReceiver = new SignhostApiReceiver(receiverSettings);
			bool result = signhostApiReceiver.IsPostbackChecksumValid(headers, body, out Transaction transaction);

			// Assert
			result.Should().BeFalse();
		}
	}
}
