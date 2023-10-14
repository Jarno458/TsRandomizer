using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json;
using TsRandomizer.Screens.Console;
using WebSocketSharp;
using Color = Microsoft.Xna.Framework.Color;

namespace TsRandomizer.Commands
{
	class ConnectionTestRawCommand : ConsoleCommand
	{
		static ArchipelagoPacketConverter converter = new ArchipelagoPacketConverter();

		public override string Command => "test-connect-raw";
		public override string ParameterUsage => "<server> <username> <password?>";

		GameConsole console;

		ArchipelagoPacketBase loginResult;
		
		void Reset()
		{
			loginResult = null;
		}

		public override bool Handle(GameConsole gameConsole, string[] parameters)
		{
			console = gameConsole;

			Reset();

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
			var uri = new Uri($"wss://{server}");

			console.AddLine($"Connecting to '{server}' using AP client:", Color.Yellow);
			
			var socket = new WebSocket(uri.ToString());
			if (socket.IsSecure)
				socket.SslConfiguration.EnabledSslProtocols = (SslProtocols)3072 | (SslProtocols)12288;

			console.AddLine($"Calculated URI to {socket.Url}", Color.White);
	
			//socket.
			
			socket.OnError += Socket_ErrorReceived;
			socket.OnOpen += Socket_SocketOpened;
			socket.OnClose += Socket_SocketClosed;
			socket.OnMessage += Socket_OnMessage;

			try
			{
				console.AddLine("Attempting connection to server, with timeout of 5 seconds", Color.White);
				socket.ConnectAsync();

				var connectStartTime = DateTime.Now;
				while ((DateTime.Now - connectStartTime).Seconds < 5)
				{
					if(socket.ReadyState == WebSocketState.Open)
						break;
				}
				
				if (socket.ReadyState == WebSocketState.Open)
					console.AddLine("connection to server was established", Color.White);
				else
					console.AddLine("connection to server failed", Color.Orange);

				console.AddLine($"connection to server with status: {socket.ReadyState}, Alive: {socket.IsAlive}", Color.White);
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
				console.AddLine($"connection to server Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

			if (socket.ReadyState != WebSocketState.Open)
				console.AddLine("Connection failed, aborting login", Color.Orange);
			else
				console.AddLine("Attemting login to server, with timeout of 5 seconds", Color.White);

			var packets = new List<ArchipelagoPacketBase> {
				new ConnectPacket {
					Game = "Timespinner",
					ItemsHandling = ItemsHandlingFlags.NoItems,
					Name = user,
					Password = password,
					RequestSlotData = false,
					Tags = new string[0],
					Uuid = Guid.Empty.ToString(),
					Version = new NetworkVersion(0, 4, 1)
				}
			};
			var packetAsJson = JsonConvert.SerializeObject(packets);

			try
			{
				socket.SendAsync(packetAsJson, success => {
					console.AddLine($"Sending ConnectPacket result: {success}", Color.White);
				});
			}
			catch (AggregateException ae)
			{
				console.AddLine($"login packet send failed with AggregateException[{ae.InnerExceptions.Count}]:", Color.Red);

				foreach (var e in ae.InnerExceptions)
				{
					console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

				}
			}
			catch (Exception e)
			{
				console.AddLine($"login packet send Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

			console.AddLine("awaiting login result", Color.White);

			var started = DateTime.Now;

			while ((DateTime.Now - started).Seconds < 15)
			{
				if (loginResult == null)
					continue;

				if (loginResult.PacketType == ArchipelagoPacketType.Connected)
					console.AddLine("Successfully logged in", Color.Green);
				if (loginResult.PacketType == ArchipelagoPacketType.ConnectionRefused)
					console.AddLine("Failed to log in", Color.Red);

				Reset();

				return;
			}

			Reset();

			console.AddLine("Login timed out", Color.Red);
		}

		void Socket_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.IsPing)
				console.AddLine($"Ping Received", Color.Gray);
			if (e.IsBinary)
				console.AddLine($"Binary Received {Convert.ToBase64String(e.RawData)}", Color.Gray);
			if (e.IsText)
				console.AddLine($"Json Received: {e.Data}", Color.DarkGray);

			var packets = JsonConvert.DeserializeObject<List<ArchipelagoPacketBase>>(e.Data, converter);
			if (packets == null)
			{
				console.AddLine("Json to packet conversion yielded empty list", Color.Orange);
			}
			else
			{
				foreach (var packet in packets)
				{
					console.AddLine($"Data Received: {packet.ToJObject()}", Color.Gray);

					if (packet.PacketType == ArchipelagoPacketType.Connected || packet.PacketType == ArchipelagoPacketType.ConnectionRefused)
						loginResult = packet;
				}
			}
		}

		void Socket_SocketClosed(object sender, CloseEventArgs e) =>
			console.AddLine($"Socket Closed: Code: {e.Code}, Reason: {e.Reason}, Clean: {e.WasClean}", Color.Red);
		void Socket_SocketOpened(object sender, EventArgs e) =>
			console.AddLine("Socket Opened", Color.Green);
		void Socket_ErrorReceived(object sender, ErrorEventArgs e)
		{
			console.AddLine($"Socket Error: {e.Message}", Color.Red);
			console.AddLine($"Socket Exception: [{e.Exception.GetType().FullName}]: {e.Exception}", Color.Red);
			console.AddLine($"  Stacktrace: {e.Exception.StackTrace}", Color.Red);
		}
	}
}
