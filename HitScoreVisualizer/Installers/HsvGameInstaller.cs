using HitScoreVisualizer.Services;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	public class HsvGameInstaller : Installer<HsvGameInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<JudgmentService>().AsSingle();
		}
	}
}