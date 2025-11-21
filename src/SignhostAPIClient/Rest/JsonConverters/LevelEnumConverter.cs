using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest.JsonConverters;

/// <summary>
/// JSON converter factory for converting the <see cref="Level"/> enum.
/// Invalid values are mapped to <see cref="Level.Unknown"/>.
/// </summary>
internal class LevelEnumConverter
	: JsonConverter<Level?>
{
	public override Level? Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null) {
			return null;
		}

		if (reader.TokenType == JsonTokenType.String) {
			var value = reader.GetString() ?? string.Empty;
			if (Enum.TryParse<Level>(value, out var level)) {
				return level;
			}

			return Level.Unknown;
		}

		if (reader.TokenType == JsonTokenType.Number) {
			int value = reader.GetInt32();
			if (Enum.IsDefined(typeof(Level), value)) {
				return (Level)value;
			}
		}

		return Level.Unknown;
	}

	public override void Write(
		Utf8JsonWriter writer,
		Level? value,
		JsonSerializerOptions options)
	{
		if (value is null) {
			writer.WriteNullValue();
		}
		else {
			writer.WriteStringValue(value.ToString());
		}
	}
}
