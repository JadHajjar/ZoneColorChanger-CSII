using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;

using Game;
using Game.Modding;
using Game.SceneFlow;

using System.IO;
using System.Linq;

using ZoneColorChanger.Systems;
using ZoneColorChanger.Utilities;

namespace ZoneColorChanger
{
	public class Mod : IMod
	{
		public const string Id = nameof(ZoneColorChanger);
		public static ILog Log = LogManager.GetLogger(nameof(ZoneColorChanger)).SetShowsErrorsInUI(false).SetLogStackTrace(false);
		public static Setting Settings { get; private set; }

		private static bool? isAssetIconLibraryEnabled;
		public static bool IsAssetIconLibraryEnabled => isAssetIconLibraryEnabled ??= GameManager.instance.modManager.ListModsEnabled().Any(x => x.StartsWith("AssetIconLibrary"));

		public void OnLoad(UpdateSystem updateSystem)
		{
			Log.Info(nameof(OnLoad));

			Settings = new Setting(this);
			Settings.RegisterInOptionsUI();
			GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));

			AssetDatabase.global.LoadSettings(nameof(ZoneColorChanger), Settings, new Setting(this));

			updateSystem.UpdateAt<SnowDetectionSystem>(SystemUpdatePhase.GameSimulation);
			updateSystem.UpdateAt<ZoneSnowColorChangerSystem>(SystemUpdatePhase.GameSimulation);
			updateSystem.UpdateAt<ZoneColorChangerSystem>(SystemUpdatePhase.Modification1);
			updateSystem.UpdateAt<ZoneChangerUISystem>(SystemUpdatePhase.UIUpdate);

			GameManager.instance.RegisterUpdater(PrepareThumbnails);
		}

		private void PrepareThumbnails()
		{
			foreach (var item in GameManager.instance.modManager)
			{
				if (item.isLoaded && item.assemblyFullName.StartsWith("AssetIconLibrary"))
				{
					ThumbnailUtil.AILPath = Path.Combine(Path.GetDirectoryName(item.asset.path), "Thumbnails");
				}
			}

			UIManager.defaultUISystem.AddHostLocation("zcc", ThumbnailUtil.TempFolder, true);
		}

		public void OnDispose()
		{
			Log.Info(nameof(OnDispose));

			UIManager.defaultUISystem.RemoveHostLocation("zcc");

			new DirectoryInfo(ThumbnailUtil.TempFolder).Delete(true);

			if (Settings != null)
			{
				Settings.UnregisterInOptionsUI();
				Settings = null;
			}
		}
	}
}
