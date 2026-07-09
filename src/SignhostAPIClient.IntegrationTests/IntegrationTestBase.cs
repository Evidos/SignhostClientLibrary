using System;

namespace Signhost.APIClient.Rest.IntegrationTests;

public abstract class IntegrationTestBase
	: IDisposable
{
	protected readonly SignhostApiClient Client;

	protected IntegrationTestBase()
	{
		var config = TestConfiguration.Instance;

		if (!config.IsConfigured) {
			throw new InvalidOperationException("Integration tests are not configured.");
		}

		SignhostApiClientSettings settings = new(config.AppKey, config.UserToken) {
			Endpoint = config.ApiBaseUrl,
		};

		Client = new(settings);
	}

	public void Dispose()
	{
		Client?.Dispose();
		GC.SuppressFinalize(this);
	}
}
