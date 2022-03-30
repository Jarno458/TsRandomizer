using Newtonsoft.Json;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class OnOffGameSetting : GameSetting<bool>
	{
		[JsonConstructor]
		public OnOffGameSetting(string name, string description, bool defaultValue, bool canBeToggledInGame = false) 
			: base(name, description, defaultValue, canBeToggledInGame)
		{

		}

		public override void ToggleValue() => Value = !Value;

		internal override void UpdateMenuEntry(MenuEntry menuEntry)
		{
			base.UpdateMenuEntry(menuEntry);

			menuEntry.Text = $"{Name} - {(Value ? "On" : "Off")}";
		} 
	}
}
