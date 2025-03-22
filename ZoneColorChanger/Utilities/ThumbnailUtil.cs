using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Colossal.UI;

using Game.Prefabs;
using Game.UI;
using Game.Zones;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		internal static string[] GenericFolders { get; }
		internal static string CustomAILPath { get; }
		internal static string TempFolder { get; }
		internal static string AILPath { get; set; }

		static ThumbnailUtil()
		{
			GenericFolders = Directory.GetDirectories(EnvPath.kContentPath);
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
			try
			{
				ProcessSvgThumbnail(prefab, newColor, defaultZoneColorHue);
			}
			catch (Exception ex)
			{
				Mod.Log.Error(ex, $"Failed to process thumbnail for {prefab.name}");
			}
		}

		private static void ProcessSvgThumbnail(ZonePrefab prefab, HslColor newColor, float defaultZoneColorHue)
		{
			var text = GetFileText(prefab);

			if (text == null)
			{
				ProcessPngThumbnail(prefab, newColor, defaultZoneColorHue);

				return;
			}

			var defaultHue = GetDefaultHue(prefab);
			var tempFileName = Path.Combine(TempFolder, Guid.NewGuid() + ".svg");
			var newFileText = Regex.Replace(text, @"\#\w{6}", match =>
			{
				var color = ColorUtility.TryParseHtmlString(match.Value, out var hexColor) ? (HslColor)hexColor : (HslColor)Color.white;

				if (math.abs(color.Hue - defaultHue) < 0.075f || (!prefab.builtin && math.abs(color.Hue - defaultZoneColorHue) < 0.125f))
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

				Mod.Log.DebugFormat("Icon set for '{0}' to '{1}'", prefab.name, uIObject.m_Icon);
			}
		}

		private static void ProcessPngThumbnail(ZonePrefab prefab, HslColor newColor, float defaultZoneColorHue)
		{
			var file = GetFileName(prefab);

			if (string.IsNullOrEmpty(file) || !file.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
			{
				if (!IsKnownNoIcon(prefab))
				{
					Mod.Log.InfoFormat("Icon could not be found for '{0}'", prefab.name);
				}

				return;
			}

			var defaultHue = GetDefaultHue(prefab);
			var tempFileName = Path.Combine(TempFolder, Guid.NewGuid() + ".png");
			using var bitmap = new System.Drawing.Bitmap(file);

			var width = bitmap.Width;
			var height = bitmap.Height;

			for (var i = 0; i < height; i++)
			{
				for (var j = 0; j < width; j++)
				{
					var pixel = bitmap.GetPixel(j, i);
					var color = (HslColor)pixel;

					if (math.abs(color.Hue - defaultHue) < 0.075f || (!prefab.builtin && math.abs(color.Hue - defaultZoneColorHue) < 0.125f))
					{
						color.Hue = newColor.Hue;
						color.Lum = Mathf.Clamp01(color.Lum + ((newColor.Lum - color.Lum) / 6));
						color.Sat = Mathf.Clamp01(color.Sat + ((newColor.Sat - color.Sat) / 6));
					}

					bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(pixel.A, color));
				}
			}

			bitmap.Save(tempFileName);

			if (prefab.TryGet<UIObject>(out var uIObject))
			{
				uIObject.m_Icon = $"coui://zcc/{Path.GetFileName(tempFileName)}";

				Mod.Log.DebugFormat("Icon set for '{0}' to '{1}'", prefab.name, uIObject.m_Icon);
			}
		}

		private static bool IsKnownNoIcon(ZonePrefab prefab)
		{
			return prefab.name is
				"Industrial Agriculture" or
				"Industrial Oil" or
				"Industrial Ore" or
				"Industrial Forestry" or
				"Commercial Low" or
				"Residential Low" or
				"Residential High" or
				"Commercial High" or
				"Residential Mixed" or
				"Unzoned";
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

			var file = GetFileName(prefab);

			if (string.IsNullOrEmpty(file) || !file.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			return _cachedThumbnails[prefab.name] = File.ReadAllText(file);
		}

		private static string GetFileName(PrefabBase prefab)
		{
			var icon = ImageSystem.GetThumbnail(prefab);

			if (string.IsNullOrEmpty(icon) || icon.StartsWith("thumbnail://"))
			{
				return null;
			}

			var regex = Regex.Match(icon, "coui://(.+?)/(.+)");

			if (regex.Success)
			{
				var hostMap = (IDictionary<string, List<(string path, int)>>)typeof(DefaultResourceHandler).GetField("m_HostLocationsMap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(UIManager.defaultUISystem.resourceHandler);

				if (hostMap?.TryGetValue(regex.Groups[1].Value, out var paths) ?? false)
				{
					foreach (var item in paths.OrderBy(x => x.Item2))
					{
						if (File.Exists(Path.Combine(item.path, regex.Groups[2].Value)))
						{
							return Path.Combine(item.path, regex.Groups[2].Value);
						}
					}
				}
			}

			for (var i = 0; i < GenericFolders.Length; i++)
			{
				var fileName = Path.Combine(GenericFolders[i], "UI", icon);

				if (File.Exists(fileName))
				{
					return fileName;
				}
			}

			if (AssetDatabase.global.TryGetAsset<ImageAsset>(icon, out var asset)
				|| AssetDatabase.global.TryGetAsset("assetdb://gameui/" + icon, out asset))
			{
				var fileName = Path.Combine(TempFolder, $"{asset.identifier}.png");

				if (File.Exists(fileName))
				{
					return fileName;
				}

				var stream = asset.GetReadStream();
				using var fileStream = File.Create(fileName);

				stream.CopyTo(fileStream);
				fileStream.Close();

				return fileName;
			}

			return null;
		}
	}
}
