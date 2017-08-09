using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;

namespace Signhost.APIClient.Rest
{
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
			if (!options.UseFileDigesting || options.DigestHashAlgorithm == null) {
				return content;
			}

			SetHashValue(fileStream, options);

			string base64Digest = Convert.ToBase64String(options.DigestHashValue);

			content.Headers.Add("Digest", $"{options.DigestHashAlgorithm}={base64Digest}");

			return content;
		}

		private static void SetHashValue(Stream fileStream, FileDigestOptions options)
		{
			if (options.DigestHashValue != null) {
				return;
			}

			if (!fileStream.CanSeek) {
				return;
			}

			long position = fileStream.Position;

			using (var algo = HashAlgorithmCreate(options)) {
				options.DigestHashValue = algo.ComputeHash(fileStream);
			}

			fileStream.Position = position;
		}

		private static HashAlgorithm HashAlgorithmCreate(FileDigestOptions options)
		{
			string algorithmName = options.DigestHashAlgorithm;
			HashAlgorithm algorithm = null;

#if NETSTANDARD1_4
			switch (algorithmName) {
				case "SHA1":
				case "SHA-1":
					algorithm = SHA1.Create();
					break;
				case "SHA256":
				case "SHA-256":
					algorithm = SHA256.Create();
					break;
				case "SHA384":
				case "SHA-384":
					algorithm = SHA384.Create();
					break;
				case "SHA512":
				case "SHA-512":
					algorithm = SHA512.Create();
					break;
			}
#else
			algorithm = HashAlgorithm.Create(algorithmName);
#endif
			if (algorithm == null && options.DigestHashValue == null) {
				algorithm = DefaultHashAlgorithm();
				options.DigestHashAlgorithm = algorithm.GetType().Name;
			}

			if (algorithm == null) {
				throw new InvalidOperationException($"No hash algorithm for '{algorithmName}'");
			}

			return algorithm;
		}

		private static HashAlgorithm DefaultHashAlgorithm() =>
#if NETSTANDARD1_4
			SHA256.Create();
#else
			HashAlgorithm.Create();
#endif
	}
}
