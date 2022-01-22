using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class OnOffGameSetting : GameSetting<bool>
	{
		[JsonConstructor]
		public OnOffGameSetting(string category, string name, string description, bool defaultValue = false, bool canBeToggledInGame = false) 
			: base(category, name, description, defaultValue, canBeToggledInGame)
		{

		}
	}
}
