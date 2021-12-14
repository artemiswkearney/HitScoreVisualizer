using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Loader;
using SiraUtil.Logging;
using SiraUtil.Zenject;
using Zenject;

namespace HitScoreVisualizer.UI
{
	internal class HitScoreFlowCoordinator : FlowCoordinator
	{
		private SiraLog _siraLog = null!;
		private string _pluginName = null!;
		private ConfigSelectorViewController _configSelectorViewController = null!;

		[Inject]
		internal void Construct(SiraLog siraLog, UBinder<Plugin, PluginMetadata> pluginMetadata, ConfigSelectorViewController configSelectorViewController)
		{
			_siraLog = siraLog;
			_pluginName = pluginMetadata.Value.Name;
			_configSelectorViewController = configSelectorViewController;
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			if (firstActivation)
			{
				SetTitle(_pluginName);
				showBackButton = true;

				ProvideInitialViewControllers(_configSelectorViewController);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			// Dismiss ourselves
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
		}
	}
}