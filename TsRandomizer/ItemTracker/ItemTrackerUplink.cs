using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	public static class ItemTrackerUplink
	{
		const int SerializerOverheadSize = 500;
		const int FileSize = sizeof(bool) * ItemTrackerState.NumberOfItems + SerializerOverheadSize;

		static readonly string StateFilePath = Path.GetTempPath() + "TsRandomizerItemTrackerState";

		static ItemTrackerState lastSuccessfullRead;

		public static void UpdateState(ItemTrackerState state)
		{
			try
			{
				var formatter = new BinaryFormatter();

				using (var fileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
					FileShare.ReadWrite, FileSize))
				{
					fileStream.SetLength(FileSize);

					using (var memmoryMappedFIle = MemoryMappedFile.CreateFromFile(fileStream,
						"TsRandomizerItemTrackerState", 0, MemoryMappedFileAccess.ReadWrite, null,
						HandleInheritability.Inheritable, true))
					using (var stream = memmoryMappedFIle.CreateViewStream(0, 0, MemoryMappedFileAccess.Write))
						formatter.Serialize(stream, state);
				}
			}
			catch
			{
			}
		}

		public static ItemTrackerState LoadState()
		{
			var formatter = new BinaryFormatter();

			try
			{
				using (var fileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FileSize))
				{
					fileStream.SetLength(FileSize);

					using (var memmoryMappedFIle = MemoryMappedFile.CreateFromFile(fileStream, "TsRandomizerItemTrackerState", 0, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true))
					using (var stream = memmoryMappedFIle.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
						lastSuccessfullRead = (ItemTrackerState)formatter.Deserialize(stream);
				}
			}
			catch
			{
			}

			return lastSuccessfullRead;
		}
	}
}
