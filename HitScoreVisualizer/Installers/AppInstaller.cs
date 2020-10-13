using HitScoreVisualizer.Services;
using HitScoreVisualizer.Settings;
using IPA.Config.Stores;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	public class AppInstaller : Installer<AppInstaller>
	{
		public override void InstallBindings()
		{
			Plugin.LoggerInstance.Debug($"Running {nameof(InstallBindings)} of {nameof(AppInstaller)}");

			Plugin.LoggerInstance.Debug($"Binding {nameof(HSVConfig)}");
			HSVConfig.Instance ??= IPA.Config.Config
				.GetConfigFor(Plugin.Name)
				.Generated<HSVConfig>();
			Container.BindInstance(HSVConfig.Instance);

			Plugin.LoggerInstance.Debug($"Binding {nameof(ConfigProvider)}");
			Container.BindInterfacesAndSelfTo<ConfigProvider>().AsSingle().NonLazy();

			Plugin.LoggerInstance.Debug($"All bindings installed in {nameof(AppInstaller)}");
		}
	}
}