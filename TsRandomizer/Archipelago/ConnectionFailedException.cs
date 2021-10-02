using System;

namespace TsRandomizer.Archipelago
{
	class ConnectionFailedException : Exception
	{
		public ConnectionFailedException(ConnectionFailed connectionResult, string server, string user, string password)
			: base($"Failed to connect to server: {server}, for user {user}. Message: {connectionResult.ErrorMessage}")
		{
		}
	}
}
