namespace Signhost.APIClient.Rest.Tests
{
	public partial class TeamsApiClientTests
	{
		private static readonly SignHostApiClientSettings settings =
			new SignHostApiClientSettings("AppKey", "AuthKey") {
				Endpoint = "http://localhost/api/"
			};

		/// <summary>
		/// AcceptInvite and RejectInvite are performed by a *different* user
		/// then i.e. CreateTeam, RevokeInvite. This has no effect on the tests,
		/// but for clarity's sake these methods get a different settings
		/// instance.
		/// </summary>
		private static readonly SignHostApiClientSettings settingsOtherUser =
			new SignHostApiClientSettings("AppKey", "AuthKey") {
				Endpoint = "http://localhost/api/"
			};
	}
}