using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Signhost.APIClient.Rest;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient
{
	/// <summary>
	/// Implements the <see cref="ISignhostApiReceiver"/> interface which provides
	/// a Signhost API receiver implementation.
	/// </summary>
	public class SignhostApiReceiver
		: ISignhostApiReceiver
	{
		private readonly SignhostApiReceiverSettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="SignhostApiReceiver"/> class.
		/// Set your SharedSecret by creating a <see cref="SignhostApiReceiverSettings"/>.
		/// </summary>
		/// <param name="receiverSettings"><see cref="SignhostApiReceiverSettings"/></param>
		public SignhostApiReceiver(SignhostApiReceiverSettings receiverSettings)
		{
			this.settings = receiverSettings;
		}

		/// <inheritdoc />
		public bool IsPostbackChecksumValid(
			IDictionary<string, string[]> headers,
			string body,
			out Transaction postbackTransaction)
		{
			postbackTransaction = null;
			string[] postbackChecksumArray;
			string postbackChecksum;
			string calculatedChecksum;
			PostbackTransaction postback;

			postback = JsonConvert.DeserializeObject<PostbackTransaction>(body);
			postbackTransaction = postback;

			if (headers.TryGetValue("Checksum", out postbackChecksumArray)) {
				postbackChecksum = postbackChecksumArray.First();
			} else {
				postbackChecksum = postback.Checksum;
			}

			if (!string.IsNullOrEmpty(postbackTransaction.Id) &&
				!string.IsNullOrEmpty(postbackChecksum)
			) {
				using (var sha1 = SHA1.Create()) {
					var preCalculatedChecksum = sha1.ComputeHash(Encoding.UTF8.GetBytes(
						$"{postback.Id}||{(int)postback.Status}|{settings.SharedSecret}"));
					calculatedChecksum = BitConverter.ToString(
					preCalculatedChecksum)
						 .Replace("-", string.Empty)
						.ToLower();
				}
			} else {
				return false;
			}

			return Equals(calculatedChecksum, postbackChecksum);
		}
	}
}
