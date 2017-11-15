using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Adds a consent verification screen
	/// </summary>
	public class IPAddressVerification
		: IVerification
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IPAddressVerification"/> class.
		/// Do not use!
		/// </summary>
		[Obsolete("This constructor is for internal usage only!")]
		public IPAddressVerification()
		{
		}

		/// <summary>
		/// Gets the <see cref="IVerification.Type"/>.
		/// </summary>
		public string Type { get; } = "IPAddress";

		/// <summary>
		/// Gets or sets the IP Address used by the signer while signing the documents.
		/// </summary>
		public string IPAddress { get; set; }
	}
}
