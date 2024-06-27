using Colossal.Collections;

using Game.Tools;
using Game.UI.InGame;

using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using ZoneColorChanger.Domain;
using ZoneColorChanger.Utilities;

namespace ZoneColorChanger.Systems
{
	internal partial class ZoneChangerUISystem : ExtendedUISystemBase
	{
		private ToolSystem toolSystem;
		private CancellationTokenSource cancellationTokenSource;
		private ZoneColorChangerSystem zoneChangerSystem;
		private ValueBindingHelper<bool> _ShowMainPanel;
		private ValueBindingHelper<bool> _ZoneToolActive;
		private ValueBindingHelper<ZoneGroupItem[]> _ZoneInfoList;
		private ValueBindingHelper<ColorMode> _ColorMode;

		protected override void OnCreate()
		{
			base.OnCreate();

			zoneChangerSystem = World.GetOrCreateSystemManaged<ZoneColorChangerSystem>();
			toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();

			toolSystem.EventToolChanged += OnToolChanged;

			_ShowMainPanel = CreateBinding("MainPanelVisible", "SetMainPanelVisible", false);
			_ZoneToolActive = CreateBinding("ZoneToolActive", false);
			_ZoneInfoList = CreateBinding("ZoneInfoList", new ZoneGroupItem[0]);
			_ColorMode = CreateBinding("ColorMode", "SetColorMode", ConfigUtil.Instance.GetColorMode(), ConfigUtil.Instance.SetColorMode);
			CreateBinding("CellFillAlpha", "SetCellFillAlpha", ConfigUtil.Instance.GetCellFillAlpha(), ConfigUtil.Instance.SetCellFillAlpha);
			CreateBinding("CellEdgeAlpha", "SetCellEdgeAlpha", ConfigUtil.Instance.GetCellEdgeAlpha(), ConfigUtil.Instance.SetCellEdgeAlpha);
			CreateBinding("CellUnzonedFillAlpha", "SetUnzonedCellFillAlpha", ConfigUtil.Instance.GetUnzonedCellFillAlpha(), ConfigUtil.Instance.SetUnzonedCellFillAlpha);
			CreateBinding("CellUnzonedEdgeAlpha", "SetUnzonedCellEdgeAlpha", ConfigUtil.Instance.GetUnzonedCellEdgeAlpha(), ConfigUtil.Instance.SetUnzonedCellEdgeAlpha);

			CreateTrigger<string, Color>("SetZoneColor", (s, c) => ConfigUtil.Instance.SetCustomColor(s, c));
			CreateTrigger<string>("ResetZoneColor", ConfigUtil.Instance.ResetCustomColor);
		}

		protected override void OnUpdate()
		{
			if (_ShowMainPanel)
			{
				_ZoneInfoList.Value = zoneChangerSystem.GetZoneDataList();
			}

			if (ConfigUtil.Instance.IsDirty)
			{
				ConfigUtil.Instance.IsDirty = false;

				_ColorMode.Value = ConfigUtil.Instance.GetColorMode();

				zoneChangerSystem.UpdateColors();

				Task.Run(UpdateIcons);
			}

			base.OnUpdate();
		}

		private void OnToolChanged(ToolBaseSystem system)
		{
			_ZoneToolActive.Value = system is ZoneToolSystem;
		}

		private async void UpdateIcons()
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new();

			var token = cancellationTokenSource.Token;

			await Task.Delay(1000);

			if (token.IsCancellationRequested)
			{ 
				return; 
			}

			zoneChangerSystem.UpdateIcons();
		}
	}
}
