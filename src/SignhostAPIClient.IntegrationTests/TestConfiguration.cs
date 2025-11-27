using System;
using Microsoft.Extensions.Configuration;

namespace Signhost.APIClient.Rest.IntegrationTests;

/// <summary>
/// Configuration for integration tests loaded from user secrets only.
/// No appsettings.json is used to prevent accidental credential commits.
/// </summary>
public class TestConfiguration
{
	private static readonly Lazy<TestConfiguration> LazyInstance =
		new(() => new TestConfiguration());

	private TestConfiguration()
	{
		var builder = new ConfigurationBuilder()
			.AddUserSecrets<TestConfiguration>(optional: false);

		IConfiguration configuration = builder.Build();
		AppKey = configuration["Signhost:AppKey"];
		UserToken = configuration["Signhost:UserToken"];
		ApiBaseUrl = configuration["Signhost:ApiBaseUrl"];
	}

	public static TestConfiguration Instance => LazyInstance.Value;

	public string AppKey { get; }

	public string UserToken { get; }

	public string ApiBaseUrl { get; }

	public bool IsConfigured =>
		!string.IsNullOrWhiteSpace(AppKey) &&
		!string.IsNullOrWhiteSpace(UserToken) &&
		!string.IsNullOrWhiteSpace(ApiBaseUrl);
}
