using Colossal.Serialization.Entities;

using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.InGame;
using Game.Zones;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

using ZoneColorChanger.Domain;
using ZoneColorChanger.Utilities;

namespace ZoneColorChanger.Systems
{
	internal partial class ZoneColorChangerSystem : GameSystemBase
	{
		private FieldInfo zoneSystem_m_UpdateColors;
		private EntityQuery zonesQuery;
		private PrefabSystem prefabSystem;
		private PrefabUISystem prefabUISystem;
		private SnowDetectionSystem snowDetectionSystem;
		private ZoneSystem zoneSystem;
		private bool lastSnowStatus;
		private bool colorsChangedSinceLastIconUpdate;
		private readonly Dictionary<string, HslColor> _vanillaColors = new();
		private readonly Dictionary<string, List<ZonePrefab>> _zonePrefabs = new();

		protected override void OnCreate()
		{
			base.OnCreate();

			prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
			prefabUISystem = World.GetOrCreateSystemManaged<PrefabUISystem>();
			snowDetectionSystem = World.GetOrCreateSystemManaged<SnowDetectionSystem>();
			zoneSystem = World.GetOrCreateSystemManaged<ZoneSystem>();

			zoneSystem_m_UpdateColors = typeof(ZoneSystem).GetField("m_UpdateColors", BindingFlags.Instance | BindingFlags.NonPublic);
			zonesQuery = GetEntityQuery(ComponentType.ReadOnly<ZoneData>(), ComponentType.ReadOnly<PrefabData>(), ComponentType.Exclude<IndexedZone>());

			RequireForUpdate(zonesQuery);

			Enabled = false;
		}

		protected override void OnGamePreload(Purpose purpose, GameMode mode)
		{
			base.OnGamePreload(purpose, mode);

			Enabled = !mode.HasFlag(GameMode.MainMenu);
		}

		protected override void OnUpdate()
		{
			var zonesQuery = GetEntityQuery(ComponentType.ReadWrite<ZoneData>(), ComponentType.ReadOnly<PrefabData>());
			var entities = zonesQuery.ToEntityArray(Allocator.Temp);
			var prefabData = zonesQuery.ToComponentDataArray<PrefabData>(Allocator.Temp);

			for (var i = 0; i < prefabData.Length; i++)
			{
				var prefab = prefabSystem.GetPrefab<ZonePrefab>(prefabData[i]);
				var name = Mod.Settings.GroupThemes ? Regex.Replace(prefab.name, "^[A-Z]{2,3} ", string.Empty) : prefab.name;

				_vanillaColors[name] = prefab.m_Color;

				if (_zonePrefabs.ContainsKey(name))
				{
					_zonePrefabs[name].Add(prefab);
				}
				else
				{
					_zonePrefabs[name] = new() { prefab };
				}

				EntityManager.AddComponent<IndexedZone>(entities[i]);
			}

			UpdateColors();
			UpdateIcons();

			Enabled = false;
		}

		internal void UpdateColors()
		{
			lastSnowStatus = snowDetectionSystem.AverageSnowCoverage > 8f;

			var fillAlpha = ConfigUtil.Instance.GetCellFillAlpha();
			var edgeAlpha = ConfigUtil.Instance.GetCellEdgeAlpha();

			foreach (var key in _zonePrefabs.Keys)
			{
				var color = GetColor(key, true);

				foreach (var item in _zonePrefabs[key])
				{
					item.m_Edge = new Color(color.r, color.g, color.b, key is "Unzoned" ? ConfigUtil.Instance.GetUnzonedCellEdgeAlpha() : edgeAlpha);
					item.m_Color = new Color(color.r, color.g, color.b, key is "Unzoned" ? ConfigUtil.Instance.GetUnzonedCellFillAlpha() : fillAlpha);
				}
			}

			zoneSystem_m_UpdateColors.SetValue(zoneSystem, true);
			colorsChangedSinceLastIconUpdate = true;
		}

