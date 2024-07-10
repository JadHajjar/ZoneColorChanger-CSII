using Colossal.Json;
using Colossal.PSI.Environment;

using System;
using System.IO;

using ZoneColorChanger.Domain;

namespace ZoneColorChanger.Utilities
{
	public class ConfigUtil
	{
		private readonly Config _config;
		private readonly string _configFilePath;

		public static ConfigUtil Instance { get; } = new();
		public string SettingsFolder { get; }
		public bool IsDirty { get; set; }

		private ConfigUtil()
		{
			SettingsFolder = Path.Combine(EnvPath.kUserDataPath, "ModsSettings", nameof(ZoneColorChanger));

			_configFilePath = Path.Combine(SettingsFolder, "Config.json");

			try
			{
				if (File.Exists(_configFilePath))
				{
					_config = JSON.MakeInto<Config>(JSON.Load(File.ReadAllText(_configFilePath)));
				}
			}
			catch (Exception ex)
			{
				Mod.Log.Error(ex, "Failed to load config file");
			}
			finally
			{
				_config ??= new();
			}
		}

		public void SetColorMode(ColorMode colorMode)
		{
			_config.Mode = colorMode;

			Save();
		}

		public void SetCustomColor(string key, HslColor color)
		{
			if (_config.Mode != ColorMode.Custom)
			{
				_config.ZoneColors = new();
				_config.Mode = ColorMode.Custom;
			}

			_config.ZoneColors[key] = color;

			Save();
		}

		public void ResetAllCustomColors()
		{
			_config.ZoneColors.Clear();

			Save();
		}

		public void ResetCustomColor(string key)
		{
			_config.ZoneColors.Remove(key);

			Save();
		}

		public ColorMode GetColorMode()
		{
			return _config.Mode;
		}

		public bool TryGetCustomColor(string key, out HslColor color)
		{
			return _config.ZoneColors.TryGetValue(key, out color);
		}

		public float GetCellFillAlpha()
		{
			return _config.FillAlpha;
		}

		public float GetCellEdgeAlpha()
		{
			return _config.EdgeAlpha;
		}

		public float GetUnzonedCellFillAlpha()
		{
			return _config.UnzonedFillAlpha;
		}

		public float GetUnzonedCellEdgeAlpha()
		{
			return _config.UnzonedEdgeAlpha;
		}

		public void SetCellFillAlpha(float fillAlpha)
		{
			_config.FillAlpha = fillAlpha;

			Save();
		}

		public void SetCellEdgeAlpha(float edgeAlpha)
		{
			_config.EdgeAlpha = edgeAlpha;

			Save();
		}

		public void SetUnzonedCellFillAlpha(float fillAlpha)
		{
			_config.UnzonedFillAlpha = fillAlpha;

			Save();
		}

		public void SetUnzonedCellEdgeAlpha(float edgeAlpha)
		{
			_config.UnzonedEdgeAlpha = edgeAlpha;

			Save();
		}

		private void Save()
		{
			IsDirty = true;

			Directory.CreateDirectory(SettingsFolder);
			File.WriteAllText(_configFilePath, JSON.Dump(_config));
		}
	}
}
