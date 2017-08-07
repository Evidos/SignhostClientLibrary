using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Signhost.APIClient.Rest.DataObjects;

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
		private readonly FlurlClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SignHostApiClient"/> class.
		/// Set your usertoken and APPKey by creating a <see cref="SignHostApiClientSettings"/>.
		/// </summary>
		/// <param name="settings"><see cref="SignHostApiClientSettings"/></param>
		public SignHostApiClient(ISignHostApiClientSettings settings)
		{
			this.settings = settings;
			this.client = new FlurlClient(new FlurlHttpSettings {
				BeforeCall = call => {
					settings.AddHeader?.Invoke(call.Request.Headers.Add);
					call.Request.Headers.UserAgent.Add(
						new System.Net.Http.Headers.ProductInfoHeaderValue(
							"SignhostClientLibrary",
							Version));
				},
				OnError = ErrorHandling.ErrorHandling.HandleError,
			});
		}

		private string ApplicationHeader
			=> $"APPKey {settings.APPKey}";

		private string AuthorizationHeader
			=> $"APIKey {settings.APIKey}";

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">A transaction model</param>
		/// <returns>A transaction object</returns>
		public Task<Transaction> CreateTransaction(Transaction transaction)
		{
			if (transaction == null) {
				throw new ArgumentNullException(nameof(transaction));
			}

			return settings.Endpoint
				.AppendPathSegment("transaction")
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PostJsonAsync(transaction)
				.ReceiveJson<Transaction>();
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

			var responseMessage = await settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.WithClient(client)
				.WithHeaders(new {
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.AllowHttpStatus("410")
				.GetAsync()
				.ConfigureAwait(false);

			var transaction = JsonConvert.DeserializeObject<Transaction>(
					await responseMessage.Content.ReadAsStringAsync()
						.ConfigureAwait(false));

			return new ApiResponse<Transaction>(responseMessage, transaction);
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
		public Task DeleteTransaction(string transactionId, DeleteTransactionOptions options = null)
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

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.SendJsonAsync(System.Net.Http.HttpMethod.Delete, options).ReceiveString();
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
		public Task AddOrReplaceFileMetaToTransaction(FileMeta fileMeta, string transactionId, string fileId)
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

			if (!ValidPathSegment(fileId)) {
				throw new ArgumentException("Contains invalid character.", nameof(fileId));
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.AppendPathSegments("file", fileId)
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutJsonAsync(fileMeta);
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
		public Task AddOrReplaceFileToTransaction(
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

			if (!ValidPathSegment(fileId)) {
				throw new ArgumentException("Contains invalid character.", nameof(fileId));
			}

			if (uploadOptions == null) {
				uploadOptions = new FileUploadOptions();
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.AppendPathSegments("file", fileId)
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.WithDigest(fileStream, uploadOptions.DigestOptions)
				.PutStreamAsync(fileStream, "application/pdf");
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
		public Task StartTransaction(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId, "start")
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutAsync(null);
		}

		/// <summary>
		/// Gets the receipt of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an finnished
		/// transaction</param>
		/// <returns>Returns a stream containing the receipt data</returns>
		public Task<Stream> GetReceipt(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			return settings.Endpoint
				.AppendPathSegments("file", "receipt", transactionId)
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.GetAsync()
				.ReceiveStream();
		}

		/// <summary>
		/// Gets the signed document of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A valid file Id of a signed document</param>
		/// <returns>Returns a stream containing the signed document data</returns>
		public Task<Stream> GetDocument(string transactionId, string fileId)
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

			if (!ValidPathSegment(fileId)) {
				throw new ArgumentException("Contains invalid character.", nameof(fileId));
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.AppendPathSegments("file", fileId)
				.WithClient(client)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.GetAsync()
				.ReceiveStream();
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

		/// <summary>
		/// Flurl does some weird things when preparing the path segment making
		/// it impossible to encode / as %2F as Flurl removes the encoding.
		/// This method checks these unfortunate characters and returns false
		/// if such a character is found.
		/// </summary>
		/// <param name="segment">A URL segment</param>
		/// <returns>True when the path segment is valid.</returns>
		private bool ValidPathSegment(string segment)
		{
			return segment.IndexOf("/", StringComparison.OrdinalIgnoreCase) == -1;
		}
	}
}
