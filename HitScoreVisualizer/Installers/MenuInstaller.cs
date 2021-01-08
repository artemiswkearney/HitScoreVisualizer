using HitScoreVisualizer.UI;
using SiraUtil;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	internal class MenuInstaller : Installer<MenuInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<ConfigSelectorViewController>().FromNewComponentAsViewController().AsSingle();
			Container.BindFlowCoordinator<HitScoreFlowCoordinator>();

			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle().NonLazy();

		}
	}
}