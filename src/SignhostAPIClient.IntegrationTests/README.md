# Integration Tests

This project contains integration tests that require live API credentials to run.

## Configuration

The tests require Signhost API credentials configured via .NET User Secrets:

```bash
cd src/SignhostAPIClient.IntegrationTests
dotnet user-secrets set "Signhost:AppKey" "your-app-key-here"
dotnet user-secrets set "Signhost:UserToken" "your-user-token-here"
dotnet user-secrets set "Signhost:ApiBaseUrl" "https://api.signhost.com/api"
```

## Running Tests

```bash
dotnet test src/SignhostAPIClient.IntegrationTests/SignhostAPIClient.IntegrationTests.csproj
```

## Important Notes

- These tests are **excluded from CI/CD** pipelines
- These tests are **not packaged** in the NuGet package
- Tests will fail if credentials are not configured
- Tests create real transactions in your Signhost account
