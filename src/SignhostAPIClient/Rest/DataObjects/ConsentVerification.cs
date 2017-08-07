using System;
using System.Collections.Generic;
using System.Text;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Adds a consent verification screen
	/// </summary>
	public class ConsentVerification
		: IVerification
	{
		/// <summary>
		/// Gets the <see cref="IVerification.Type"/>.
		/// </summary>
		public string Type { get; } = "Consent";
	}
}
