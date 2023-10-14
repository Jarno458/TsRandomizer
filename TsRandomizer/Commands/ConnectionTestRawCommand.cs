using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using TsRandomizer.Screens.Console;
using System.Threading;
using System.Text;
using Archipelago.MultiClient.Net.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Models;
using Color = Microsoft.Xna.Framework.Color;

namespace TsRandomizer.Commands
{
	class ConnectionTestRawCommand : ConsoleCommand
	{
		const int ReceiveChunkSize = 1024;
		const int SendChunkSize = 1024;

		static ArchipelagoPacketConverter converter = new ArchipelagoPacketConverter();

		public override string Command => "test-connect-raw";
		public override string ParameterUsage => "<server> <username> <password?>";

		BlockingCollection<Tuple<ArchipelagoPacketBase, TaskCompletionSource<bool>>> sendQueue;

		GameConsole console;

		ClientWebSocket webSocket;
		Task pollTask;
		Task sendTask;
		ArchipelagoPacketBase loginResult;

		void Reset()
		{
			if(pollTask != null)
				pollTask.Dispose();

			if (sendTask != null)
				sendTask.Dispose();

			pollTask = null;
			sendTask = null;

			sendQueue = new BlockingCollection<Tuple<ArchipelagoPacketBase, TaskCompletionSource<bool>>>();

			webSocket = null;
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

			var task = Task.Run(() => AttemptToConnect(server, user, password));
			task.Wait(TimeSpan.FromSeconds(15));

			return true;
		}

