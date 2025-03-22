using UnityEngine;

namespace ZoneColorChanger.Domain
{
	public struct HslColor
	{
		public float Hue { get; set; }
		public float Sat { get; set; }
		public float Lum { get; set; }

		public static implicit operator Color(HslColor color)
		{
			return Color.HSVToRGB(color.Hue, color.Sat, color.Lum);
		}

		public static implicit operator System.Drawing.Color(HslColor hslcolor)
		{
			var color = Color.HSVToRGB(hslcolor.Hue, hslcolor.Sat, hslcolor.Lum);

			return System.Drawing.Color.FromArgb((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
		}

		public static implicit operator HslColor(Color color)
		{
			Color.RGBToHSV(color, out var hue, out var sat, out var lum);

			return new HslColor
			{
				Hue = hue,
				Sat = sat,
				Lum = lum
			};
		}

		public static implicit operator HslColor(System.Drawing.Color color)
		{
			Color.RGBToHSV(new Color(color.R / 255f, color.G / 255f, color.B / 255f), out var hue, out var sat, out var lum);

			return new HslColor
			{
				Hue = hue,
				Sat = sat,
				Lum = lum
			};
		}
	}
}
