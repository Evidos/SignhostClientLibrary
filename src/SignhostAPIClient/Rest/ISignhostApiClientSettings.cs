using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest
{
	public interface ISignHostApiClientSettings
	{
		/// <summary>
		/// Gets the usertoken identifying an authorized user.
		/// </summary>
		string APIKey { get; }

		/// <summary>
		/// Gets the app key of your applications.
		/// </summary>
		string APPKey { get; }

		string SharedSecret { get; }

		/// <summary>
		/// Gets the signhost API endpoint.
		/// </summary>
		string Endpoint { get; }

		Action<AddHeaders> AddHeader { get; }
	}

	public delegate void AddHeaders(string name, string value);
}
