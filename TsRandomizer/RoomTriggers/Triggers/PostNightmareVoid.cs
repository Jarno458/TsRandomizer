using Archipelago.MultiClient.Net.Enums;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16, 27)]
	class PostNightmareVoid : RoomTrigger
	{

		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSettings().BossRando.Value)
				RoomTriggerHelper.CreateAndCallCutScene(roomState, "Temple2_End");

			roomState.Level.JukeBox.StopSong();

			if (roomState.SeedOptions.Archipelago)
				HandleArchipelagoEndOfGame(roomState.ScreenManager);
		}

		static void HandleArchipelagoEndOfGame(ScreenManager screenManager)
		{
			Client.SetStatus(ArchipelagoClientState.ClientGoal);

			AskPermissionMessage(screenManager, "collect", Client.CollectPermissions);
			AskPermissionMessage(screenManager, "release", Client.ForfeitPermissions);
		}

		static void AskPermissionMessage(ScreenManager screenManager, string command, Permissions permissionFlags)
		{
			if (!permissionFlags.HasFlag(Permissions.Auto) &&
			    (permissionFlags.HasFlag(Permissions.Enabled) || permissionFlags.HasFlag(Permissions.Goal)))
			{
				var messageBox = MessageBox.Create(screenManager, $"Press OK to {command} remaining item checks", _ => {
					Client.Say($"!{command}");
				});

				screenManager.AddScreen(messageBox.Screen, null);
			}
		}
	}
}
