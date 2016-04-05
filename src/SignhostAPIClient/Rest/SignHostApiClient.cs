using System.Threading.Tasks;
using System.IO;
using Flurl;
using Flurl.Http;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest
{
	public class SignHostApiClient
	{
		private const string ApiUrl = "https://api.signhost.com/api/";

		private readonly string applicationNameKey;
		private readonly string apiKey;

		public SignHostApiClient(string appNameKey, string authKey)
		{
			applicationNameKey = appNameKey;
			apiKey = authKey;

			FlurlHttp.Configure(c =>
			{
				c.OnError = ErrorHandling.ErrorHandling.HandleError;
			});
		}

		public SignHostApiClient(string appName, string appKey, string authkey)
		{
			new SignHostApiClient($"{appName} {appKey}", authkey);
		}

		private string ApplicationHeader
			=> $"APPKey {applicationNameKey}";

		private string AuthorizationHeader
			=> $"APIKey {apiKey}";

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">A transaction model</param>
		/// <returns>A transaction object</returns>
		public Task<Transaction> CreateTransaction(Transaction transaction)
		{
			return ApiUrl
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
			return ApiUrl
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
			return ApiUrl
				.AppendPathSegments("transaction", transactionId)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.DeleteAsync();
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
		/// <returns>A Task</returns>
		public Task AddOrReplaceFileToTansaction(Stream fileStream, string transactionId, string fileId)
		{
			return ApiUrl
				.AppendPathSegments("transaction", transactionId)
				.AppendPathSegments("file", fileId)
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutStreamAsync(fileStream);
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
		/// <returns>A Task</returns>
		public async Task AddOrReplaceFileToTansaction(string filePath, string transactionId, string fileId)
		{
			using (Stream fileStream = System.IO.File.Open(
					filePath,
					FileMode.Open,
					FileAccess.Read,
					FileShare.Delete | FileShare.Read))
			{
				await AddOrReplaceFileToTansaction(fileStream, transactionId, fileId).ConfigureAwait(false);
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
			return ApiUrl
				.AppendPathSegments("transaction", transactionId, "start")
				.WithHeaders(new
				{
					Application = ApplicationHeader,
					Authorization = AuthorizationHeader
				})
				.PutAsync();
		}

		/// <summary>
		/// Gets the receipt of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an finnished
		/// transaction</param>
		/// <returns>Returns a stream containing the receipt data</returns>
		public Task<Stream> GetReceipt(string transactionId)
		{
			return ApiUrl
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
			return ApiUrl
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
	}
}
