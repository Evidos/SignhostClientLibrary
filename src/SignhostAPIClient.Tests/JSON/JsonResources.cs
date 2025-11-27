using System.IO;
using System.Reflection;

namespace SignhostAPIClient.Tests.JSON;

public static class JsonResources
{
	// Request JSONs
	public static string AddOrReplaceFileMetaToTransaction { get; } = GetJson("Requests.AddOrReplaceFileMetaToTransaction");

	// Response JSONs
	public static string TransactionSingleSignerJson { get; } = GetJson("Responses.TransactionSingleSignerJson");
	public static string AddTransaction { get; } = GetJson("Responses.AddTransaction");
	public static string DeleteTransaction { get; } = GetJson("Responses.DeleteTransaction");
	public static string GetTransaction { get; } = GetJson("Responses.GetTransaction");
	public static string MinimalTransactionResponse { get; } = GetJson("Responses.MinimalTransactionResponse");
	public static string MockPostbackInvalid { get; } = GetJson("Responses.MockPostbackInvalid");
	public static string MockPostbackValid { get; } = GetJson("Responses.MockPostbackValid");

	private static string GetJson(string fileName)
	{
		var assembly = Assembly.GetExecutingAssembly();
		string resourceName = $"Signhost.APIClient.Rest.Tests.JSON.{fileName}.json";

		using var stream = assembly.GetManifestResourceStream(resourceName)
			?? throw new FileNotFoundException($"File not found: {fileName}");

		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}
