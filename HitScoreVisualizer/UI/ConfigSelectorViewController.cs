using System;
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

		[UIValue("is-valid-config-selected")]
		internal bool CanConfigGetSelected => _selectedConfigFileInfo?.ConfigPath != _configProvider.CurrentConfigPath && _configProvider.ConfigSelectable(_selectedConfigFileInfo?.State);

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
			}
		}

		protected override async void DidActivate(bool firstActivation, ActivationType type)
		{
			base.DidActivate(firstActivation, type);

			Plugin.LoggerInstance.Info("DidActivate");

			try
			{
				await LoadInternal().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Plugin.LoggerInstance.Error(ex);
			}
		}

		protected override void DidDeactivate(DeactivationType deactivationType)
		{
			base.DidDeactivate(deactivationType);

			Plugin.LoggerInstance.Info("DidDeactivate");

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
				// await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.ReloadData()).ConfigureAwait(false);
			}

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
			}).ConfigureAwait(false);
		}
	}
}