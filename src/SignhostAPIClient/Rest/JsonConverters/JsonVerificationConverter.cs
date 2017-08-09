using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest.JsonConverters
{
	internal class JsonVerificationConverter
		: JsonBaseConverter<IVerification>
	{
		private static readonly IReadOnlyDictionary<string, TypeInfo> VerificationTypes =
			CreateVerificationTypeMap();

		public override bool CanWrite
			=> false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> new NotImplementedException();

		protected override IVerification Create(Type objectType, JObject jsonObject)
		{
			var typeName = jsonObject["Type"]?.ToString();

			if (VerificationTypes.TryGetValue(typeName, out var verificationType)) {
				return (IVerification)Activator.CreateInstance(verificationType.AsType());
			}

			return new UnknownVerification();
		}

		private static IReadOnlyDictionary<string, TypeInfo> CreateVerificationTypeMap()
		{
			return typeof(JsonVerificationConverter).GetTypeInfo().Assembly.ExportedTypes
				.Select(t => t.GetTypeInfo())
				.Where(t => typeof(IVerification).GetTypeInfo().IsAssignableFrom(t))
				.Where(t => !t.IsInterface && !t.IsAbstract)
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
				.Select(t => (
					typeInfo: t,
					instance: (IVerification)Activator.CreateInstance(t.AsType())))
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
				.Where(t => t.instance.Type != null)
				.ToDictionary(t => t.instance.Type, t => t.typeInfo);
		}
	}
}
