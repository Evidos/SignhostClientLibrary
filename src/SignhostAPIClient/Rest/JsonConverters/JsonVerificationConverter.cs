using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest.JsonConverters
{
	public class JsonVerificationConverter
		: JsonBaseConverter<IVerification>
	{
		public override bool CanWrite
			=> false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> new NotImplementedException();

		protected override IVerification Create(Type objectType, JObject jsonObject)
		{
			var typeName = jsonObject["Type"]?.ToString();
			switch (typeName) {
				case "Ideal":
					return new Ideal();
				case "Idin":
					return new Idin();
				default:
					return new UnknownVerification();
			}
		}
	}
}
