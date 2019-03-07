namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Verification object for itsme Identification.
	/// </summary>
	public class ItsmeIdentificationVerification
		: IVerification
	{
		/// <summary>
		/// Gets the <see cref="IVerification.Type"/>.
		/// </summary>
		public string Type => "itsme Identification";

		/// <summary>
		/// Gets or sets the phonenumber.
		/// </summary>
		public string PhoneNumber { get; set; }
	}
}
