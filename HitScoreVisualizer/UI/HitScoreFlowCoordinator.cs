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
		private UBinder<Plugin, PluginMetadata> _pluginMetadata = null!;
		private ConfigSelectorViewController _configSelectorViewController = null!;

		[Inject]
		internal void Construct(SiraLog siraLog, UBinder<Plugin, PluginMetadata> pluginMetadata, ConfigSelectorViewController configSelectorViewController)
		{
			_pluginMetadata = pluginMetadata;
			_siraLog = siraLog;
			_configSelectorViewController = configSelectorViewController;
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			if (firstActivation)
			{
				SetTitle(_pluginMetadata.Value.Name);
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