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
	public class SignHostApiClient
		: IDisposable
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

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">A transaction model</param>
		/// <returns>A transaction object</returns>
		public async Task<Transaction> CreateTransaction(Transaction transaction)
		{
			if (transaction == null) {
				throw new ArgumentNullException(nameof(transaction));
			}

			var result = await client.PostAsync("transaction", JsonContent.From(transaction))
				.EnsureSignhostSuccessStatusCode()
				.ConfigureAwait(false);

			return await result.Content.FromJson<Transaction>()
				.ConfigureAwait(false);
		}

		/// <summary>
		/// Gets a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <returns>A <see cref="ApiResponse{Transaction}"/> object.
		public async Task<ApiResponse<Transaction>> GetTransactionResponse(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			var result = await client.GetAsync("transaction".JoinPaths(transactionId))
				.EnsureSignhostSuccessStatusCode(HttpStatusCode.Gone)
				.ConfigureAwait(false);
			var transaction = await result.Content.FromJson<Transaction>()
				.ConfigureAwait(false);

			return new ApiResponse<Transaction>(result, transaction);
		}

		/// <summary>
		/// Gets an exisiting transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction id for an existing
		/// transaction</param>
		/// <returns>A <see cref="Transaction"/> object.</returns>
		public async Task<Transaction> GetTransaction(string transactionId)
		{
			var response = await GetTransactionResponse(transactionId)
				.ConfigureAwait(false);

			response.EnsureAvailableStatusCode();

			return response.Value;
		}

		/// <summary>
		/// Deletes a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="options">Optional <see cref="DeleteTransactionOptions"/>.</param>
		/// <returns>A Task</returns>
		public async Task DeleteTransaction(string transactionId, DeleteTransactionOptions options = null)
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
			var result = await client.SendAsync(request)
				.EnsureSignhostSuccessStatusCode()
				.ConfigureAwait(false);
		}

		/// <summary>
		/// Adds meta data for a file to an existing transaction by providing a
		/// file location and a transaction id.
		/// </summary>
		/// <param name="fileMeta">Meta data for the file</param>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">An Id for the file. Should be the same
		/// as the fileId in the <see cref="AddOrReplaceFileToTransaction"/>.</param>
		/// <returns>A task</returns>
		/// <remarks>Make sure to call this method before
		/// <see cref="AddOrReplaceFileToTransaction"/>.</remarks>
		public async Task AddOrReplaceFileMetaToTransaction(FileMeta fileMeta, string transactionId, string fileId)
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

			var result = await client.PutAsync(
					"transaction".JoinPaths(transactionId, "file", fileId),
					JsonContent.From(fileMeta))
				.EnsureSignhostSuccessStatusCode()
				.ConfigureAwait(false);
		}

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="fileStream">A Stream containing the file to upload</param>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced</param>
		/// <param name="uploadOptions"><see cref="FileUploadOptions"/></param>
		/// <returns>A Task</returns>
		public async Task AddOrReplaceFileToTransaction(
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

			var result = await client.PutAsync(
					"transaction".JoinPaths(transactionId, "file", fileId),
					content)
				.EnsureSignhostSuccessStatusCode()
				.ConfigureAwait(false);
		}

		/// <inheritdoc cref="AddOrReplaceFileToTransaction(Stream, string, string, FileUploadOptions)" />
		public Task AddOrReplaceFileToTransaction(
			Stream fileStream,
			string transactionId,
			string fileId)
		{
			return AddOrReplaceFileToTransaction(
				fileStream,
				transactionId,
				fileId,
				null);
		}

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="filePath">A string representation of the file path.</param>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced</param>
		/// <param name="uploadOptions">Optional <see cref="FileUploadOptions"/></param>
		/// <returns>A Task</returns>
		public async Task AddOrReplaceFileToTransaction(
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
				await AddOrReplaceFileToTransaction(
						fileStream,
						transactionId,
						fileId,
						uploadOptions)
					.ConfigureAwait(false);
			}
		}

		/// <inheritdoc cref="AddOrReplaceFileToTransaction(string, string, string, FileUploadOptions)" />
		public Task AddOrReplaceFileToTransaction(
			string filePath,
			string transactionId,
			string fileId)
		{
			return AddOrReplaceFileToTransaction(
				filePath,
				transactionId,
				fileId,
				null);
		}

		/// <summary>
		/// start a existing transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <returns>A Task</returns>
		public async Task StartTransaction(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			var result = await client.PutAsync(
					"transaction".JoinPaths(transactionId, "start"),
					null)
				.EnsureSignhostSuccessStatusCode()
				.ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the receipt of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an finnished
		/// transaction</param>
		/// <returns>Returns a stream containing the receipt data</returns>
		public async Task<Stream> GetReceipt(string transactionId)
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

		/// <summary>
		/// Gets the signed document of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A valid file Id of a signed document</param>
		/// <returns>Returns a stream containing the signed document data</returns>
		public async Task<Stream> GetDocument(string transactionId, string fileId)
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

			if (string.IsNullOrWhiteSpace(transactionId)) {
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
