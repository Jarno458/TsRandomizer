using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TsRandomizer.ItemTracker
{
	public static class ItemTrackerUplink
	{
		const int SerializerOverheadSize = 500;
		const int FileSize = sizeof(bool) * ItemTrackerState.NumberOfItems + SerializerOverheadSize;

		static readonly string StateFilePath = Path.GetTempPath() + "TsRandomizerItemTrackerState";

        //static readonly FileStream FileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, FileSize);

		//static readonly MemoryMappedFile MemoryMappedFile = GetMemoryMappedFile();
		static ItemTrackerState lastSuccessfullRead;

		public static void UpdateState(ItemTrackerState state)
		{
			var formatter = new BinaryFormatter();

			using (var fileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FileSize))
			using (var memmoryMappedFIle = MemoryMappedFile.CreateFromFile(fileStream, "TsRandomizerItemTrackerState", fileStream.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true))
			using (var stream = memmoryMappedFIle.CreateViewStream(0, fileStream.Length, MemoryMappedFileAccess.Write))
				formatter.Serialize(stream, state);

			//using (var stream = GetMemoryMappedFileStream(MemoryMappedFileAccess.Write))
			//formatter.Serialize(stream, state);
		}

		public static ItemTrackerState LoadState()
		{
			var formatter = new BinaryFormatter();

			try
			{
				using (var fileStream = new FileStream(StateFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FileSize))
				using (var memmoryMappedFIle = MemoryMappedFile.CreateFromFile(fileStream, "TsRandomizerItemTrackerState", fileStream.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true))
				using (var stream = memmoryMappedFIle.CreateViewStream(0, fileStream.Length, MemoryMappedFileAccess.Read))
					lastSuccessfullRead = (ItemTrackerState)formatter.Deserialize(stream);
			}
			catch
			{
			}

			return lastSuccessfullRead;
		}

		/*static Stream GetMemoryMappedFileStream(MemoryMappedFileAccess access)
		{
			return MemoryMappedFile.CreateViewStream(0, FileStream.Length, access);
		}

		static MemoryMappedFile GetMemoryMappedFile()
		{
            try
            {
                //FileStream.SetLength(FileSize);

                var m = MemoryMappedFile.CreateFromFile(FileStream,"TsRandomizerItemTrackerState",FileStream.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true);
                return m;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("ERROR opening MemoryMappedFile: " + e);
            }
            return null;
		}*/
	}
}
