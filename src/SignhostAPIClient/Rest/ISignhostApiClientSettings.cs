using System;

namespace Signhost.APIClient.Rest;

public delegate void AddHeaders(string name, string value);

public interface ISignhostApiClientSettings
{
	/// <summary>
	/// Gets the usertoken identifying an authorized user.
	/// </summary>
	string? UserToken { get; }

	/// <summary>
	/// Gets the app key of your applications.
	/// </summary>
	string APPKey { get; }

	/// <summary>
	/// Gets the signhost API endpoint.
	/// </summary>
	string Endpoint { get; }

	Action<AddHeaders>? AddHeader { get; }
}
