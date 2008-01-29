using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	internal class Utils
	{
		public Utils()
		{
		}

		public static Stream GetSettingsReadStream()
		{
			IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
			IsolatedStorageFileStream stream = new IsolatedStorageFileStream("settings.bin", FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, file);
			return stream;
		}

		public static Stream GetSettingsWriteStream()
		{
			IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
			IsolatedStorageFileStream stream = new IsolatedStorageFileStream("settings.bin", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, file);
			return stream;
		}
	}
}
