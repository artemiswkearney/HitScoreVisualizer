using HarmonyLib;
using System.Reflection;
using HitScoreVisualizer.Extensions;
using HitScoreVisualizer.Installers;
using IPA;
using IPA.Loader;
using IPA.Logging;
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

		internal static Logger LoggerInstance { get; private set; } = null!;

		public static string Name => _name ??= _metadata?.Name ?? Assembly.GetExecutingAssembly().GetName().Name;
		public static Version Version => _version ??= _metadata?.Version ?? Assembly.GetExecutingAssembly().GetName().Version.ToSemVerVersion();

		[Init]
		public void Init(Logger log, PluginMetadata pluginMetadata)
		{
			LoggerInstance = log;
			_metadata = pluginMetadata;
		}

		[OnEnable]
		public void OnEnable()
		{
			SiraUtil.Zenject.Installer.RegisterAppInstaller<AppInstaller>();
			SiraUtil.Zenject.Installer.RegisterMenuInstaller<Installers.MenuInstaller>();

			_harmonyInstance = new Harmony(HARMONY_ID);
			_harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}

		[OnDisable]
		public void OnDisable()
		{
			SiraUtil.Zenject.Installer.UnregisterAppInstaller<AppInstaller>();
			SiraUtil.Zenject.Installer.UnregisterMenuInstaller<Installers.MenuInstaller>();

			_harmonyInstance?.UnpatchAll(HARMONY_ID);
			_harmonyInstance = null;
		}
	}
}