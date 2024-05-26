using System.Collections.Generic;

namespace ZoneColorChanger.Domain
{
	public class Config
	{
		public ColorMode Mode { get; set; }
		public float EdgeAlpha { get; set; } = 0f;
		public float FillAlpha { get; set; } = 0.4f;
		public float UnzonedEdgeAlpha { get; set; } = 0.0f;
		public float UnzonedFillAlpha { get; set; } = 0.15f;
		public Dictionary<string, HslColor> ZoneColors { get; set; } = new();
	}
}
