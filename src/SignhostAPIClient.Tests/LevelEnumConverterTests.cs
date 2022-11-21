using FluentAssertions;
using Newtonsoft.Json;
using Signhost.APIClient.Rest.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Signhost.APIClient.Rest.Tests
{
	public class LevelEnumConverterTests
	{
		[Fact]
		public void When_Level_is_null_should_deserialize_to_null()
		{
			// Arrange
			const string json = "{\"Type\":\"eIDAS Login\",\"Level\":null}";

			// Act
			var eidasLogin = JsonConvert.DeserializeObject<EidasLoginVerification>(json);

			// Assert
			eidasLogin.Level.Should().Be(null);
		}

		[Fact]
		public void When_Level_is_not_supplied_should_deserialize_to_null()
		{
			// Arrange
			const string json = "{\"Type\":\"eIDAS Login\"}";

			// Act
			var eidasLogin = JsonConvert.DeserializeObject<EidasLoginVerification>(json);

			// Assert
			eidasLogin.Level.Should().Be(null);
		}

		[Fact]
		public void When_Level_is_unknown_should_deserialize_to_Unknown_Level()
		{
			// Arrange
			const string json = "{\"Type\":\"eIDAS Login\",\"Level\":\"foobar\"}";

			// Act
			var eidasLogin = JsonConvert.DeserializeObject<EidasLoginVerification>(json);

			// Assert
			eidasLogin.Level.Should().Be(Level.Unknown);
		}

		[Theory]
		[ClassData(typeof(LevelTestData))]
		public void When_Level_is_valid_should_deserialize_to_correct_value(Level level)
		{
			// Arrange
			string json = $"{{\"Type\":\"eIDAS Login\",\"Level\":\"{level}\"}}";

			// Act
			var eidasLogin = JsonConvert.DeserializeObject<EidasLoginVerification>(json);

			// Assert
			eidasLogin.Level.Should().Be(level);
		}

		private class LevelTestData
			: IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				foreach (var value in Enum.GetValues(typeof(Level))) {
					yield return new[] { value };
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
