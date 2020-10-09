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
		}

		public void Initialize()
		{
			_hsvButton = new MenuButton(Plugin.Name, "Select the config you want.", OnClick);
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

			// ReSharper disable once Unity.NoNullPropagation
			MenuButtons.instance?.UnregisterButton(_hsvButton);

			_hsvButton = null!;
		}
	}
}