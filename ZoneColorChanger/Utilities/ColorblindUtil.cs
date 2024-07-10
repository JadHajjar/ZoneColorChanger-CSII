using UnityEngine;

using ZoneColorChanger.Domain;

namespace ZoneColorChanger.Utilities
{
	internal static class ColorblindUtil
	{
		public static HslColor ConvertColor(HslColor color, ColorMode colorBlindness)
		{
			return new HslColor
			{
				Hue = colorBlindness switch
				{
					ColorMode.Deuteranopia => 0.66f + (Mathf.Cos(2.5f * color.Hue * Mathf.PI) / 3f) - (0.45f * color.Hue) - (0.33f * Mathf.Pow(color.Hue - 0.2f, 2)),
					ColorMode.Protanopia => 0.81f - (0.72f * color.Hue) + (0.18f * Mathf.Pow(color.Hue, 2)),
					ColorMode.Tritanopia => 1f - (4.188801f * color.Hue) + (4.408151f * Mathf.Pow(color.Hue, 2)),
					_ => color.Hue,
				},
				Sat = colorBlindness switch
				{
					ColorMode.Deuteranopia => (0.7f * color.Sat) + 0.5f - (2f * Mathf.Pow(color.Hue - 0.5f, 2)),
					ColorMode.Protanopia => 2.449778f - (4.449332f * color.Sat) + (2.673068f * Mathf.Pow(color.Sat, 2)),
					ColorMode.Tritanopia => 0.65f + (0.3f * color.Sat) - (2.5f * Mathf.Pow(color.Sat, 2)) + (2.68f * Mathf.Pow(color.Sat, 3)),
					_ => color.Sat,
				},
				Lum = colorBlindness switch
				{
					ColorMode.Deuteranopia or ColorMode.Protanopia => 0.588270408f + (44.7403636f * Mathf.Pow(color.Lum, 2)) - (67.1476f * Mathf.Pow(color.Lum, 3)) + (31.5790818f * Mathf.Pow(color.Lum, 4)) - (9.17121818f * color.Lum),
					ColorMode.Tritanopia => 0.633636f + (7.272727f * Mathf.Pow(color.Lum, 3)) - (9.090909f * Mathf.Pow(color.Lum, 2)) + (2.309091f * color.Lum),
					_ => color.Lum,
				},
			};
		}
	}
}
