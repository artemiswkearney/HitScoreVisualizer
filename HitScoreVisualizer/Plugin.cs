using HarmonyLib;
using System;
using System.Reflection;
using HitScoreVisualizer.Extensions;
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

		internal static Logger Logger;

		public static string Name => _name ??= _metadata?.Name ?? Assembly.GetExecutingAssembly().GetName().Name;
		public static Version Version => _version ??= _metadata?.Version ?? Assembly.GetExecutingAssembly().GetName().Version.ToSemVerVersion();

		internal const int majorVersion = 2;
		internal const int minorVersion = 4;
		internal const int patchVersion = 4;

		[Init]
		public void Init(Logger logger, PluginMetadata pluginMetadata)
		{
			Logger = logger;
			_metadata = pluginMetadata;
		}

		[OnEnable]
		public void OnEnable()
		{
			try
			{
				_harmonyInstance = new Harmony(HARMONY_ID);
				_harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			}
			catch (Exception e)
			{
				Logger.Warn("[HitScoreVisualizer] This plugin requires Harmony. Make sure you " +
				            "installed the plugin properly, as the Harmony DLL should have been installed with it.");
				Logger.Error(e);
			}

			Config.Load();
		}

		[OnDisable]
		public void OnDisable()
		{
			try
			{
				_harmonyInstance?.UnpatchAll(HARMONY_ID);
			}
			catch (Exception e)
			{
				Logger.Warn("[HitScoreVisualizer] This plugin requires Harmony. Make sure you " +
				            "installed the plugin properly, as the Harmony DLL should have been installed with it.");
				Logger.Error(e);
			}
		}

		internal static void log(object message)
		{
#if DEBUG
			Console.WriteLine("[HitScoreVisualizer] " + message);
#endif
		}
	}
}