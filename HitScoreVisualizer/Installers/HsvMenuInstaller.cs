using HitScoreVisualizer.UI;
using SiraUtil;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	internal class HsvMenuInstaller : Installer<HsvMenuInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<ConfigSelectorViewController>().FromNewComponentAsViewController().AsSingle();
			Container.Bind<HitScoreFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
			Container.BindInterfacesTo<SettingsControllerManager>().AsSingle();
		}
	}
}