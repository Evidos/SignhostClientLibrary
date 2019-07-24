using System;

namespace Signhost.APIClient.Rest
{
	public class SignHostApiClientSettings
		: ISignHostApiClientSettings
	{
		public const string DefaultEndpoint = "https://api.signhost.com/api/";

		public SignHostApiClientSettings(string appkey, string apikey)
		{
			APPKey = appkey;
			APIKey = apikey;
		}

		[Obsolete("Obsoleted by UserToken")]
		public string APIKey { get; private set; }

		public string UserToken {
			get { return APIKey; }
			set { APIKey = value; }
		}

		public string APPKey { get; private set; }

		public string Endpoint { get; set; } = DefaultEndpoint;

		public Action<AddHeaders> AddHeader { get; set; }
	}
}
