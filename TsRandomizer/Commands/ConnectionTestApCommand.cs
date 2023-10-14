using System;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using Microsoft.Xna.Framework;

using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class ConnectionTestApCommand : ConsoleCommand
	{
		public override string Command => "test-connect-ap";
		public override string ParameterUsage => "<server> <username> <password?>";

		GameConsole console;

		public override bool Handle(GameConsole gameConsole, string[] parameters)
		{
			console = gameConsole;

			string user;
			string password;
			string server;

			if (parameters.Length == 2)
			{
				server = parameters[0];
				user = parameters[1];
				password = null;
			}
			else if (parameters.Length == 3)
			{
				server = parameters[0];
				user = parameters[1];
				password = parameters[2];
			}
			else
			{
				return false;
			}

			var task = Task.Factory.StartNew(() => AttemptToConnect(server, user, password));
			task.Wait(TimeSpan.FromSeconds(15));
			
			return true;
		}

		void AttemptToConnect(string server, string user, string password)
		{
			console.AddLine($"Connecting to '{server}' using AP client:", Color.Yellow);
			var session = ArchipelagoSessionFactory.CreateSession(server);
			console.AddLine($"Calculated URI to {session.Socket.Uri}", Color.White);
			session.Socket.PacketReceived += OnPacketReceived;
			session.Socket.PacketsSent += OnPacketsSend;
			session.Socket.ErrorReceived += Socket_ErrorReceived;
			session.Socket.SocketOpened += Socket_SocketOpened;
			session.Socket.SocketClosed += Socket_SocketClosed;

			RoomInfoPacket roomInfo = null;
			try
			{
				console.AddLine("Attempting connection to server, with timeout of 5 seconds", Color.White);
				var task = session.ConnectAsync();
				if (task.Wait(TimeSpan.FromSeconds(5)))
					console.AddLine("connection task Server responded within 5 seconds", Color.White);
				else
					console.AddLine("connection task timed out", Color.Orange);
				console.AddLine($"connection task status {task.Status}", Color.White);

				if (task.IsCompleted)
				{
					roomInfo = task.Result;
					console.AddLine($"connection task result {task.Result.ToJObject()}", Color.White);
				}
			}
			catch (AggregateException ae)
			{
				console.AddLine($"connection task failed with AggregateException[{ae.InnerExceptions.Count}]:", Color.Red);

				foreach (var e in ae.InnerExceptions)
				{
					console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

				}
			}
			catch (Exception e)
			{
				console.AddLine($"connection task Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

			if (roomInfo == null)
				console.AddLine("Connection failed, aborting login", Color.Orange);
			else
				console.AddLine("Attemting login to server, with timeout of 5 seconds", Color.White);

			try
			{
				var task = session.LoginAsync("Timespinner", user, itemsHandlingFlags: ItemsHandlingFlags.NoItems, password: password, requestSlotData: false);
				if (task.Wait(TimeSpan.FromSeconds(5)))
					console.AddLine("login task Server responded within 5 seconds", Color.White);
				else
					console.AddLine("login task timed out", Color.Orange);
				console.AddLine($"login task status {task.Status}", Color.White);

				if (task.IsCompleted)
				{
					console.AddLine($"login result: {task.Result.Successful}", Color.White);

					if (task.Result is LoginFailure failure)
					{
						console.AddLine($"login failed error codes: {string.Join(", ", failure.ErrorCodes.Select(c => c.ToString()))}", Color.Red);
						console.AddLine($"login failed errors: {string.Join(", ", failure.ErrorCodes)}", Color.Red);
					}
					if (task.Result is LoginSuccessful successful)
						console.AddLine($"login successful for team: {successful.Team}, slot: {successful.Slot}", Color.Green);
				}
			}
			catch (AggregateException ae)
			{
				console.AddLine($"login task with AggregateException[{ae.InnerExceptions.Count}]:", Color.Red);

				foreach (var e in ae.InnerExceptions)
				{
					console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
				}
			}
			catch (Exception e)
			{
				console.AddLine($"login task Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

		}

		void Socket_SocketClosed(string reason) => 
			console.AddLine($"Socket Closed: {reason}", Color.Red);
		void Socket_SocketOpened() =>
			console.AddLine("Socket Opened", Color.Green);
		void Socket_ErrorReceived(Exception e, string message)
		{
			console.AddLine($"Socket Error: {message}", Color.Red);
			console.AddLine($"Socket Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
			console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
		}
		void OnPacketReceived(ArchipelagoPacketBase packet) =>
			console.AddLine($"Data Received: {packet.ToJObject()}", Color.Gray);
		void OnPacketsSend(ArchipelagoPacketBase[] packets)
		{
			foreach (var packet in packets)
				console.AddLine($"Data Send: {packet.PacketType}", Color.Gray);
		}

	}
}
