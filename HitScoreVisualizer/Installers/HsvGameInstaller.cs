using HitScoreVisualizer.Services;
using Zenject;

namespace HitScoreVisualizer.Installers
{
	internal sealed class HsvGameInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<JudgmentService>().AsSingle();
		}
	}
}