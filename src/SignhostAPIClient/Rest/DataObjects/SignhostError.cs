using Newtonsoft.Json;

namespace Signhost.APIClient.Rest.DataObjects
{
	/// <summary>
	/// An class representing a Signhost Error response.
	/// </summary>
	public class SignhostError
	{
		/// <summary>
		/// Gets or sets the type of error.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the message describing the error.
		/// </summary>
		public string Message { get; set; }
	}
}
