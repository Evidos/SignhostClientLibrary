using System.IO;
using System.Reflection;
using System.Text;

namespace Signhost.APIClient.Rest.Tests.Responses
{
	public class Resources
	{
		public static string GetFileContent(string filename)
		{
			using var stream = typeof(Resources)
				.GetTypeInfo()
				.Assembly
				.GetManifestResourceStream($"Signhost.APIClient.Rest.Tests.Responses.{filename}");
			using var reader = new StreamReader(stream, Encoding.UTF8);
			return reader.ReadToEnd();
		}

		public static string CreateTeam => GetFileContent("CreateTeam.json");
		public static string AcceptInvite => GetFileContent("AcceptInvite.json");
		public static string DeleteMember => GetFileContent("DeleteMember.json");
		public static string RejectInvite => GetFileContent("RejectInvite.json");
		public static string InviteMember => GetFileContent("InviteMember.json");
		public static string RevokeInvite => GetFileContent("RevokeInvite.json");
		public static string GetTeamById => GetFileContent("GetTeamById.json");
		public static string GetTeams => GetFileContent("GetTeams.json");
	}
}
