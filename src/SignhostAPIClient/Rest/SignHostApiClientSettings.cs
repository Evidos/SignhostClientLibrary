using System;

namespace Signhost.APIClient.Rest
{
	public class SignHostApiClientSettings
		: ISignHostApiClientSettings
	{
		public const string DefaultEndpoint = "https://api.signhost.com/api/";

		public SignHostApiClientSettings(string appkey, string userToken)
		{
			APPKey = appkey;
			UserToken = userToken;
		}

		public SignHostApiClientSettings(string appkey)
		{
			APPKey = appkey;
		}

		public string UserToken { get; set; }

		public string APPKey { get; private set; }

		public string Endpoint { get; set; } = DefaultEndpoint;

		public Action<AddHeaders> AddHeader { get; set; }
	}
}
