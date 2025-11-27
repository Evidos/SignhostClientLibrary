using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Signhost.APIClient.Rest;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient;

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
	/// <param name="receiverSettings"><see cref="SignhostApiReceiverSettings"/>
	/// Settings for the receiver.
	/// </param>
	public SignhostApiReceiver(SignhostApiReceiverSettings receiverSettings)
	{
		this.settings = receiverSettings;
	}

	/// <inheritdoc />
	public bool IsPostbackChecksumValid(
		IDictionary<string, string[]> headers,
		string body,
		[NotNullWhen(true)] out Transaction? postbackTransaction)
	{
		postbackTransaction = null;
		var postback = DeserializeToPostbackTransaction(body);
		if (postback is null) {
			return false;
		}

		string postbackChecksum = GetChecksumFromHeadersOrPostback(headers, postback);
		bool parametersAreValid = HasValidChecksumProperties(postbackChecksum, postback);

		string calculatedChecksum;
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

	private PostbackTransaction? DeserializeToPostbackTransaction(string body)
	{
		return JsonSerializer.Deserialize<PostbackTransaction>(body, SignhostJsonSerializerOptions.Default);
	}

	private string GetChecksumFromHeadersOrPostback(
		IDictionary<string, string[]> headers,
		PostbackTransaction postback)
	{
		if (
			headers.TryGetValue("Checksum", out var postbackChecksumArray) &&
			postbackChecksumArray is not null
		) {
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
