using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.DataObjects;
using Signhost.APIClient.Rest.ErrorHandling;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Implements the <see cref="ISignHostApiClient"/> interface which provides
	/// an signhost api client implementation.
	/// </summary>
	public class SignHostApiClient
		: ISignHostApiClient
		, IDisposable
	{
		private static readonly string Version = typeof(SignHostApiClient)
			.GetTypeInfo()
			.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()
			.Version;

		private readonly ISignHostApiClientSettings settings;
		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SignHostApiClient"/> class.
		/// Set your usertoken and APPKey by creating a <see cref="SignHostApiClientSettings"/>.
		/// </summary>
		/// <param name="settings"><see cref="SignHostApiClientSettings"/></param>
		public SignHostApiClient(ISignHostApiClientSettings settings)
			: this(settings, new HttpClient())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SignHostApiClient"/> class.
		/// Set your usertoken and APPKey by creating a <see cref="SignHostApiClientSettings"/>.
		/// </summary>
		/// <param name="settings"><see cref="SignHostApiClientSettings"/></param>
		/// <param name="httpClient"><see cref="HttpClient"/> to use for all http calls.</param>
		public SignHostApiClient(ISignHostApiClientSettings settings, HttpClient httpClient)
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
			this.client.DefaultRequestHeaders.Add("Authorization", AuthorizationHeader);
			settings.AddHeader?.Invoke(this.client.DefaultRequestHeaders.Add);
		}

		private string ApplicationHeader
			=> $"APPKey {settings.APPKey}";

		private string AuthorizationHeader
			=> $"APIKey {settings.APIKey}";

		/// <summary>
		/// Globally register an additional verification type.
		/// </summary>
		/// <typeparam name="T"><see cref="IVerification"/> to </typeparam>
		public static void RegisterVerification<T>()
			where T : IVerification
		{
			JsonConverters.JsonVerificationConverter.RegisterVerification<T>();
		}

		/// <inheritdoc />
		public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
		{
			if (transaction == null) {
				throw new ArgumentNullException(nameof(transaction));
			}

			var result = await client.PostAsync("transaction", JsonContent.From(transaction))
				.EnsureSignhostSuccessStatusCodeAsync()
				.ConfigureAwait(false);

			return await result.Content.FromJsonAsync<Transaction>()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<ApiResponse<Transaction>> GetTransactionResponseAsync(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			var result = await client.GetAsync("transaction".JoinPaths(transactionId))
				.EnsureSignhostSuccessStatusCodeAsync(HttpStatusCode.Gone)
				.ConfigureAwait(false);
			var transaction = await result.Content.FromJsonAsync<Transaction>()
				.ConfigureAwait(false);

			return new ApiResponse<Transaction>(result, transaction);
		}

		/// <inheritdoc />
		public async Task<Transaction> GetTransactionAsync(string transactionId)
		{
			var response = await GetTransactionResponseAsync(transactionId)
				.ConfigureAwait(false);

			response.EnsureAvailableStatusCode();

			return response.Value;
		}

		/// <inheritdoc />
		public async Task DeleteTransactionAsync(string transactionId, DeleteTransactionOptions options = null)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			if (options == null) {
				options = new DeleteTransactionOptions();
			}

			var request = new HttpRequestMessage(HttpMethod.Delete, "transaction".JoinPaths(transactionId));
			request.Content = JsonContent.From(options);
			await client.SendAsync(request)
				.EnsureSignhostSuccessStatusCodeAsync()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task AddOrReplaceFileMetaToTransactionAsync(FileMeta fileMeta, string transactionId, string fileId)
		{
			if (fileMeta == null) {
				throw new ArgumentNullException("fileMeta");
			}

			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			if (fileId == null) {
				throw new ArgumentNullException(nameof(fileId));
			}

			if (string.IsNullOrWhiteSpace(fileId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
			}

			await client.PutAsync(
					"transaction".JoinPaths(transactionId, "file", fileId),
					JsonContent.From(fileMeta))
				.EnsureSignhostSuccessStatusCodeAsync()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task AddOrReplaceFileToTransactionAsync(
			Stream fileStream,
			string transactionId,
			string fileId,
			FileUploadOptions uploadOptions)
		{
			if (fileStream == null) {
				throw new ArgumentNullException(nameof(fileStream));
			}

			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			if (fileId == null) {
				throw new ArgumentNullException(nameof(fileId));
			}

			if (string.IsNullOrWhiteSpace(fileId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
			}

			if (uploadOptions == null) {
				uploadOptions = new FileUploadOptions();
			}

			var content = new StreamContent(fileStream)
				.WithDigest(fileStream, uploadOptions.DigestOptions);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			await client.PutAsync(
					"transaction".JoinPaths(transactionId, "file", fileId),
					content)
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
			FileUploadOptions uploadOptions)
		{
			if (filePath == null) {
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
						uploadOptions)
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
		public async Task StartTransactionAsync(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			await client.PutAsync(
					"transaction".JoinPaths(transactionId, "start"),
					null)
				.EnsureSignhostSuccessStatusCodeAsync()
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<Stream> GetReceiptAsync(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			var result = await client.GetStreamAsync(
					"file".JoinPaths("receipt", transactionId))
				.ConfigureAwait(false);

			return result;
		}

		/// <inheritdoc />
		public async Task<Stream> GetDocumentAsync(string transactionId, string fileId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			if (fileId == null) {
				throw new ArgumentNullException(nameof(fileId));
			}

			if (string.IsNullOrWhiteSpace(fileId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(fileId));
			}

			var result = await client.GetStreamAsync(
					"transaction".JoinPaths(transactionId, "file", fileId))
				.ConfigureAwait(false);

			return result;
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
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
	}
}
