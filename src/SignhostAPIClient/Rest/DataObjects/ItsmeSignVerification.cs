namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Verification object for itsme sign.
	/// </summary>
	public class ItsmeSignVerification
		: IVerification
	{
		/// <summary>
		/// Gets the <see cref="IVerification.Type"/>.
		/// </summary>
		public string Type => "itsme sign";
	}
}
