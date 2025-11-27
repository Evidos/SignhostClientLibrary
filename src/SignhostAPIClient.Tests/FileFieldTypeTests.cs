using System.Text.Json;
using FluentAssertions;
using Signhost.APIClient.Rest.DataObjects;
using Xunit;

namespace Signhost.APIClient.Rest.Tests;

public class FileFieldTypeTests
{
	[Theory]
	[InlineData(FileFieldType.Seal, "Seal")]
	[InlineData(FileFieldType.Signature, "Signature")]
	[InlineData(FileFieldType.Check, "Check")]
	[InlineData(FileFieldType.Radio, "Radio")]
	[InlineData(FileFieldType.SingleLine, "SingleLine")]
	[InlineData(FileFieldType.Number, "Number")]
	[InlineData(FileFieldType.Date, "Date")]
	public void Given_a_field_with_specific_type_When_serialized_to_json_Then_type_is_string_not_numeric(FileFieldType fieldType, string expectedString)
	{
		// Arrange
		var field = new Field
		{
			Type = fieldType,
			Value = "test",
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain($"\"Type\":\"{expectedString}\"");
		json.Should().NotContain($"\"Type\":{(int)fieldType}");
	}

	[Fact]
	public void Given_json_with_string_field_type_When_deserialized_Then_field_type_enum_is_correctly_parsed()
	{
		// Arrange
		var json = @"{
			""Type"": ""Signature"",
			""Value"": ""test"",
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Type.Should().Be(FileFieldType.Signature);
	}
}
