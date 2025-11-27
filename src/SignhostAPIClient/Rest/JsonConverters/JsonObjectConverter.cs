using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signhost.APIClient.Rest.JsonConverters;

/// <summary>
/// Converts JSON values of specific types (string, number, boolean) to/from a C# object.
/// Allows flexible deserialization of fields that can contain string, numeric, or boolean JSON values.
/// </summary>
public class JsonObjectConverter
	: JsonConverter<object>
{
	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.TokenType switch
		{
			JsonTokenType.String => reader.GetString(),
			JsonTokenType.Number => reader.TryGetInt64(out var value)
				? value
				: reader.GetDouble(),
			JsonTokenType.True or JsonTokenType.False => reader.GetBoolean(),
			JsonTokenType.Null => null,
			_ => throw new JsonException($"Unexpected token {reader.TokenType} when parsing field value. Only string, number, boolean, or null are allowed."),
		};
	}

	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
	{
		if (value is null) {
			writer.WriteNullValue();
		}
		else if (value is string s) {
			writer.WriteStringValue(s);
		}
		else if (value is bool b) {
			writer.WriteBooleanValue(b);
		}
		else if (value is int i) {
			writer.WriteNumberValue(i);
		}
		else if (value is long l) {
			writer.WriteNumberValue(l);
		}
		else if (value is double d) {
			writer.WriteNumberValue(d);
		}
		else if (value is decimal dec) {
			writer.WriteNumberValue(dec);
		}
		else {
			throw new JsonException($"Field value must be string, number, or boolean, but got {value.GetType().Name}");
		}
	}
}
