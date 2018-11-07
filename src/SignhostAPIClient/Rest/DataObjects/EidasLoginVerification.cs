using System;
using System.Collections.Generic;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// Verification object for eIDAS.
	/// </summary>
	public class EidasLoginVerification
		: IVerification
	{
		/// <summary>
		/// Gets the <see cref="IVerification.Type"/>.
		/// </summary>
		public string Type { get; } = "eIDAS Login";

		/// <summary>
		/// Gets or sets the uid.
		/// </summary>
		public string Uid { get; set; }

		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		public Level? Level { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		public DateTime? DateOfBirth { get; set; }

		/// <summary>
		/// Gets or sets the eIDAS attributes.
		/// </summary>
		public IDictionary<string, string> Attributes { get; set; }
	}
}
