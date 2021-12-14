using HitScoreVisualizer.Installers;
using HitScoreVisualizer.Settings;
using Hive.Versioning;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Zenject;

namespace HitScoreVisualizer
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		private static PluginMetadata? _metadata;

		public static string Name => _metadata?.Name!;
		public static Version Version => _metadata?.HVersion!;

		[Init]
		public void Init(Logger logger, Config config, PluginMetadata pluginMetadata, Zenjector zenject)
		{
			_metadata = pluginMetadata;

			zenject.UseLogger(logger);
			zenject.Install<HsvAppInstaller>(Location.App, config.Generated<HSVConfig>());
			zenject.Install<HsvMenuInstaller>(Location.Menu);
			zenject.Install<HsvGameInstaller>(Location.Player);
		}

		[OnEnable, OnDisable]
		public void OnStateChanged()
		{
		}
	}
}