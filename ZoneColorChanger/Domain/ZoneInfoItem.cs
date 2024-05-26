namespace ZoneColorChanger.Domain
{
	public class ZoneGroupItem
	{
		public string GroupName;
		public ZoneInfoItem[] Zones;
	}

	public class ZoneInfoItem
	{
		public string PrefabId;
		public string PrefabName;
		public UnityEngine.Color Color;
	}
}
