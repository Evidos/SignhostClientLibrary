using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Signhost.APIClient.Rest.JsonConverters
{
	public abstract class JsonBaseConverter<T>
		: JsonConverter
	{
#if TYPEINFO
		public override bool CanConvert(Type objectType)
			=> typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
#else
		public override bool CanConvert(Type objectType)
			=> typeof(T).IsAssignableFrom(objectType);
#endif

		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var target = Create(objectType, jsonObject);
			serializer.Populate(jsonObject.CreateReader(), target);
			return target;
		}

		protected abstract T Create(Type objectType, JObject jsonObject);
	}
}
