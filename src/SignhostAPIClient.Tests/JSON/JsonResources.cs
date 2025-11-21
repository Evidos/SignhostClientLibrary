using System.IO;
using System.Reflection;

namespace SignhostAPIClient.Tests.JSON;

public static class JsonResources
{
	public static string TransactionSingleSignerJson { get; } =
		GetJson("TransactionSingleSignerJson");

	public static string AddOrReplaceFileMetaToTransaction { get; } =
		GetJson("AddOrReplaceFileMetaToTransaction");
	public static string AddTransaction { get; } =
		GetJson("AddTransaction");
	public static string DeleteTransaction { get; } =
		GetJson("DeleteTransaction");
	public static string GetTransaction { get; } =
		GetJson("GetTransaction");
	public static string MinimalTransactionResponse { get; } =
		GetJson("MinimalTransactionResponse");
	public static string MockPostbackInvalid { get; } =
		GetJson("MockPostbackInvalid");
	public static string MockPostbackValid { get; } =
		GetJson("MockPostbackValid");

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
