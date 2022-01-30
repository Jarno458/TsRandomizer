using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class OnOffGameSetting : GameSetting<bool>
	{
		[JsonConstructor]
		public OnOffGameSetting(string name, string description, bool defaultValue, bool canBeToggledInGame = false) 
			: base(name, description, defaultValue, canBeToggledInGame)
		{

		}
	}
}
