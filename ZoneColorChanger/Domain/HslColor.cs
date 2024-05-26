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
	}
}
