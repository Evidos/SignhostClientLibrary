using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest.JsonConverters;

public class VerificationConverter
	: JsonConverter<IVerification>
{
	public override IVerification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var jsonElement = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
		if (jsonElement.ValueKind is not JsonValueKind.Object) {
			throw new JsonException("Verification must be a JSON object.");
		}

		if (
			!jsonElement.TryGetProperty("Type", out var typeElement) ||
			typeElement.ValueKind is not JsonValueKind.String ||
			string.IsNullOrWhiteSpace(typeElement.GetString())
		) {
			throw new JsonException("Verification must contain a non-empty Type property.");
		}

		string discriminator = typeElement.GetString()!;
		string json = jsonElement.GetRawText();
		var verificationType = ResolveType(discriminator);

		if (verificationType is not null) {
			return (IVerification)JsonSerializer.Deserialize(json, verificationType, options)!;
		}

		return JsonSerializer.Deserialize<UnknownVerification>(json, options)!;
	}

	public override void Write(Utf8JsonWriter writer, IVerification value, JsonSerializerOptions options)
	{
		if (value is UnknownVerification unknownVerification) {
			JsonSerializer.Serialize(writer, unknownVerification, options);
			return;
		}

		string discriminator = ResolveDiscriminator(value)
			?? throw new JsonException($"Verification type '{value.GetType().Name}' is not supported.");

		string json = JsonSerializer.Serialize(value, value.GetType(), options);
		using var jsonDocument = JsonDocument.Parse(json);

		writer.WriteStartObject();
		writer.WriteString("Type", discriminator);

		foreach (var property in jsonDocument.RootElement.EnumerateObject()) {
			writer.WritePropertyName(property.Name);
			property.Value.WriteTo(writer);
		}

		writer.WriteEndObject();
	}

	private static Type? ResolveType(string discriminator) => discriminator switch {
		"Consent" => typeof(ConsentVerification),
		"DigiD" => typeof(DigidVerification),
		"eIDAS Login" => typeof(EidasLoginVerification),
		"iDeal" => typeof(IdealVerification),
		"iDIN" => typeof(IdinVerification),
		"IPAddress" => typeof(IPAddressVerification),
		"itsme Identification" => typeof(ItsmeIdentificationVerification),
		"PhoneNumber" => typeof(PhoneNumberVerification),
		"Scribble" => typeof(ScribbleVerification),
		"SURFnet" => typeof(SurfnetVerification),
		"CSC Qualified" => typeof(CscVerification),
		"eHerkenning" => typeof(EherkenningVerification),
		"OpenID Providers" => typeof(OidcVerification),
		"Onfido" => typeof(OnfidoVerification),
		_ => null,
	};

	private static string? ResolveDiscriminator(IVerification value) => value switch {
		ConsentVerification => "Consent",
		DigidVerification => "DigiD",
		EidasLoginVerification => "eIDAS Login",
		IdealVerification => "iDeal",
		IdinVerification => "iDIN",
		IPAddressVerification => "IPAddress",
		ItsmeIdentificationVerification => "itsme Identification",
		PhoneNumberVerification => "PhoneNumber",
		ScribbleVerification => "Scribble",
		SurfnetVerification => "SURFnet",
		CscVerification => "CSC Qualified",
		EherkenningVerification => "eHerkenning",
		OidcVerification => "OpenID Providers",
		OnfidoVerification => "Onfido",
		_ => null,
	};
}
