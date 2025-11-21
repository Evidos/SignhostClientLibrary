namespace Signhost.APIClient.Rest
{
	/// <summary>
	/// Registers the necessary settings for the <see cref="SignhostApiReceiver"/> class.
	/// </summary>
	public class SignhostApiReceiverSettings
	{
		public SignhostApiReceiverSettings(string sharedsecret)
		{
			SharedSecret = sharedsecret;
		}

		/// <summary>
		/// Gets the shared secret.
		/// </summary>
		/// <value>The shared secret key issued by Signhost.com.</value>
		public string SharedSecret { get; private set; }
	}
}
