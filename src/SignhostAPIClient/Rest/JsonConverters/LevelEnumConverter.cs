using System;
using System.Reflection;
using Newtonsoft.Json;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest.JsonConverters
{
	/// <summary>
	/// JSON converter for converting the <see cref="Level"/> enum.
	/// Invalid values are mapped to <see cref="Level.Unknown"/>.
	/// </summary>
	internal class LevelEnumConverter
		: JsonConverter
	{
		/// <inheritdoc/>
		public override bool CanWrite => false;

		/// <inheritdoc/>
		public override bool CanConvert(Type objectType)
			=> IsLevelEnum(GetUnderlyingType(objectType));

		/// <inheritdoc/>
		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer)
		{
			var value = reader.Value as string;

			if (value != null) {
				if (Enum.TryParse(value, out Level level)) {
					return level;
				}

				return Level.Unknown;
			}

			return null;
		}

		/// <inheritdoc/>
		public override void WriteJson(
				JsonWriter writer,
				object value,
				JsonSerializer serializer)
			=> throw new NotImplementedException();

		private static Type GetUnderlyingType(Type type)
			=>
#if TYPEINFO
				type.GetTypeInfo().IsGenericType &&
#else
				type.IsGenericType &&
#endif
				type.GetGenericTypeDefinition() == typeof(Nullable<>)
					? Nullable.GetUnderlyingType(type)
					: type;

		private static bool IsLevelEnum(Type type)
			=>
#if TYPEINFO
				type.GetTypeInfo().IsEnum &&
#else
				type.IsEnum &&
#endif
				type == typeof(Level);
	}
}
