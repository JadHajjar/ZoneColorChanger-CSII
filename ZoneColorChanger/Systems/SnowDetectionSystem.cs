using Colossal.Collections;

using Game;
using Game.Common;
using Game.Objects;
using Game.Tools;
using Game.Vehicles;

using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ZoneColorChanger.Systems
{
	public partial class SnowDetectionSystem : GameSystemBase
	{
		private const int UPDATE_INTERVAL = 128;

		private NativeArray<float> averageSnowAccumulation;

		public bool IsSnowy => averageSnowAccumulation[0] > 15f;
		public float AverageSnowCoverage => averageSnowAccumulation[0];

		protected override void OnCreate()
		{
			base.OnCreate();

			averageSnowAccumulation = new NativeArray<float>(1, Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			averageSnowAccumulation.Dispose();
		}

		public override int GetUpdateInterval(SystemUpdatePhase phase)
		{
			return UPDATE_INTERVAL;
		}

		protected override void OnUpdate()
		{
			var snowAccumulation = new NativeAccumulator<AverageFloat>(Allocator.TempJob);
			var surfaceQuery = SystemAPI.QueryBuilder().WithAll<Surface>().WithNone<Temp, Deleted, Car, Moving>().Build();

			Dependency = new SnowDetectionJob
			{
				SurfaceTypeHandle = SystemAPI.GetComponentTypeHandle<Surface>(true),
				SnowAccumulation = snowAccumulation.AsParallelWriter()
			}.ScheduleParallel(surfaceQuery, Dependency);

			Dependency = new FinalCountJob
			{
				AverageSnowAccumulation = averageSnowAccumulation,
				SnowAccumulation = snowAccumulation
			}.Schedule(Dependency);

			snowAccumulation.Dispose(Dependency);
		}

		[BurstCompile]
		private struct SnowDetectionJob : IJobChunk
		{
			[ReadOnly]
			internal ComponentTypeHandle<Surface> SurfaceTypeHandle;
			internal NativeAccumulator<AverageFloat>.ParallelWriter SnowAccumulation;

			public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
			{
				var surfaces = chunk.GetNativeArray(ref SurfaceTypeHandle);
				var sum = 0;

				for (var i = 0; i < chunk.Count; i++)
				{
					sum += surfaces[i].m_AccumulatedSnow;
				}

				SnowAccumulation.Accumulate(new AverageFloat
				{
					m_Count = chunk.Count,
					m_Total = sum
				});
			}
		}

		[BurstCompile]
		private struct FinalCountJob : IJob
		{
			internal NativeAccumulator<AverageFloat> SnowAccumulation;
			internal NativeArray<float> AverageSnowAccumulation;

			public void Execute()
			{
				AverageSnowAccumulation[0] = SnowAccumulation.GetResult().average;
			}
		}
	}
}
