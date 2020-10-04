using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HitScoreVisualizer.Models;
using HitScoreVisualizer.Services;
using HMUI;
using IPA.Utilities.Async;
using Zenject;

namespace HitScoreVisualizer.UI
{
	[HotReload(RelativePathToLayout = @"Views\ConfigSelector.bsml")]
	[ViewDefinition("HitScoreVisualizer.UI.Views.ConfigSelector.bsml")]
	internal class ConfigSelectorViewController : BSMLAutomaticViewController
	{
		private ConfigProvider _configProvider = null!;

		private ConfigFileInfo? _selectedConfigFileInfo;

		public ConfigSelectorViewController()
		{
			AvailableConfigs = new List<object>();
		}

		[Inject]
		internal void Construct(ConfigProvider configProvider)
		{
			_configProvider = configProvider;
		}

		[UIComponent("configs-list")]
		public CustomCellListTableData? customListTableData;

		[UIValue("available-configs")]
		internal List<object> AvailableConfigs { get; }

		[UIValue("loading-available-configs")]
		internal bool LoadingConfigs { get; private set; }

		[UIValue("has-loaded-available-configs")]
		internal bool HasLoadedConfigs => !LoadingConfigs;

		[UIValue("is-valid-config-selected")]
		internal bool CanConfigGetSelected => _selectedConfigFileInfo?.ConfigPath != _configProvider.CurrentConfigPath && _configProvider.ConfigSelectable(_selectedConfigFileInfo?.State);

		[UIValue("has-config-loaded")]
		internal bool HasConfigCurrently => !string.IsNullOrWhiteSpace(_configProvider.CurrentConfigPath);

		[UIValue("config-loaded-text")]
		internal string LoadedConfigText => $"Currently loaded config: <size=80%>{(HasConfigCurrently ? Path.GetFileNameWithoutExtension(_configProvider.CurrentConfigPath) : "None")}";

		[UIAction("config-Selected")]
		internal void Select(TableView tableView, object @object)
		{
			_selectedConfigFileInfo = (ConfigFileInfo)@object;
			NotifyPropertyChanged(nameof(CanConfigGetSelected));
		}

		[UIAction("reload-list")]
		internal async void RefreshList()
		{
			await LoadInternal().ConfigureAwait(false);
		}

		[UIAction("pick-config")]
		internal void PickConfig()
		{
			if (CanConfigGetSelected)
			{
				_configProvider.SelectUserConfig(_selectedConfigFileInfo);
				NotifyPropertyChanged(nameof(HasConfigCurrently));
				NotifyPropertyChanged(nameof(LoadedConfigText));
			}
		}

		[UIAction("unpick-config")]
		internal void UnpickConfig()
		{
			if (HasConfigCurrently)
			{
				UnityMainThreadTaskScheduler.Factory.StartNew(() =>
				{
					if (customListTableData == null)
					{
						Plugin.LoggerInstance.Warn($"{nameof(customListTableData)} is null.");
						return;
					}

					customListTableData.tableView.ClearSelection();
				});

				_configProvider.UnselectUserConfig();
				NotifyPropertyChanged(nameof(HasConfigCurrently));
				NotifyPropertyChanged(nameof(LoadedConfigText));
			}
		}

		protected override async void DidActivate(bool firstActivation, ActivationType type)
		{
			base.DidActivate(firstActivation, type);

			await LoadInternal().ConfigureAwait(false);
		}

		protected override void DidDeactivate(DeactivationType deactivationType)
		{
			base.DidDeactivate(deactivationType);

			AvailableConfigs.Clear();

			_selectedConfigFileInfo = null;
		}

		private async Task LoadInternal()
		{
			if (customListTableData == null)
			{
				Plugin.LoggerInstance.Warn($"{nameof(customListTableData)} is null.");
				return;
			}

			if (AvailableConfigs.Count > 0)
			{
				AvailableConfigs.Clear();
			}

			LoadingConfigs = true;
			NotifyPropertyChanged(nameof(LoadingConfigs));
			NotifyPropertyChanged(nameof(HasLoadedConfigs));

			var intermediateConfigs = (await _configProvider.ListAvailableConfigs())
				.OrderByDescending(x => x.State)
				.ThenBy(x => x.ConfigName)
				.ToList();
			AvailableConfigs.AddRange(intermediateConfigs);

			var currentConfigIndex = intermediateConfigs.FindIndex(x => x.ConfigPath == _configProvider.CurrentConfigPath);

			await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
			{
				customListTableData.tableView.ReloadData();
				customListTableData.tableView.ScrollToCellWithIdx(0, TableViewScroller.ScrollPositionType.Beginning, false);
				if (currentConfigIndex >= 0)
				{
					customListTableData.tableView.SelectCellWithIdx(currentConfigIndex, true);
				}

				LoadingConfigs = false;
				NotifyPropertyChanged(nameof(LoadingConfigs));
				NotifyPropertyChanged(nameof(HasLoadedConfigs));
			}).ConfigureAwait(false);
		}
	}
}