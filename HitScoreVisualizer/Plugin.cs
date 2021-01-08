using HarmonyLib;
using System.Reflection;
using HitScoreVisualizer.Extensions;
using HitScoreVisualizer.Installers;
using IPA;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Zenject;
using Version = SemVer.Version;

namespace HitScoreVisualizer
{
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
		public void Init(Logger logger, PluginMetadata pluginMetadata, Zenjector zenject)
		{
			_metadata = pluginMetadata;

			zenject.OnApp<AppInstaller>().WithParameters(logger);
			zenject.OnMenu<Installers.MenuInstaller>();
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