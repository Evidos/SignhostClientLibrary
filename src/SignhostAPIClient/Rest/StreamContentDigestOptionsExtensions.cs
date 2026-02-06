using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;

namespace Signhost.APIClient.Rest;

/// <summary>
/// <see cref="StreamContent"/> digest extensions.
/// </summary>
public static class StreamContentDigestOptionsExtensions
{
	/// <summary>
	/// Digest extension method on the <see cref="IFlurlClient"/>.
	/// </summary>
	/// <param name="content"><see cref="StreamContent"/></param>
	/// <param name="fileStream"><see cref="Stream"/> of the filestream.
	/// No digest is calculated if the stream is not <see cref="Stream.CanSeek"/>.</param>
	/// <param name="options"><see cref="FileDigestOptions"/> digest options to use.</param>
	/// <returns><see cref="IFlurlClient"/>.</returns>
	public static StreamContent WithDigest(
		this StreamContent content,
		Stream fileStream,
		FileDigestOptions options)
	{
		content.ThrowIfNullOrEmpty(nameof(content));
		fileStream.ThrowIfNullOrEmpty(nameof(fileStream));
		options.ThrowIfNullOrEmpty(nameof(options));

		if (
			!options.UseFileDigesting ||
			options.DigestHashAlgorithm == DigestHashAlgorithm.None
		) {
			return content;
		}

		SetHashValue(fileStream, options);
		if (options.DigestHashValue is null) {
			throw new InvalidOperationException(
				"Digest hash value is not set after calculating it.");
		}

		string base64Digest = Convert.ToBase64String(options.DigestHashValue);

		content.Headers.Add("Digest", $"{GetDigestHashAlgorithmName(options)}={base64Digest}");

		return content;
	}

	private static string GetDigestHashAlgorithmName(FileDigestOptions options)
	{
		return options.DigestHashAlgorithm switch {
			DigestHashAlgorithm.SHA256 => "SHA-256",
			DigestHashAlgorithm.SHA512 => "SHA-512",

			_ => throw new InvalidOperationException(
				$"No hash algorithm name for '{options.DigestHashAlgorithm}'."),
		};
	}

	private static void SetHashValue(
		Stream fileStream,
		FileDigestOptions options)
	{
		if (options.DigestHashValue is not null) {
			return;
		}

		if (!fileStream.CanSeek) {
			return;
		}

		long position = fileStream.Position;

		using var algo = HashAlgorithmCreate(options);
		options.DigestHashValue = algo.ComputeHash(fileStream);

		fileStream.Position = position;
	}

	private static HashAlgorithm HashAlgorithmCreate(
		FileDigestOptions options)
	{
		return options.DigestHashAlgorithm switch {
#if NET462
			DigestHashAlgorithm.SHA256 => HashAlgorithm.Create("SHA256"),
			DigestHashAlgorithm.SHA512 => HashAlgorithm.Create("SHA512"),
#else
			DigestHashAlgorithm.SHA256 => SHA256.Create(),
			DigestHashAlgorithm.SHA512 => SHA512.Create(),
#endif
			_ => throw new InvalidOperationException(
				$"No hash algorithm for '{options.DigestHashAlgorithm}'."),
		};
	}
}
