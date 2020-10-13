using BeatSaberMarkupLanguage;
using HitScoreVisualizer.UI;
using SiraUtil;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	public class MenuInstaller : Installer<MenuInstaller>
	{
		public override void InstallBindings()
		{
			Plugin.LoggerInstance.Debug($"Running {nameof(InstallBindings)} of {nameof(MenuInstaller)}");

			Container.BindViewController<ConfigSelectorViewController>(BeatSaberUI.CreateViewController<ConfigSelectorViewController>());
			Container.BindFlowCoordinator<HitScoreFlowCoordinator>(BeatSaberUI.CreateFlowCoordinator<HitScoreFlowCoordinator>());

			Plugin.LoggerInstance.Debug($"Binding {nameof(SettingsControllerManager)}");
			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle().NonLazy();

			Plugin.LoggerInstance.Debug($"All bindings installed in {nameof(MenuInstaller)}");
		}
	}
}