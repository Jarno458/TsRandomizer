using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	public static class ItemTrackerUplink
	{
		//static readonly MemoryMappedFile MemoryMappedFile = GetMemoryMappedFile();
		static ItemTrackerState lastSuccessfullRead;

		public static void UpdateState(ItemTrackerState state)
		{
			try
			{
				var formatter = new BinaryFormatter();

				using (var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Write))
					formatter.Serialize(stream, state);
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
		}

		public static ItemTrackerState LoadState()
		{
			var formatter = new BinaryFormatter();

			try
			{
				using (var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Read))
					lastSuccessfullRead = (ItemTrackerState)formatter.Deserialize(stream);
			}
			catch (SerializationException)
			{
			}

			return lastSuccessfullRead;
		}

		static Stream GetMemoryMappedFileStream(MemoryMappedFileAccess access)
		{
			const int serializerOverheadSize = 500;

			return GetMemoryMappedFile().CreateViewStream(0, sizeof(bool) * ItemTrackerState.NumberOfItems + serializerOverheadSize, access);
		}

		static MemoryMappedFile GetMemoryMappedFile()
		{
			return MemoryMappedFile.CreateOrOpen("TsRandomizerItemTrackerState", 30, MemoryMappedFileAccess.ReadWrite);
		}
	}
}