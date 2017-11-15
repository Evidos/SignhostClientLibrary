namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Options to be used during a file upload
	/// </summary>
	public class FileUploadOptions
	{
		/// <summary>
		/// Gets or sets the <see cref="FileDigestOptions"/>.
		/// </summary>
		public FileDigestOptions DigestOptions { get; set; }
			= new FileDigestOptions();
	}
}
