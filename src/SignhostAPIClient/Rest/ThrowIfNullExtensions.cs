using System;

namespace Signhost.APIClient.Rest;

/// <summary>
/// Polyfill extensions for ArgumentNullException.ThrowIfNull that works across all target frameworks.
/// </summary>
internal static class ThrowIfNullExtensions
{
	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if the specified argument is <c>null</c>.
	/// </summary>
	/// <param name="value">The value to check for <c>null</c>.</param>
	/// <param name="paramName">The name of the parameter.</param>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
	internal static void ThrowIfNullOrEmpty(this object? value, string? paramName = null)
	{
		if (value is null) {
			throw new ArgumentNullException(paramName);
		}

		if (value is string str && string.IsNullOrWhiteSpace(str)) {
			throw new ArgumentException("Argument cannot be empty or whitespace.", paramName);
		}
	}
}
