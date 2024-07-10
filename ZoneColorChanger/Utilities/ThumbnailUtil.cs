using Colossal.PSI.Environment;
using Colossal.UI;

using Game.Prefabs;
using Game.UI;
using Game.Zones;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Unity.Mathematics;

using UnityEngine;

using ZoneColorChanger.Domain;

namespace ZoneColorChanger.Utilities
{
	internal class ThumbnailUtil
	{
		private static readonly Dictionary<string, string> _cachedThumbnails = new();
		private static readonly Dictionary<string, string> _cachedTempPath = new();
		internal static string GameUIPath { get; }
		internal static string CustomAILPath { get; }
		internal static string TempFolder { get; }
		internal static string AILPath { get; set; }

		static ThumbnailUtil()
		{
			GameUIPath = Path.Combine(EnvPath.kStreamingDataPath, "~UI~", "GameUI");
			CustomAILPath = Path.Combine(EnvPath.kUserDataPath, "ModsData", "AssetIconLibrary", "CustomThumbnails");
			TempFolder = Path.Combine(EnvPath.kTempDataPath, nameof(ZoneColorChanger));

			if (Directory.Exists(TempFolder))
			{
				new DirectoryInfo(TempFolder).Delete(true);
			}

			Directory.CreateDirectory(TempFolder);
		}

		internal static void ProcessThumbnail(ZonePrefab prefab, HslColor newColor, float defaultZoneColorHue)
		{
			var text = GetFileText(prefab);

			if (text == null)
			{
				return;
			}

			var defaultHue = GetDefaultHue(prefab);
			var tempFileName = Path.Combine(TempFolder, Guid.NewGuid() + ".svg");
			var newFileText = Regex.Replace(text, @"\#\w{6}", match =>
			{
				var color = ColorUtility.TryParseHtmlString(match.Value, out var hexColor) ? (HslColor)hexColor : (HslColor)Color.white;

				if (math.abs(color.Hue - defaultHue) < 0.075f || (!prefab.builtin && math.abs(color.Hue - defaultZoneColorHue) < 0.075f))
				{
					color.Hue = newColor.Hue;
					color.Lum = Mathf.Clamp01(color.Lum + ((newColor.Lum - color.Lum) / 6));
					color.Sat = Mathf.Clamp01(color.Sat + ((newColor.Sat - color.Sat) / 6));
				}

				return $"#{ColorUtility.ToHtmlStringRGB(color)}";
			});

			File.WriteAllText(tempFileName, newFileText);

			if (prefab.TryGet<UIObject>(out var uIObject))
			{
				uIObject.m_Icon = $"coui://zcc/{Path.GetFileName(tempFileName)}";
			}
		}

		private static float GetDefaultHue(ZonePrefab prefab)
		{
			return prefab.m_AreaType switch
			{
				AreaType.Residential => 0.34f,
				AreaType.Commercial => 0.54f,
				AreaType.Industrial => prefab.m_Office ? 0.78f : 0.13f,
				_ => 0f,
			};
		}

		private static string GetFileText(PrefabBase prefab)
		{
			if (_cachedThumbnails.TryGetValue(prefab.name, out var cache))
			{
				return cache;
			}

			try
			{
				var file = GetFileName(prefab);

				if (string.IsNullOrEmpty(file) || !file.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase))
				{
					return null;
				}

				return _cachedThumbnails[prefab.name] = File.ReadAllText(file);
			}
			catch (Exception ex)
			{
				Mod.Log.Error(ex);

				return null;
			}
		}

		private static string GetFileName(PrefabBase prefab)
		{
			var icon = ImageSystem.GetThumbnail(prefab);

			if (string.IsNullOrEmpty(icon) || icon.StartsWith("thumbnail://"))
			{
				return null;
			}

			var regex = Regex.Match(icon, "coui://(.+?)/(.+)");

			if (!regex.Success)
			{
				return Path.Combine(GameUIPath, icon);
			}

			var hostMap = (IDictionary<string, HashSet<string>>)typeof(DefaultResourceHandler).GetField("m_HostLocationsMap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(UIManager.defaultUISystem.resourceHandler);

			if (hostMap?.TryGetValue(regex.Groups[1].Value, out var paths) ?? false)
			{
				foreach (var path in paths)
				{
					if (File.Exists(Path.Combine(path, regex.Groups[2].Value)))
					{
						return Path.Combine(path, regex.Groups[2].Value);
					}
				}
			}

			return null;
		}
	}
}
