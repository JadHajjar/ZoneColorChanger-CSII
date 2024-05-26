using Game;

using Unity.Entities;

namespace ZoneColorChanger.Systems
{
	internal partial class ZoneSnowColorChangerSystem : GameSystemBase
	{
		private const int UPDATE_INTERVAL = 128;

		private SnowDetectionSystem snowDetectionSystem;
		private ZoneColorChangerSystem zoneColorChangerSystem;
		private bool lastSnowStatus;

		protected override void OnCreate()
		{
			base.OnCreate();

			snowDetectionSystem = World.GetOrCreateSystemManaged<SnowDetectionSystem>();
			zoneColorChangerSystem = World.GetOrCreateSystemManaged<ZoneColorChangerSystem>();
		}

		public override int GetUpdateInterval(SystemUpdatePhase phase)
		{
			return UPDATE_INTERVAL;
		}

		protected override void OnUpdate()
		{
			if (lastSnowStatus != (snowDetectionSystem.AverageSnowCoverage > 8f))
			{
				lastSnowStatus = snowDetectionSystem.AverageSnowCoverage > 8f;

				zoneColorChangerSystem.UpdateColors();
			}
		}
	}
}
