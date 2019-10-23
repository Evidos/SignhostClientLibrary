using System;

namespace Signhost.APIClient.Rest.DataObjects
{
	[Obsolete("This verification is no longer supported and will be removed in SemVer 4.")]
	public class KennisnetVerification
		: IVerification
	{
		public string Type => "Kennisnet";
	}
}
