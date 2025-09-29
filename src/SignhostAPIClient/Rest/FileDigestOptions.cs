﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// File digest options for file uploads
	/// </summary>
	public class FileDigestOptions
	{
		/// <summary>
		/// Gets or sets whether to use the Digest header with a checksum of
		/// the uploaded file.
		/// </summary>
		public bool UseFileDigesting { get; set; } = true;

		/// <summary>
		/// Gets or sets the digest algorithm to use when calculating
		/// the hash value or the digest algorithm that is used
		/// to set the <see cref="DigestHashValue"/>.
		/// </summary>
		public DigestHashAlgorithm DigestHashAlgorithm { get; set; } = DigestHashAlgorithm.SHA256;

		/// <summary>
		/// Gets or sets the hash digest value, you can set this yourself
		/// if you know the digest value in advance.
		/// </summary>
		public byte[] DigestHashValue { get; set; }
	}
}
