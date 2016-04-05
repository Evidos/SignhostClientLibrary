using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest
{
	public interface ISignHostApiClientSettings
	{
		string APIKey { get; }

		string APPKey { get; }

		string Endpoint { get; }
	}
}
