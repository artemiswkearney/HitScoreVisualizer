using System.Collections.Generic;
using System.IO;
using HitScoreVisualizer.Settings;
using IPA.Config.Stores;

namespace HitScoreVisualizer.Services
{
	internal class ConfigProvider
	{
		private static string HsvConfigsFolderName { get; } = nameof(HitScoreVisualizer);
		private static string HsvConfigsFolderPath { get; } = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, nameof(HitScoreVisualizer));

		internal static Settings.Config CurrentConfig { get; set; } = null!;

		internal static void Load()
		{


			if (!Directory.Exists(HsvConfigsFolderPath))
			{
				Directory.CreateDirectory(HsvConfigsFolderPath);
			}

			var config = IPA.Config.Config
				.GetConfigFor(Path.Combine(HsvConfigsFolderName, "HitScoreVisualizerConfig(Zilian's,DeleteBracketsForUsage)"))
				//.GetConfigFor("HitScoreVisualizerConfig")
				.Generated<Settings.Config>();

			if (Validate(config))
			{
				CurrentConfig = config;
			}
			else
			{
				CurrentConfig = null!;
			}
		}

		internal static Dictionary<string, string> ListAvailableConfigs()
		{
			return Directory
				.GetFiles(HsvConfigsFolderPath)
				.ToDictionary(x => x, x => Path.GetFileName(x));
		}

		internal static Settings.Config LoadInternal(string path)
		{
			return Jsonconv
		}

		private static bool Validate(Settings.Config config)
		{
			if (TooNew(config))
			{
				return false;
			}

			var judgmentsValid = true;
			foreach (var j in config.Judgments)
			{
				if (!ValidateJudgment(j))
				{
					judgmentsValid = false;
				}
			}

			return judgmentsValid;
		}

		private static bool ValidateJudgment(Judgment judgment)
		{
			if (judgment.Color.Count != 4)
			{
				Plugin.LoggerInstance.Warn($"Judgment \"{judgment.Text}\" with threshold {judgment.Threshold} has invalid color!");
				Plugin.LoggerInstance.Warn("Make sure to include exactly 4 numbers for each judgment's color!");
				return false;
			}

			return true;
		}

		private static bool Outdated(Settings.Config config)
		{
			if (config.MajorVersion < Plugin.Version.Major)
			{
				return true;
			}

			return config.MinorVersion < Plugin.Version.Minor;
		}

		private static bool TooNew(Settings.Config config)
		{
			if (config.MajorVersion > Plugin.Version.Major)
			{
				return true;
			}

			return config.MinorVersion > Plugin.Version.Minor;
		}
	}
}