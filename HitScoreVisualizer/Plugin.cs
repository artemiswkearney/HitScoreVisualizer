using HarmonyLib;
using System.Reflection;
using HitScoreVisualizer.Extensions;
using HitScoreVisualizer.Installers;
using HitScoreVisualizer.Settings;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Attributes;
using SiraUtil.Zenject;
using Version = SemVer.Version;

namespace HitScoreVisualizer
{
	[Slog]
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		private const string HARMONY_ID = "be.erisapps.HitScoreVisualizer";

		private static Harmony? _harmonyInstance;
		private static PluginMetadata? _metadata;
		private static string? _name;
		private static Version? _version;

		public static string Name => _name ??= _metadata?.Name ?? Assembly.GetExecutingAssembly().GetName().Name;
		public static Version Version => _version ??= _metadata?.Version ?? Assembly.GetExecutingAssembly().GetName().Version.ToSemVerVersion();

		[Init]
		public void Init(Logger logger, Config config, PluginMetadata pluginMetadata, Zenjector zenject)
		{
			_metadata = pluginMetadata;

			zenject.OnApp<HsvAppInstaller>().WithParameters(logger, config.Generated<HSVConfig>());
			zenject.OnMenu<HsvMenuInstaller>();
		}

		[OnEnable]
		public void OnEnable()
		{
			_harmonyInstance = new Harmony(HARMONY_ID);
			_harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}

		[OnDisable]
		public void OnDisable()
		{
			_harmonyInstance?.UnpatchAll(HARMONY_ID);
			_harmonyInstance = null;
		}
	}
}