		async void AttemptToConnect(string server, string user, string password)
		{
			//var uri = new UriBuilder(server) { Scheme = "wss" }.Uri;
			var uri = new Uri($"wss://{server}");
    		webSocket = new ClientWebSocket();

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;

			console.AddLine($"Connecting to '{server}' using raw websocket client:", Color.Yellow);
			console.AddLine($"Calculated URI to {uri}", Color.White);

			try
			{
				console.AddLine("Attempting connection to server", Color.White);

				await webSocket.ConnectAsync(uri, CancellationToken.None);
			}
			catch (AggregateException ae)
			{
				console.AddLine($"Websocket status: {webSocket.State}, CloseStatus? '{webSocket.CloseStatus}', CloseReason: '{webSocket.CloseStatusDescription}'", Color.Brown);

				console.AddLine($"connection task failed with AggregateException[{ae.InnerExceptions.Count}]:", Color.Red);

				foreach (var e in ae.InnerExceptions)
				{
					console.AddLine($"  Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
				}
			}
			catch (Exception e)
			{
				console.AddLine($"Websocket status: {webSocket.State}, CloseStatus? '{webSocket.CloseStatus}', CloseReason: '{webSocket.CloseStatusDescription}'", Color.Brown);

				console.AddLine($"connection task Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

			console.AddLine($"Websocket status: {webSocket.State}, CloseStatus? {webSocket.CloseStatus}, CloseReason: {webSocket.CloseStatusDescription}", Color.White);

			_ = Task.Run(PollingLoop);
			_ = Task.Run(SendLoop);

			try
			{
				await SendMultiplePacketsAsync(new ConnectPacket
				{
					Game = "Timespinner",
					ItemsHandling = ItemsHandlingFlags.NoItems,
					Name = user,
					Password = password,
					RequestSlotData = false,
					Tags = Array.Empty<string>(),
					Uuid = Guid.Empty.ToString(),
					Version = new NetworkVersion(0, 4, 1)
				});
			}
			catch (AggregateException ae)
			{
				console.AddLine($"Send connect task AggregateException[{ae.InnerExceptions}]:", Color.Red);

				foreach (var e in ae.InnerExceptions)
				{
					console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

				}
			}
			catch (Exception e)
			{
				console.AddLine($"send connect task Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
				console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
			}

			console.AddLine("awaiting login result", Color.White);

			var started = DateTime.Now;

			while ((DateTime.Now - started).Seconds < 15)
			{
				if(loginResult == null)
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

		async Task PollingLoop()
		{
			var buffer = new byte[ReceiveChunkSize];

			while (webSocket != null && webSocket.State == WebSocketState.Open)
			{
				string message = null;

				try
				{
					message = await ReadMessageAsync(buffer);
				}
				catch (AggregateException ae)
				{
					console.AddLine($"PollingLoop: AggregateException[{ae.InnerExceptions}]:", Color.Red);

					foreach (var e in ae.InnerExceptions)
					{
						console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
						console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

					}

					continue;
				}
				catch (Exception e)
				{
					console.AddLine($"PollingLoop: Socket Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

					continue;
				}

				console.AddLine($"Json Received: {message}", Color.DarkGray);

				var packets = JsonConvert.DeserializeObject<List<ArchipelagoPacketBase>>(message, converter);
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
		}

		async Task SendLoop()
		{
			while (webSocket != null && webSocket.State == WebSocketState.Open)
			{
				try
				{
					await HandleSendBuffer();
				}
				catch (AggregateException ae)
				{
					console.AddLine($"SendLoop: AggregateException[{ae.InnerExceptions}]:", Color.Red);

					foreach (var e in ae.InnerExceptions)
					{
						console.AddLine($"SendLoop Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
						console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

					}
				}
				catch (Exception e)
				{
					console.AddLine($"SendLoop Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
					console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
				}
			}
		}

		async Task<string> ReadMessageAsync(byte[] buffer)
		{
			var stringResult = new StringBuilder();

			WebSocketReceiveResult result;
			do
			{
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close)
				{
					try
					{
						await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
					}
					catch (AggregateException ae)
					{
						console.AddLine($"ReadMessageAsync (Close): AggregateException[{ae.InnerExceptions}]:", Color.Red);

						foreach (var e in ae.InnerExceptions)
						{
							console.AddLine($"  Exception: [{e.GetType().FullName}]{e.Message}", Color.Red);
							console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);

						}
					}
					catch (Exception e)
					{
						console.AddLine($"ReadMessageAsync (Close): Socket Exception: [{e.GetType().FullName}]: {e.Message}", Color.Red);
						console.AddLine($"  Stacktrace: {e.StackTrace}", Color.Red);
					}

					console.AddLine("Socket Closed", Color.Red);
				}
				else
				{
					stringResult.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
				}
			} while (!result.EndOfMessage);

			return stringResult.ToString();
		}

		async Task HandleSendBuffer()
		{
			var packetList = new List<ArchipelagoPacketBase>();
			var tasks = new List<TaskCompletionSource<bool>>();

			var firstPacketTuple = sendQueue.Take();
			packetList.Add(firstPacketTuple.Item1);
			tasks.Add(firstPacketTuple.Item2);
			while (sendQueue.TryTake(out var packetTuple))
			{
				packetList.Add(packetTuple.Item1);
				tasks.Add(packetTuple.Item2);
			}

			if (!packetList.Any())
				return;

			if (webSocket.State != WebSocketState.Open)
				throw new ArchipelagoSocketClosedException();

			var packets = packetList.ToArray();

			var packetAsJson = JsonConvert.SerializeObject(packets);
			var messageBuffer = Encoding.UTF8.GetBytes(packetAsJson);
			var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

			for (var i = 0; i < messagesCount; i++)
			{
				var offset = (SendChunkSize * i);
				var count = SendChunkSize;
				var lastMessage = ((i + 1) == messagesCount);

				if ((count * (i + 1)) > messageBuffer.Length)
					count = messageBuffer.Length - offset;

				await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count),
					WebSocketMessageType.Text, lastMessage, CancellationToken.None);
			}

			foreach (var task in tasks)
				task.TrySetResult(true);

			foreach (var packet in packets)
				console.AddLine($"Data Send: {packet.PacketType}", Color.Gray);
		}

		public Task SendMultiplePacketsAsync(params ArchipelagoPacketBase[] packets)
		{ 
			var task = new TaskCompletionSource<bool>();

			foreach (var packet in packets)
				sendQueue.Add(new Tuple<ArchipelagoPacketBase, TaskCompletionSource<bool>>(packet, task));

			return task.Task;
		}
	}
}
