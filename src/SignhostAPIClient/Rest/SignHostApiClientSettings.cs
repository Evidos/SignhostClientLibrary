using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public string APIKey { get; private set; }

		public string APPKey { get; private set; }

		public string Endpoint { get; set; } = DefaultEndpoint;

		public Action<AddHeaders> AddHeader { get; set; }
	}
}
