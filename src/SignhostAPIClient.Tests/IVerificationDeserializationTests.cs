using System.Text.Json;
using FluentAssertions;
using Signhost.APIClient.Rest.DataObjects;
using Xunit;

namespace Signhost.APIClient.Rest.Tests;

public class IVerificationDeserializationTests
{
	[Fact]
	public void Given_unknown_verification_type_When_deserializing_to_IVerification_Then_returns_unknown_verification()
	{
		// Arrange
		const string json = "{\"Type\":\"SomethingUnknown\"}";

		// Act
		var verification = JsonSerializer.Deserialize<IVerification>(json, SignhostJsonSerializerOptions.Default);

		// Assert
		var unknownVerification = verification.Should().BeOfType<UnknownVerification>().Subject;
		unknownVerification.Type.Should().Be("SomethingUnknown");
	}

	[Fact]
	public void Given_unknown_verification_with_additional_fields_When_deserializing_to_IVerification_Then_additional_data_is_available()
	{
		// Arrange
		const string json = "{\"Type\":\"SomethingUnknown\",\"DisplayName\":\"External\",\"Requires2FA\":true,\"Meta\":{\"Source\":\"partner\"}}";

		// Act
		var verification = JsonSerializer.Deserialize<IVerification>(json, SignhostJsonSerializerOptions.Default);

		// Assert
		var unknownVerification = verification.Should().BeOfType<UnknownVerification>().Subject;
		unknownVerification.Type.Should().Be("SomethingUnknown");
		unknownVerification.AdditionalData.Should().NotBeNull();
		unknownVerification.AdditionalData!.Should().ContainKey("DisplayName");
		unknownVerification.AdditionalData["DisplayName"].GetString().Should().Be("External");
		unknownVerification.AdditionalData.Should().ContainKey("Requires2FA");
		unknownVerification.AdditionalData["Requires2FA"].GetBoolean().Should().BeTrue();
		unknownVerification.AdditionalData.Should().ContainKey("Meta");
		unknownVerification.AdditionalData["Meta"].GetProperty("Source").GetString().Should().Be("partner");
	}
}
