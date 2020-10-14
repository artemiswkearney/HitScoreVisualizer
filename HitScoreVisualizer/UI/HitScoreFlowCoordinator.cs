using System;
using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace HitScoreVisualizer.UI
{
	internal class HitScoreFlowCoordinator : FlowCoordinator
	{
		private ConfigSelectorViewController? _configSelectorViewController;

		[Inject]
		internal void Construct(ConfigSelectorViewController configSelectorViewController)
		{
			_configSelectorViewController = configSelectorViewController;
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			try
			{
				if (firstActivation)
				{
					SetTitle(Plugin.Name);
					showBackButton = true;
					ProvideInitialViewControllers(_configSelectorViewController);
				}
			}
			catch (Exception ex)
			{
				Plugin.LoggerInstance.Error(ex);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			// Dismiss ourselves
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
		}
	}
}