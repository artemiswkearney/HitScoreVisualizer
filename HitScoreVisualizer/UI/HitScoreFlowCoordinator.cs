using System;
using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Tools;
using Zenject;

namespace HitScoreVisualizer.UI
{
	internal class HitScoreFlowCoordinator : FlowCoordinator
	{
		private SiraLog _siraLog = null!;
		private ConfigSelectorViewController? _configSelectorViewController;

		[Inject]
		internal void Construct(SiraLog siraLog, ConfigSelectorViewController configSelectorViewController)
		{
			_siraLog = siraLog;
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
				_siraLog.Error(ex);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			// Dismiss ourselves
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
		}
	}
}