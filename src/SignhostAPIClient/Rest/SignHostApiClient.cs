using System;
using System.IO;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest
{
	public class SignHostApiClient
	{
		private readonly ISignHostApiClientSettings settings;

		public SignHostApiClient(ISignHostApiClientSettings settings)
		{
			this.settings = settings;

			FlurlHttp.Configure(c =>
			{
				c.OnError = ErrorHandling.ErrorHandling.HandleError;
			});
		}

		public SignHostApiClient(string appName, string appKey, string authkey, string apiUrl = SignHostApiClientSettings.DefaultEndpoint)
			: this(new SignHostApiClientSettings(appName, appKey, authkey) {
				Endpoint = apiUrl
			})
		{
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
		/// <returns>A transaction object</returns>
		public Task<Transaction> GetTransaction(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.GetAsync()
				.ReceiveJson<Transaction>();
		}

		/// <summary>
		/// Deletes a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <returns>A Task</returns>
		public Task DeleteTransaction(string transactionId)
		{
			if (transactionId == null) {
				throw new ArgumentNullException(nameof(transactionId));
			}

			if (string.IsNullOrWhiteSpace(transactionId)) {
				throw new ArgumentException("Cannot be empty or contain only whitespaces.", nameof(transactionId));
			}

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.DeleteAsync();
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
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutJsonAsync(fileMeta);
		}

		[Obsolete("Deprecated, use AddOrReplaceFileToTransaction instead")]
		public Task AddOrReplaceFileToTansaction(Stream fileStream, string transactionId, string fileId)
			=> AddOrReplaceFileToTransaction(fileStream, transactionId, fileId);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="fileStream">A Stream containing the file to upload</param>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced</param>
		/// <returns>A Task</returns>
		public Task AddOrReplaceFileToTransaction(Stream fileStream, string transactionId, string fileId)
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

			return settings.Endpoint
				.AppendPathSegments("transaction", transactionId)
				.AppendPathSegments("file", fileId)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutStreamAsync(fileStream, "application/pdf");
		}

		[Obsolete("Deprecated, use AddOrReplaceFileToTransaction instead")]
		public Task AddOrReplaceFileToTansaction(string filePath, string transactionId, string fileId)
			=> AddOrReplaceFileToTransaction(filePath, transactionId, fileId);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="filePath">A string representation of the file path.</param>
		/// <param name="transactionId">A valid transaction Id of an existing
		/// transaction</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced</param>
		/// <returns>A Task</returns>
		public async Task AddOrReplaceFileToTransaction(string filePath, string transactionId, string fileId)
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
				await AddOrReplaceFileToTransaction(fileStream, transactionId, fileId).ConfigureAwait(false);
			}
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
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.GetAsync()
				.ReceiveStream();
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
