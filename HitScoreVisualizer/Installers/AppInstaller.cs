using HitScoreVisualizer.Services;
using HitScoreVisualizer.Settings;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	internal class AppInstaller : Installer<Logger, AppInstaller>
	{
		private readonly Logger _logger;

		internal AppInstaller(Logger logger)
		{
			_logger = logger;
		}

		public override void InstallBindings()
		{
			Container.BindLoggerAsSiraLogger(_logger);

			HSVConfig.Instance ??= IPA.Config.Config
				.GetConfigFor(Plugin.Name)
				.Generated<HSVConfig>();
			Container.BindInstance(HSVConfig.Instance);

			Container.BindInterfacesAndSelfTo<ConfigProvider>().AsSingle().NonLazy();
		}
	}
}