using System.Text.Json;
using FluentAssertions;
using Signhost.APIClient.Rest.DataObjects;
using Xunit;

namespace Signhost.APIClient.Rest.Tests;

public class FieldValueTests
{
	[Fact]
	public void Given_a_field_with_string_value_When_serialized_Then_value_is_json_string()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.SingleLine,
			Value = "John Smith",
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":\"John Smith\"");
	}

	[Fact]
	public void Given_a_field_with_numeric_integer_value_When_serialized_Then_value_is_json_number()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.Number,
			Value = 42,
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":42");
		json.Should().NotContain("\"Value\":\"42\"");
	}

	[Fact]
	public void Given_a_field_with_numeric_double_value_When_serialized_Then_value_is_json_number()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.Number,
			Value = 3.14,
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":3.14");
		json.Should().NotContain("\"Value\":\"3.14\"");
	}

	[Fact]
	public void Given_a_field_with_boolean_true_value_When_serialized_Then_value_is_json_boolean()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.Check,
			Value = true,
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":true");
		json.Should().NotContain("\"Value\":\"true\"");
	}

	[Fact]
	public void Given_a_field_with_boolean_false_value_When_serialized_Then_value_is_json_boolean()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.Check,
			Value = false,
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":false");
		json.Should().NotContain("\"Value\":\"false\"");
	}

	[Fact]
	public void Given_a_field_with_null_value_When_serialized_Then_value_is_json_null()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.Signature,
			Value = null,
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var json = JsonSerializer.Serialize(field);

		// Assert
		json.Should().Contain("\"Value\":null");
	}

	[Fact]
	public void Given_json_with_string_value_When_deserialized_Then_field_value_is_string()
	{
		// Arrange
		var json = @"{
			""Type"": ""SingleLine"",
			""Value"": ""Test Name"",
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeOfType<string>().Which.Should().Be("Test Name");
	}

	[Fact]
	public void Given_json_with_number_integer_value_When_deserialized_Then_field_value_is_numeric()
	{
		// Arrange
		var json = @"{
			""Type"": ""Number"",
			""Value"": 123,
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeOneOf(123L, 123.0);
	}

	[Fact]
	public void Given_json_with_number_decimal_value_When_deserialized_Then_field_value_is_double()
	{
		// Arrange
		var json = @"{
			""Type"": ""Number"",
			""Value"": 45.67,
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeOfType<double>().Which.Should().Be(45.67);
	}

	[Fact]
	public void Given_json_with_boolean_true_value_When_deserialized_Then_field_value_is_boolean_true()
	{
		// Arrange
		var json = @"{
			""Type"": ""Check"",
			""Value"": true,
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeOfType<bool>().Which.Should().BeTrue();
	}

	[Fact]
	public void Given_json_with_boolean_false_value_When_deserialized_Then_field_value_is_boolean_false()
	{
		// Arrange
		var json = @"{
			""Type"": ""Check"",
			""Value"": false,
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeOfType<bool>().Which.Should().BeFalse();
	}

	[Fact]
	public void Given_json_with_null_value_When_deserialized_Then_field_value_is_null()
	{
		// Arrange
		var json = @"{
			""Type"": ""Signature"",
			""Value"": null,
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var field = JsonSerializer.Deserialize<Field>(json);

		// Assert
		field.Should().NotBeNull();
		field!.Value.Should().BeNull();
	}

	[Fact]
	public void Given_a_field_with_invalid_object_value_When_serialized_Then_throws_json_exception()
	{
		// Arrange
		var field = new Field {
			Type = FileFieldType.SingleLine,
			Value = new { Name = "Test" },
			Location = new Location { PageNumber = 1 }
		};

		// Act
		var act = () => JsonSerializer.Serialize(field);

		// Assert
		act.Should().Throw<JsonException>();
	}

	[Fact]
	public void Given_json_with_object_value_When_deserialized_Then_throws_json_exception()
	{
		// Arrange
		var json = @"{
			""Type"": ""SingleLine"",
			""Value"": { ""nested"": ""object"" },
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var act = () => JsonSerializer.Deserialize<Field>(json);

		// Assert
		act.Should().Throw<JsonException>();
	}

	[Fact]
	public void Given_json_with_array_value_When_deserialized_Then_throws_json_exception()
	{
		// Arrange
		var json = @"{
			""Type"": ""SingleLine"",
			""Value"": [1, 2, 3],
			""Location"": { ""PageNumber"": 1 }
		}";

		// Act
		var act = () => JsonSerializer.Deserialize<Field>(json);

		// Assert
		act.Should().Throw<JsonException>();
	}
}
