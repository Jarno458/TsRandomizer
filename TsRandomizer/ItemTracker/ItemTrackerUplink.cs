using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	public static class ItemTrackerUplink
	{
        static readonly string StateFilePath = Path.GetTempPath() + "TsRandomizerItemTrackerState";
        // NOTE FileOptions.DeleteOnClose is a nice idea but creates issues if a file handle still in use by an active process is unmapped.
        // This can be done if only the game creates/deletes files and the tracker refreshes until it sees a file.
        // That would also be a good first step toward supporting multiple simultaneous instances (game could append PID to filename, tracker would need selection UI)
        static readonly FileStream FileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, 4096);
        const int serializerOverheadSize = 500;
        const int fileSize = sizeof(bool) * ItemTrackerState.NumberOfItems + serializerOverheadSize;
		static readonly MemoryMappedFile MemoryMappedFile = GetMemoryMappedFile();
		static ItemTrackerState lastSuccessfullRead;

		public static void UpdateState(ItemTrackerState state)
		{
			var formatter = new BinaryFormatter();

			using (var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Write))
				formatter.Serialize(stream, state);
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
			return MemoryMappedFile.CreateViewStream(0, FileStream.Length, access);
		}

		static MemoryMappedFile GetMemoryMappedFile()
		{
            try
            {
                FileStream.SetLength(fileSize);
                MemoryMappedFile m = MemoryMappedFile.CreateFromFile(FileStream,"TsRandomizerItemTrackerState",FileStream.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true);
                return m;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("ERROR opening MemoryMappedFile: " + e.ToString());
            }
            return null;
		}
	}
}
