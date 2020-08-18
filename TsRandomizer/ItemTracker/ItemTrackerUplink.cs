using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	static class ItemTrackerUplink
	{
		public static void UpdateState(ItemTrackerState state)
		{
			var formatter = new BinaryFormatter();

			using(var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Write))
				formatter.Serialize(stream, state);
		}

		public static ItemTrackerState LoadState()
		{
			var formatter = new BinaryFormatter();

			using (var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Read))
				return (ItemTrackerState)formatter.Deserialize(stream);
		}

		static Stream GetMemoryMappedFileStream(MemoryMappedFileAccess access)
		{
			const int serializerOverheadSize = 500;
			const int numberOfMembers = 25;

			return GetMemoryMappedFile().CreateViewStream(0, sizeof(bool) * numberOfMembers + serializerOverheadSize, access);
		}

		static MemoryMappedFile GetMemoryMappedFile()
		{
			return MemoryMappedFile.CreateOrOpen("TsRandomizerItemTrackerState", 30, MemoryMappedFileAccess.ReadWrite);
		}
	}
}