using Xunit;
using Signhost.APIClient.Rest.DataObjects;
using FluentAssertions;
using System.Collections.Generic;
using SignhostAPIClient.Tests.JSON;

namespace Signhost.APIClient.Rest.Tests;

public class SignhostApiReceiverTests
{
	private readonly SignhostApiReceiverSettings receiverSettings =
		new("SharedSecret");

	[Fact]
	public void When_IsPostbackChecksumValid_is_called_with_valid_postback_in_body_Then_true_is_returned()
	{
		// Arrange
		var headers = new Dictionary<string, string[]> {
			["Content-Type"] = ["application/json"]
		};

		string body = JsonResources.MockPostbackValid;

		// Act
		SignhostApiReceiver signhostApiReceiver = new(receiverSettings);
		bool result = signhostApiReceiver
			.IsPostbackChecksumValid(headers, body, out Transaction _);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void When_IsPostbackChecksumValid_is_called_with_invalid_postback_in_body_Then_false_is_returned()
	{
		// Arrange
		var headers = new Dictionary<string, string[]> {
			["Content-Type"] = ["application/json"]
		};

		string body = JsonResources.MockPostbackInvalid;

		// Act
		SignhostApiReceiver signhostApiReceiver = new(receiverSettings);
		bool result = signhostApiReceiver
			.IsPostbackChecksumValid(headers, body, out Transaction _);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public void When_IsPostbackChecksumValid_is_called_with_valid_postback_in_header_Then_true_is_returned()
	{
		// Arrange
		var headers = new Dictionary<string, string[]> {
			["Content-Type"] = ["application/json"],
			["Checksum"] = ["cdc09eee2ed6df2846dcc193aedfef59f2834f8d"]
		};

		string body = JsonResources.MockPostbackValid;

		// Act
		SignhostApiReceiver signhostApiReceiver = new(receiverSettings);
		bool result = signhostApiReceiver
			.IsPostbackChecksumValid(headers, body, out Transaction _);

		// Assert
		result.Should().BeTrue();
	}

	[Fact]
	public void When_IsPostbackChecksumValid_is_called_with_invalid_postback_in_header_Then_false_is_returned()
	{
		// Arrange
		var headers = new Dictionary<string, string[]> {
			["Content-Type"] = ["application/json"],
			["Checksum"] = ["70dda90616f744797972c0d2f787f86643a60c83"]
		};
		string body = JsonResources.MockPostbackValid;

		// Act
		SignhostApiReceiver signhostApiReceiver = new(receiverSettings);
		bool result = signhostApiReceiver
			.IsPostbackChecksumValid(headers, body, out Transaction _);

		// Assert
		result.Should().BeFalse();
	}
}
