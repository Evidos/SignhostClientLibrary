namespace Signhost.APIClient.Rest;

/// <summary>
/// Provides constants for hash algorithm names used in HTTP Digest headers,
/// following the naming conventions specified in RFC 3230 (Instance Digests in HTTP)
/// and RFC 5843 (Additional Hash Algorithms for HTTP Instance Digests).
///
/// These names are in accordance with the Digest header in HTTP requests,
/// where the header specifies the algorithm used to create the digest of the resource.
///
/// For more information:
/// https://evidos.github.io/endpoints/##/paths//api/transaction/%7BtransactionId%7D/file/%7BfileId%7D/put
/// </summary>
public enum DigestHashAlgorithm
{
	/// <summary>
	/// Use no digest.
	/// </summary>
	None = 0,

	/// <summary>
	/// SHA-256 hash algorithm, as specified in RFC 5843.
	/// </summary>
	SHA256,

	/// <summary>
	/// SHA-512 hash algorithm, as specified in RFC 5843.
	/// </summary>
	SHA512,
}
