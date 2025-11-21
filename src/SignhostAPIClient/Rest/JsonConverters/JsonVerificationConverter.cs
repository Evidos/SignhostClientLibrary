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
#if TYPEINFO
		private static readonly IDictionary<string, TypeInfo> VerificationTypes =
#else
		private static readonly IDictionary<string, Type> VerificationTypes =
#endif
			CreateVerificationTypeMap();

		public override bool CanWrite
			=> false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> new NotImplementedException();

		/// <summary>
		/// Adds an additional verification type to the <see cref="VerificationTypes"/>
		/// map.
		/// </summary>
		/// <typeparam name="T"><see cref="IVerification"/></typeparam>
		internal static void RegisterVerification<T>()
			where T : IVerification
		{
			var verification = (IVerification)Activator.CreateInstance(typeof(T));

			VerificationTypes[verification.Type] =
#if TYPEINFO
				typeof(T).GetTypeInfo();
#else
				typeof(T);
#endif
		}

		protected override IVerification Create(
			Type objectType,
			JObject jsonObject)
		{
			var typeName = jsonObject["Type"]?.ToString();

			if (VerificationTypes.TryGetValue(typeName, out var verificationType)) {
#if TYPEINFO
				return (IVerification)Activator.CreateInstance(verificationType.AsType());
#else
				return (IVerification)Activator.CreateInstance(verificationType);
#endif
			}

			return new UnknownVerification();
		}

#if TYPEINFO
		private static Dictionary<string, TypeInfo> CreateVerificationTypeMap()
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
#else
		private static Dictionary<string, Type> CreateVerificationTypeMap()
		{
			return typeof(JsonVerificationConverter).Assembly.ExportedTypes
				.Where(t => typeof(IVerification).IsAssignableFrom(t))
				.Where(t => !t.IsInterface && !t.IsAbstract)
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
				.Select(t => (
					type: t,
					instance: (IVerification)Activator.CreateInstance(t)))
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
				.Where(t => t.instance.Type is not null)
				.ToDictionary(t => t.instance.Type, t => t.type);
		}
#endif
	}
}
