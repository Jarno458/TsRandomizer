using System;
using Archipelago.MultiClient.Net;

namespace TsRandomizer.Archipelago
{
	class ConnectionFailedException : Exception
	{
		public ConnectionFailedException(LoginFailure connectionResult, string server, string user, string password)
			: base($"Failed to connect to server: {server}, for user {user}. Message: {string.Join(", ", connectionResult.Errors)}")
		{
		}
	}
}
