using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
			string postbackChecksum;
			string calculatedChecksum;
			PostbackTransaction postback;

			postback = DeserializeToPostbackTransaction(body);
			postbackChecksum = GetChecksumFromHeadersOrPostback(headers, postback);
			bool parametersAreValid = HasValidChecksumProperties(postbackChecksum, postback);

			if (parametersAreValid) {
				calculatedChecksum = CalculateChecksumFromPostback(postback);
				postbackTransaction = postback;
			} else {
				return false;
			}

			return Equals(calculatedChecksum, postbackChecksum);
		}

		private string CalculateChecksumFromPostback(PostbackTransaction postback)
		{
			using (var sha1 = SHA1.Create()) {
				var checksumBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(
					$"{postback.Id}||{(int)postback.Status}|{settings.SharedSecret}"));
				return BitConverter.ToString(checksumBytes)
					.Replace("-", string.Empty)
					.ToLower();
			}
		}

	private PostbackTransaction DeserializeToPostbackTransaction(string body)
	{
		return JsonSerializer.Deserialize<PostbackTransaction>(body, SignhostJsonSerializerOptions.Default);
	}		private string GetChecksumFromHeadersOrPostback(
			IDictionary<string, string[]> headers,
			PostbackTransaction postback)
		{
			string[] postbackChecksumArray;
			if (headers.TryGetValue("Checksum", out postbackChecksumArray)) {
				return postbackChecksumArray.First();
			}
			else {
				return postback.Checksum;
			}
		}

		private bool HasValidChecksumProperties(string postbackChecksum, PostbackTransaction postback)
		{
			return !string.IsNullOrWhiteSpace(postbackChecksum) && !string.IsNullOrWhiteSpace(postback.Id);
		}
	}
}
