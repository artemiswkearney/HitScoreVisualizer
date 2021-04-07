using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace HitScoreVisualizer.UI
{
	internal class SettingsControllerManager : IInitializable, IDisposable
	{
		private readonly HitScoreFlowCoordinator _hitScoreFlowCoordinator;
		private MenuButton? _hsvButton;

		[Inject]
		public SettingsControllerManager(HitScoreFlowCoordinator hitScoreFlowCoordinator)
		{
			_hitScoreFlowCoordinator = hitScoreFlowCoordinator;
			_hsvButton = new MenuButton($"<size=89.5%>{Plugin.Name}", "Select the config you want.", OnClick);
		}

		public void Initialize()
		{
			MenuButtons.instance.RegisterButton(_hsvButton);
		}

		private void OnClick()
		{
			if (_hitScoreFlowCoordinator == null)
			{
				return;
			}

			BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_hitScoreFlowCoordinator);
		}

		public void Dispose()
		{
			if (_hsvButton == null)
			{
				return;
			}

			if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
			{
				MenuButtons.instance.UnregisterButton(_hsvButton);
			}


			_hsvButton = null!;
		}
	}
}