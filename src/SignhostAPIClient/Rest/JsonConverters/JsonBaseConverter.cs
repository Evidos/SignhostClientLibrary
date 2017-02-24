using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Signhost.APIClient.Rest.JsonConverters
{
	public abstract class JsonBaseConverter<T>
		: JsonConverter
	{
		public override bool CanConvert(Type objectType)
			=> typeof(T).IsAssignableFrom(objectType);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var target = Create(objectType, jsonObject);
			serializer.Populate(jsonObject.CreateReader(), target);
			return target;
		}

		protected abstract T Create(Type objectType, JObject jsonObject);
	}
}
