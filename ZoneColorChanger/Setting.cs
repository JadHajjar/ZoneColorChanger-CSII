using Colossal;
using Colossal.IO.AssetDatabase;

using Game.Modding;
using Game.Settings;

using System.Collections.Generic;

namespace ZoneColorChanger
{
	[FileLocation("ModsSettings\\" + nameof(ZoneColorChanger) + "\\" + nameof(ZoneColorChanger))]
	[SettingsUIGroupOrder(OptionsGroup)]
	[SettingsUIShowGroupName(OptionsGroup)]
	public class Setting : ModSetting
	{
		public const string MainSection = "MainSection";

		public const string OptionsGroup = "OptionsGroup";

		public Setting(IMod mod) : base(mod)
		{

		}

		[SettingsUISection(MainSection, OptionsGroup)]
		public bool RecolorIcons { get; set; } = true;

		[SettingsUISection(MainSection, OptionsGroup)]
		public bool GroupThemes { get; set; } = true;

		public override void SetDefaults()
		{
		}
	}

	public class LocaleEN : IDictionarySource
	{
		private readonly Setting m_Setting;
		public LocaleEN(Setting setting)
		{
			m_Setting = setting;
		}
		public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
		{
			return new Dictionary<string, string>
			{
				{ m_Setting.GetSettingsLocaleID(), "Zone Color Changer" },
				{ m_Setting.GetOptionTabLocaleID(Setting.MainSection), "Main" },

				{ m_Setting.GetOptionGroupLocaleID(Setting.OptionsGroup), "Settings" },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.RecolorIcons)), "Change zones' icon color" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.RecolorIcons)), $"When enabled, zones' icons will be updated to match the selected color. Changes take effect when re-opening the zones toolbar. Disabling this option requires a restart to take effect." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.GroupThemes)), "Group zone types' themes" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.GroupThemes)), $"When enabled, zones with multiple themes (EU, NA, etc.) are grouped into one option." },
			};
		}

		public void Unload()
		{

		}
	}
}
