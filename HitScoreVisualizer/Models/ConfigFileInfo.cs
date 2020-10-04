using System;
using HitScoreVisualizer.Settings;
using Version = SemVer.Version;

namespace HitScoreVisualizer.Models
{
	internal class ConfigFileInfo
	{
		public ConfigFileInfo(string fileName, string filePath)
		{
			ConfigName = fileName;
			ConfigPath = filePath;
		}
		public string ConfigName { get; }

		public string ConfigPath { get; }

		public Configuration? Configuration { get; set; }
		public ConfigState State { get; set; }
		public Version? Version => Configuration?.Version;
	}
}