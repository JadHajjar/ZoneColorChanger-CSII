using System.Collections.Generic;

using UnityEngine;

using ZoneColorChanger.Domain;

namespace ZoneColorChanger.Utilities
{
	internal static class ColorblindPresets
	{
		internal static Dictionary<string, HslColor> GetColorTable(ColorMode colorBlindness)
		{
			return colorBlindness switch
			{
				ColorMode.Deuteranopia => _deuteranopia,
				ColorMode.Protanopia => _protanopia,
				ColorMode.Tritanopia => _tritanopia,
				_ => new(),
			};
		}

		private static readonly Dictionary<string, HslColor> _deuteranopia = new()
		{
			["Residential Low"] = new Color(56, 49, 237) / 255,
			["Residential Low Waterfront"] = new Color(49, 87, 237) / 255,
			["Residential LowRent"] = new Color(22, 19, 104) / 255,
			["Residential Medium Row"] = new Color(33, 29, 147) / 255,
			["Residential Medium"] = new Color(45, 40, 191) / 255,
			["Residential Mixed"] = new Color(93, 88, 221) / 255,
			["Residential High"] = new Color(4, 2, 48) / 255,
			["Industrial Manufacturing"] = new Color(149, 7, 178) / 255,
			["Industrial Forestry"] = new Color(117, 5, 140) / 255,
			["Industrial Agriculture"] = new Color(132, 6, 158) / 255,
			["Industrial Oil"] = new Color(30, 3, 35) / 255,
			["Industrial Ore"] = new Color(104, 4, 124) / 255,
		};

		private static readonly Dictionary<string, HslColor> _protanopia = new()
		{
			["Residential Low"] = new Color(56, 49, 237) / 255,
			["Residential Low Waterfront"] = new Color(49, 87, 237) / 255,
			["Residential LowRent"] = new Color(22, 19, 104) / 255,
			["Residential Medium Row"] = new Color(33, 29, 147) / 255,
			["Residential Medium"] = new Color(45, 40, 191) / 255,
			["Residential Mixed"] = new Color(93, 88, 221) / 255,
			["Residential High"] = new Color(4, 2, 48) / 255,
			["Industrial Manufacturing"] = new Color(149, 7, 178) / 255,
			["Industrial Forestry"] = new Color(117, 5, 140) / 255,
			["Industrial Agriculture"] = new Color(132, 6, 158) / 255,
			["Industrial Oil"] = new Color(30, 3, 35) / 255,
			["Industrial Ore"] = new Color(104, 4, 124) / 255,
		};

		private static readonly Dictionary<string, HslColor> _tritanopia = new()
		{
			["Commercial Low"] = new Color(255, 40, 40) / 255,
			["Commercial High"] = new Color(158, 9, 9) / 255,
			["Office Low"] = new Color(206, 120, 8) / 255,
			["Office High"] = new Color(79, 46, 3) / 255,
			["Industrial Manufacturing"] = new Color(149, 7, 178) / 255,
			["Industrial Forestry"] = new Color(117, 5, 140) / 255,
			["Industrial Agriculture"] = new Color(132, 6, 158) / 255,
			["Industrial Oil"] = new Color(30, 3, 35) / 255,
			["Industrial Ore"] = new Color(104, 4, 124) / 255,
		};
	}
}
