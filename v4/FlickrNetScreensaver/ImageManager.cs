using System;
using System.Collections;
using System.Collections.Specialized;

using FlickrNet;
using FlickrNetScreensaver.Properties;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for ImageManager.
	/// </summary>
	public static class ImageManager
	{
        public static int BackupPhotoCount { get; set; }

		// Store the photo in this collection
        private static readonly List<Photo> initialCollection = new List<Photo>();

		// contains list of photos to download
        private static readonly List<Photo> photosToDownload = new List<Photo>();

        // contains 5 photos to use in event of network failure
        private static readonly List<Photo> backupPhotos = new List<Photo>();

		// The flickr instance
		private static readonly Flickr flickr = FlickrFactory.GetInstance();
		private static string sizeRequired;

		private static int nextIndex;
        private static int nextBackupPhoto;
		private static Random rand = new Random();

		static ImageManager()
		{
            BackupPhotoCount = 5;
            sizeRequired = Settings.Default.DrawerImageSize;
		}

		/// <summary>
		/// Initialise the ImageManager with a collection of photos.
		/// </summary>
		/// <param name="photos"></param>
		public static void Initialise(List<Photo> photos)
		{
			initialCollection.AddRange(photos);
			photosToDownload.AddRange(photos);

			nextIndex = rand.Next(0, photosToDownload.Count);

            Thread t = new Thread(new ThreadStart(InitialiseBackupPhotos));
            t.Start();
		}

        public static bool IsNetworkConnection
        {
            get
            {
                return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            }
        }

        private static int NextBackupPhoto
        {
            get
            {
                return nextBackupPhoto++ % BackupPhotoCount;
            }
        }
		public static Photo NextPhoto
		{
			get 
			{
                if (!IsNetworkConnection)
                {
                    int i = NextBackupPhoto;
                    Debug.WriteLine("Disconnected. Download backup photo " + i);
                    return backupPhotos[i];
                }
                Photo p = photosToDownload[nextIndex];
                PopPhoto();
                return p;
			}
		}

        private static void PopPhoto()
		{
			photosToDownload.RemoveAt(nextIndex);

			if( photosToDownload.Count == 0 )
				photosToDownload.AddRange(initialCollection);

			nextIndex = rand.Next(0, photosToDownload.Count);
		}

		public static Uri CalcUrl(Photo p)
		{
            if (backupPhotos.Contains(p))
            {
                Debug.WriteLine("Calculate Url for backup photo " + p.PhotoId);
                return new Uri(CalculateBackupFilename(p));
            }

            if (sizeRequired == "Small")
            {
                return new Uri(p.SmallUrl);
            }

            if (sizeRequired == "Medium" && p.DoesMediumExist)
            {
                return new Uri(p.MediumUrl);
            }

            if (sizeRequired == "Medium" && !p.DoesMediumExist)
            {
                return new Uri(p.SmallUrl);
            }

            // sizeRequired == "Large"
            if (p.DoesLargeExist) return new Uri(p.LargeUrl);
            if (p.DoesMediumExist) return new Uri(p.MediumUrl);

            return new Uri(p.SmallUrl);
		}

        private static void InitialiseBackupPhotos()
        {
            string path = CalculateBackupDirectory();
            if (Directory.Exists(path)) Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            if (backupPhotos.Count == 0)
            {
                for (int i = 0; i < BackupPhotoCount; i++)
                {
                    Photo p = initialCollection[new Random().Next(0, initialCollection.Count)];

                    Debug.WriteLine("Downloading backup photo " + p.PhotoId);

                    string filename = CalculateBackupFilename(p);

                    Uri url = CalcUrl(p);

                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        client.DownloadFile(url, filename);
                    }

                    Debug.WriteLine("File successfully downloaded to " + filename);

                    backupPhotos.Add(p);
                }
            }
        }

        private static string CalculateBackupFilename(Photo p)
        {
            string filename = CalculateBackupDirectory();
            filename = Path.Combine(filename, p.PhotoId + ".jpg");
            return filename;
        }

        private static string CalculateBackupDirectory()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "BackupFiles");
            return path;
        }


	}
}
