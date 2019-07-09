using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Signhost.APIClient.Rest.DataObjects;

namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Interface abstracting the available Signhost API calls.
	/// </summary>
	public interface ISignHostApiClient
	{
		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">A transaction model.</param>
		/// <returns>A transaction object.</returns>
		Task<Transaction> CreateTransactionAsync(Transaction transaction);

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="transaction">A transaction model.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A transaction object.</returns>
		Task<Transaction> CreateTransactionAsync(
			Transaction transaction,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Adds meta data for a file to an existing transaction by providing a
		/// file location and a transaction id.
		/// </summary>
		/// <param name="fileMeta">Meta data for the file.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">An Id for the file. Should be the same as the fileId in the <see cref="AddOrReplaceFileToTransaction"/>.</param>
		/// <returns>A task.</returns>
		/// <remarks>Make sure to call this method before
		/// <see cref="AddOrReplaceFileToTransaction"/>.</remarks>
		Task AddOrReplaceFileMetaToTransactionAsync(
			FileMeta fileMeta,
			string transactionId,
			string fileId);

		/// <summary>
		/// Adds meta data for a file to an existing transaction by providing a
		/// file location and a transaction id.
		/// </summary>
		/// <param name="fileMeta">Meta data for the file.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">An Id for the file. Should be the same as the fileId in the <see cref="AddOrReplaceFileToTransaction"/>.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A task.</returns>
		/// <remarks>Make sure to call this method before
		/// <see cref="AddOrReplaceFileToTransaction"/>.</remarks>
		Task AddOrReplaceFileMetaToTransactionAsync(
			FileMeta fileMeta,
			string transactionId,
			string fileId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="fileStream">A Stream containing the file to upload.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced.</param>
		/// <param name="uploadOptions"><see cref="FileUploadOptions"/>.</param>
		/// <returns>A Task.</returns>
		Task AddOrReplaceFileToTransactionAsync(
			Stream fileStream,
			string transactionId,
			string fileId,
			FileUploadOptions uploadOptions);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="fileStream">A Stream containing the file to upload.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced.</param>
		/// <param name="uploadOptions"><see cref="FileUploadOptions"/>.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A Task.</returns>
		Task AddOrReplaceFileToTransactionAsync(
			Stream fileStream,
			string transactionId,
			string fileId,
			FileUploadOptions uploadOptions,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="filePath">A string representation of the file path.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced.</param>
		/// <param name="uploadOptions">Optional <see cref="FileUploadOptions"/>.</param>
		/// <returns>A Task.</returns>
		Task AddOrReplaceFileToTransactionAsync(
			string filePath,
			string transactionId,
			string fileId,
			FileUploadOptions uploadOptions);

		/// <summary>
		/// Add a file to a existing transaction by providing a file location
		/// and a transaction id.
		/// </summary>
		/// <param name="filePath">A string representation of the file path.</param>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A Id for the file. Using the file name is recommended.
		/// If a file with the same fileId allready exists the file wil be replaced.</param>
		/// <param name="uploadOptions">Optional <see cref="FileUploadOptions"/>.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A Task.</returns>
		Task AddOrReplaceFileToTransactionAsync(
			string filePath,
			string transactionId,
			string fileId,
			FileUploadOptions uploadOptions,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// start a existing transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <returns>A Task.</returns>
		Task StartTransactionAsync(string transactionId);

		/// <summary>
		/// start a existing transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A Task.</returns>
		Task StartTransactionAsync(
			string transactionId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets an exisiting transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction id for an existing transaction.</param>
		/// <returns>A <see cref="Transaction"/> object.</returns>
		Task<Transaction> GetTransactionAsync(string transactionId);

		/// <summary>
		/// Gets an exisiting transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction id for an existing transaction.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A <see cref="Transaction"/> object.</returns>
		Task<Transaction> GetTransactionAsync(
			string transactionId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <returns>A <see cref="ApiResponse{Transaction}"/> object.</returns>
		Task<ApiResponse<Transaction>> GetTransactionResponseAsync(string transactionId);

		/// <summary>
		/// Gets a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A <see cref="ApiResponse{Transaction}"/> object.</returns>
		Task<ApiResponse<Transaction>> GetTransactionResponseAsync(
			string transactionId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A Task.</returns>
		Task DeleteTransactionAsync(
			string transactionId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="options">Optional <see cref="DeleteTransactionOptions"/>.</param>
		/// <returns>A Task.</returns>
		Task DeleteTransactionAsync(
			string transactionId,
			DeleteTransactionOptions options);

		/// <summary>
		/// Deletes a existing transaction by providing a transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="options">Optional <see cref="DeleteTransactionOptions"/>.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>A Task.</returns>
		Task DeleteTransactionAsync(
			string transactionId,
			DeleteTransactionOptions options = default,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the signed document of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A valid file Id of a signed document.</param>
		/// <returns>Returns a stream containing the signed document data.</returns>
		Task<Stream> GetDocumentAsync(string transactionId, string fileId);

		/// <summary>
		/// Gets the signed document of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an existing transaction.</param>
		/// <param name="fileId">A valid file Id of a signed document.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>Returns a stream containing the signed document data.</returns>
		Task<Stream> GetDocumentAsync(
			string transactionId,
			string fileId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the receipt of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an finnished transaction.</param>
		/// <returns>Returns a stream containing the receipt data.</returns>
		Task<Stream> GetReceiptAsync(string transactionId);

		/// <summary>
		/// Gets the receipt of a finished transaction by providing transaction id.
		/// </summary>
		/// <param name="transactionId">A valid transaction Id of an finnished transaction.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>Returns a stream containing the receipt data.</returns>
		Task<Stream> GetReceiptAsync(
			string transactionId,
			CancellationToken cancellationToken = default);
	}
}
