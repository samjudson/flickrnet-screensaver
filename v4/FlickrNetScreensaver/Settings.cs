using System;
using System.Collections;
using System.Runtime.Serialization;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for Settings.
	/// </summary>
	public class Settings
	{
		private static Hashtable settings = null;

		static Settings()
		{
			LoadSettings();
		}

		private static void LoadSettings()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

			System.IO.Stream stream = Utils.GetSettingsReadStream();

			if( stream == null )
			{
				settings = new Hashtable();
				return;
			}

			try
			{
				settings = (Hashtable)formatter.Deserialize(stream);
			}
			catch
			{
				settings = new Hashtable();
			}

			stream.Close();
			return;
		}

		public static bool Contains(string key)
		{
			return settings.ContainsKey(key);
		}

		public static void Set(string key, string value)
		{
			if( settings.Contains(key) )
				settings[key] = value;
			else
				settings.Add(key, value);
		}

		public static string Get(string key)
		{
			if( settings.ContainsKey(key) )
				return (string)settings[key];
			else
				return null;
		}

		public static void Add(string key, string value)
		{
			if( settings.Contains(key) )
				settings[key] = value;
			else
				settings.Add(key, value);
		}

		public static void Remove(string key)
		{
			settings.Remove(key);
		}

		public static void SaveSettings()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.IO.Stream stream = Utils.GetSettingsWriteStream();
			formatter.Serialize(stream, settings);
			stream.Close();
		}

	}
}
