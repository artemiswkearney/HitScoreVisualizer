using BeatSaberMarkupLanguage;
using HitScoreVisualizer.UI;
using SiraUtil.Zenject;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	[RequiresInstaller(typeof(AppInstaller))]
	public class MenuInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Plugin.LoggerInstance.Debug($"Running {nameof(InstallBindings)} of {nameof(MenuInstaller)}");

			var configSelectorViewController = BeatSaberUI.CreateViewController<ConfigSelectorViewController>();
			var hitScoreFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<HitScoreFlowCoordinator>();

			Container.InjectSpecialInstance<ConfigSelectorViewController>(configSelectorViewController);
			Container.InjectSpecialInstance<HitScoreFlowCoordinator>(hitScoreFlowCoordinator);

			Plugin.LoggerInstance.Debug($"Binding {nameof(SettingsControllerManager)}");
			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle().NonLazy();

			Plugin.LoggerInstance.Debug($"All bindings installed in {nameof(MenuInstaller)}");
		}
	}
}