		internal void UpdateIcons()
		{
			if (!colorsChangedSinceLastIconUpdate || !Mod.Settings.RecolorIcons)
			{
				return;
			}

			colorsChangedSinceLastIconUpdate = false;

			var fillAlpha = ConfigUtil.Instance.GetCellFillAlpha();
			var edgeAlpha = ConfigUtil.Instance.GetCellEdgeAlpha();

			foreach (var key in _zonePrefabs.Keys)
			{
				var color = GetColor(key, true);

				foreach (var item in _zonePrefabs[key])
				{
					ThumbnailUtil.ProcessThumbnail(item, color, _vanillaColors[key].Hue);
				}
			}
		}

		internal ZoneGroupItem[] GetZoneDataList()
		{
			var array = new ZoneGroupItem[4];
			var index = 0;

			foreach (var grp in _zonePrefabs
				.Where(x => x.Value.Any(x => x.m_AreaType != AreaType.None))
				.GroupBy(x => x.Value[0].m_Office ? "Office" : x.Value[0].m_AreaType.ToString())
				.OrderBy(x => x.First().Value[0].m_AreaType)
				.ThenBy(x => x.First().Value[0].m_Office))
			{
				array[index++] = new ZoneGroupItem
				{
					GroupName = grp.Key,
					Zones = grp
						.OrderBy(x => x.Value[0].TryGet<UIObject>(out var uIObject) ? uIObject.m_Priority : int.MaxValue)
						.ThenBy(x => GetZoneName(x.Key))
						.Select(x => new ZoneInfoItem
						{
							PrefabId = x.Key,
							PrefabName = GetZoneName(x.Key),
							Color = GetColor(x.Key, false)
						})
						.ToArray()
				};
			}

			return array;
		}

		private Color GetColor(string key, bool withSnow)
		{
			var unZoned = key is "Unzoned";
			var mode = unZoned ? ColorMode.Default : ConfigUtil.Instance.GetColorMode();
			var color = mode switch
			{
				ColorMode.Default => _vanillaColors[key],
				ColorMode.Custom => ConfigUtil.Instance.TryGetCustomColor(key, out var customColor) ? customColor : _vanillaColors[key],
				_ => ColorblindUtil.ConvertColor(_vanillaColors[key], mode),
			};

			if (withSnow && snowDetectionSystem.AverageSnowCoverage > 8f)
			{
				color.Lum = unZoned ? 0f : Mathf.Clamp01(1.2f - color.Lum);
				color.Sat = Mathf.Clamp01(color.Sat * 1.3f);
			}

			return color;
		}

		private string GetZoneName(string zone)
		{
			if (GameManager.instance.localizationManager.activeDictionary.TryGetValue($"[{nameof(ZoneColorChanger)}].[Zones].[{zone}]", out var name))
			{
				return name;
			}

			prefabUISystem.GetTitleAndDescription(prefabSystem.GetEntity(_zonePrefabs[zone][0]), out var titleId, out var _);

			if (GameManager.instance.localizationManager.activeDictionary.TryGetValue(titleId, out var prefabName))
			{
				return Mod.Settings.GroupThemes 
					? Regex.Replace(prefabName, "^[A-Z]{2}[ _]", string.Empty)
					: prefabName;
			}

			return FormatWords(zone.Replace('_', ' '));
		}

		private string FormatWords(string str, bool forceUpper = false)
		{
			str = Regex.Replace(Regex.Replace(str,
				@"([a-z])([A-Z])", x => $"{x.Groups[1].Value} {x.Groups[2].Value}"),
				@"(\b)(?<!')([a-z])", x => $"{x.Groups[1].Value}{x.Groups[2].Value.ToUpper()}");

			if (forceUpper)
			{
				str = Regex.Replace(str, @"(^[a-z])|(\ [a-z])", x => x.Value.ToUpper(), RegexOptions.IgnoreCase);
			}

			return str;
		}
	}
}
