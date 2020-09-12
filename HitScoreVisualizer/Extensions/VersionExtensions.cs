namespace HitScoreVisualizer.Extensions
{
	public static class VersionExtensions
	{
		public static SemVer.Version ToSemVerVersion(this System.Version systemVersion)
		{
			return new SemVer.Version(systemVersion.Major, systemVersion.Minor, systemVersion.Revision, build: systemVersion.Build.ToString());
		}
	}
}