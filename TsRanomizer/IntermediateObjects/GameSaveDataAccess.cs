using Timespinner.GameAbstractions.Saving;

namespace TsRanodmizer.IntermediateObjects
{
	interface IGameSaveDataAccess
	{
		bool HasKey(string key);
		void SetKey(string key);
	}

	class GameSaveDataAccess : IGameSaveDataAccess
	{
		readonly GameSave gameSave;

		public GameSaveDataAccess(GameSave gameSave)
		{
			this.gameSave = gameSave;
		}

		public bool HasKey(string key)
		{
			return gameSave.DataKeyBools.ContainsKey(key);
		}

		public void SetKey(string key)
		{
			gameSave.DataKeyBools[key] = true;
		}
	}
}
