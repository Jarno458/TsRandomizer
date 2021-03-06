using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	public static class ItemTrackerUplink
	{
        static readonly string StateFilePath = Path.GetTempPath() + "TsRandomizerItemTrackerState";
        // FIXME only set delete on close if we're the randomizer!
        // killing the underlying file can break things - just leave it?
        //      set listener to attempt reopens on loadstate?
        // cleanup console logs
        // createviewstream(int,int,access) fails if it goes beyond the file size...set file big enough to begin works, make this cleaner
        static readonly FileStream FileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.DeleteOnClose);
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
                System.Console.WriteLine("Opening MemoryMappedFile: " + FileStream.Name);
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
