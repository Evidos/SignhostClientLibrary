using System;
using System.Linq;

namespace Signhost.APIClient.Rest;

internal static class UriPathExtensions
{
	internal static Uri JoinPaths(
		this string url,
		params string[] segments)
	{
		var segmentList = segments.ToList();
		segmentList.Insert(0, url);
		var escaped = segmentList.Select(seg => Uri.EscapeDataString(seg));
		return new Uri(string.Join("/", escaped), UriKind.Relative);
	}
}
