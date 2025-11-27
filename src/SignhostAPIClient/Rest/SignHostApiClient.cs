using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest;

/// <summary>
/// Implements the <see cref="ISignhostApiClient"/> interface which provides
/// an signhost api client implementation.
/// </summary>
public class SignhostApiClient
	: ISignhostApiClient
	, IDisposable
{
	private const string ApiVersion = "v1";

	private static readonly string Version = GetVersion()
		?? throw new InvalidOperationException("Unknown assembly version.");

	private readonly ISignhostApiClientSettings settings;
	private readonly HttpClient client;

	/// <summary>
	/// Initializes a new instance of the <see cref="SignhostApiClient"/> class.
	/// Set your usertoken and APPKey by creating a <see cref="SignhostApiClientSettings"/>.
	/// </summary>
	/// <param name="settings"><see cref="SignhostApiClientSettings"/>.</param>
	public SignhostApiClient(ISignhostApiClientSettings settings)
		: this(settings, new HttpClient())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SignhostApiClient"/> class.
	/// Set your usertoken and APPKey by creating a <see cref="SignhostApiClientSettings"/>.
	/// </summary>
	/// <param name="settings"><see cref="SignhostApiClientSettings"/>.</param>
	/// <param name="httpClient"><see cref="HttpClient"/> to use for all http calls.</param>
	public SignhostApiClient(
		ISignhostApiClientSettings settings,
		HttpClient httpClient)
	{
		this.settings = settings;
		this.client = httpClient;
		this.client.BaseAddress = new Uri(
			settings.Endpoint + (settings.Endpoint.EndsWith("/") ? string.Empty : "/"));
		this.client.DefaultRequestHeaders.UserAgent.Add(
			new System.Net.Http.Headers.ProductInfoHeaderValue(
				"SignhostClientLibrary",
				Version));
		this.client.DefaultRequestHeaders.Add("Application", ApplicationHeader);

		if (!string.IsNullOrWhiteSpace(settings.UserToken)) {
			this.client.DefaultRequestHeaders.Add("Authorization", AuthorizationHeader);
		}

		this.client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse($"application/vnd.signhost.{ApiVersion}+json"));
		settings.AddHeader?.Invoke(this.client.DefaultRequestHeaders.Add);
	}

	private string ApplicationHeader
		=> $"APPKey {settings.APPKey}";

	private string AuthorizationHeader
		=> $"APIKey {settings.UserToken}";

	/// <inheritdoc />
	public async Task<Transaction> CreateTransactionAsync(
		Transaction transaction)
		=> await CreateTransactionAsync(transaction, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task<Transaction> CreateTransactionAsync(
		Transaction transaction,
		CancellationToken cancellationToken = default)
	{
		if (transaction is null) {
			throw new ArgumentNullException(nameof(transaction));
		}

		var result = await client
			.PostAsync(
				"transaction",
				JsonContent.From(transaction),
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync()
			.ConfigureAwait(false);

		return await result.Content.FromJsonAsync<Transaction>()
			.ConfigureAwait(false)
			?? throw new InvalidOperationException(
				"Failed to deserialize the transaction.");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<Transaction>> GetTransactionResponseAsync(
		string transactionId)
		=> await GetTransactionResponseAsync(transactionId, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task<ApiResponse<Transaction>> GetTransactionResponseAsync(
		string transactionId,
		CancellationToken cancellationToken = default)
	{
		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		var result = await client
			.GetAsync(
				"transaction".JoinPaths(transactionId),
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.Gone)
			.ConfigureAwait(false);
		var transaction = await result.Content.FromJsonAsync<Transaction>()
			.ConfigureAwait(false);

		return new ApiResponse<Transaction>(result, transaction);
	}

	/// <inheritdoc />
	public async Task<Transaction> GetTransactionAsync(string transactionId)
		=> await GetTransactionAsync(transactionId, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task<Transaction> GetTransactionAsync(
		string transactionId,
		CancellationToken cancellationToken = default)
	{
		var response = await GetTransactionResponseAsync(
			transactionId,
			cancellationToken)
			.ConfigureAwait(false);

		await response.EnsureAvailableStatusCodeAsync(cancellationToken)
			.ConfigureAwait(false);

		return response.Value ?? throw new InvalidOperationException(
			"Failed to deserialize the transaction.");
	}

	/// <inheritdoc />
	public async Task DeleteTransactionAsync(
		string transactionId,
		CancellationToken cancellationToken = default)
		=> await DeleteTransactionAsync(
			transactionId,
			default,
			cancellationToken).ConfigureAwait(false);

	/// <inheritdoc />
	public async Task DeleteTransactionAsync(
		string transactionId,
		DeleteTransactionOptions options)
		=> await DeleteTransactionAsync(
			transactionId,
			options,
			default).ConfigureAwait(false);

	/// <inheritdoc />
	public async Task DeleteTransactionAsync(
		string transactionId,
		DeleteTransactionOptions? options,
		CancellationToken cancellationToken = default)
	{
		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		if (options is null) {
			options = new DeleteTransactionOptions();
		}

		var request = new HttpRequestMessage(HttpMethod.Delete, "transaction".JoinPaths(transactionId));
		request.Content = JsonContent.From(options);
		await client
			.SendAsync(
				request,
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync()
			.ConfigureAwait(false);
	}

	/// <inheritdoc />
	public async Task AddOrReplaceFileMetaToTransactionAsync(
		FileMeta fileMeta,
		string transactionId,
		string fileId)
		=> await AddOrReplaceFileMetaToTransactionAsync(
			fileMeta,
			transactionId,
			fileId,
			default).ConfigureAwait(false);

	/// <inheritdoc />
	public async Task AddOrReplaceFileMetaToTransactionAsync(
		FileMeta fileMeta,
		string transactionId,
		string fileId,
		CancellationToken cancellationToken = default)
	{
		if (fileMeta is null) {
			throw new ArgumentNullException("fileMeta");
		}

		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		if (fileId is null) {
			throw new ArgumentNullException(nameof(fileId));
		}

		if (string.IsNullOrWhiteSpace(fileId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
		}

		await client
			.PutAsync(
				"transaction".JoinPaths(transactionId, "file", fileId),
				JsonContent.From(fileMeta),
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync()
			.ConfigureAwait(false);
	}

	/// <inheritdoc />
	public async Task AddOrReplaceFileToTransactionAsync(
	Stream fileStream,
	string transactionId,
	string fileId,
	FileUploadOptions? uploadOptions)
		=> await AddOrReplaceFileToTransactionAsync(
			fileStream,
			transactionId,
			fileId,
			uploadOptions,
			default).ConfigureAwait(false);

	/// <inheritdoc />
	public async Task AddOrReplaceFileToTransactionAsync(
		Stream fileStream,
		string transactionId,
		string fileId,
		FileUploadOptions? uploadOptions,
		CancellationToken cancellationToken = default)
	{
		if (fileStream is null) {
			throw new ArgumentNullException(nameof(fileStream));
		}

		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		if (fileId is null) {
			throw new ArgumentNullException(nameof(fileId));
		}

		if (string.IsNullOrWhiteSpace(fileId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
		}

		if (uploadOptions is null) {
			uploadOptions = new FileUploadOptions();
		}

		var content = new StreamContent(fileStream)
			.WithDigest(fileStream, uploadOptions.DigestOptions);
		content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

		await client
			.PutAsync(
				"transaction".JoinPaths(transactionId, "file", fileId),
				content,
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync()
			.ConfigureAwait(false);
	}

	/// <inheritdoc cref="AddOrReplaceFileToTransactionAsync(Stream, string, string, FileUploadOptions)" />
	public Task AddOrReplaceFileToTransaction(
		Stream fileStream,
		string transactionId,
		string fileId)
	{
		return AddOrReplaceFileToTransactionAsync(
			fileStream,
			transactionId,
			fileId,
			null);
	}

	/// <inheritdoc />
	public async Task AddOrReplaceFileToTransactionAsync(
		string filePath,
		string transactionId,
		string fileId,
		FileUploadOptions? uploadOptions)
		=> await AddOrReplaceFileToTransactionAsync(
			filePath,
			transactionId,
			fileId,
			uploadOptions,
			default).ConfigureAwait(false);

	/// <inheritdoc />
	public async Task AddOrReplaceFileToTransactionAsync(
		string filePath,
		string transactionId,
		string fileId,
		FileUploadOptions? uploadOptions,
		CancellationToken cancellationToken = default)
	{
		if (filePath is null) {
			throw new ArgumentNullException(nameof(filePath));
		}

		using (Stream fileStream = System.IO.File.Open(
				filePath,
				FileMode.Open,
				FileAccess.Read,
				FileShare.Delete | FileShare.Read))
		{
			await AddOrReplaceFileToTransactionAsync(
					fileStream,
					transactionId,
					fileId,
					uploadOptions,
					cancellationToken)
				.ConfigureAwait(false);
		}
	}

	/// <inheritdoc cref="AddOrReplaceFileToTransactionAsync(string, string, string, FileUploadOptions)" />
	public Task AddOrReplaceFileToTransaction(
		string filePath,
		string transactionId,
		string fileId)
	{
		return AddOrReplaceFileToTransactionAsync(
			filePath,
			transactionId,
			fileId,
			null);
	}

	/// <inheritdoc />
	public async Task StartTransactionAsync(
		string transactionId)
		=> await StartTransactionAsync(transactionId, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task StartTransactionAsync(
		string transactionId,
		CancellationToken cancellationToken = default)
	{
		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		await client
			.PutAsync(
				"transaction".JoinPaths(transactionId, "start"),
				null,
				cancellationToken)
			.EnsureSignhostSuccessStatusCodeAsync()
			.ConfigureAwait(false);
	}

	/// <inheritdoc />
	public async Task<Stream> GetReceiptAsync(string transactionId)
		=> await GetReceiptAsync(transactionId, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task<Stream> GetReceiptAsync(
		string transactionId,
		CancellationToken cancellationToken = default)
	{
		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		var result = await client
			.GetStreamAsync(
				"file".JoinPaths("receipt", transactionId))
			.ConfigureAwait(false);

		return result;
	}

	/// <inheritdoc />
	public async Task<Stream> GetDocumentAsync(
		string transactionId,
		string fileId)
		=> await GetDocumentAsync(transactionId, fileId, default)
			.ConfigureAwait(false);

	/// <inheritdoc />
	public async Task<Stream> GetDocumentAsync(
		string transactionId,
		string fileId,
		CancellationToken cancellationToken = default)
	{
		if (transactionId is null) {
			throw new ArgumentNullException(nameof(transactionId));
		}

		if (string.IsNullOrWhiteSpace(transactionId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
		}

		if (fileId is null) {
			throw new ArgumentNullException(nameof(fileId));
		}

		if (string.IsNullOrWhiteSpace(fileId)) {
			throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
		}

		var result = await client
			.GetStreamAsync(
				"transaction".JoinPaths(transactionId, "file", fileId))
			.ConfigureAwait(false);

		return result;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Disposes the instance.
	/// </summary>
	/// <param name="disposing">Is <see cref="Dispose"/> callled.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing) {
			client?.Dispose();
		}
	}

	private static string? GetVersion()
		=> typeof(SignhostApiClient)
#if TYPEINFO
			.GetTypeInfo()
#endif
			?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()
			?.Version;
}
