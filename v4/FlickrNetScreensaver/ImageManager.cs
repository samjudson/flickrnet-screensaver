using System;
using System.Net;
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
        private static readonly List<Photo> InitialCollection = new List<Photo>();

		// contains list of photos to download
        private static readonly List<Photo> PhotosToDownload = new List<Photo>();

        // contains 5 photos to use in event of network failure
        private static readonly List<Photo> BackupPhotos = new List<Photo>();

		// The flickr instance
	    private static readonly string SizeRequired;

		private static int _nextIndex;
        private static int _nextBackupPhoto;
		private static readonly Random Rand = new Random();

		static ImageManager()
		{
            BackupPhotoCount = 5;
            SizeRequired = Settings.Default.DrawerImageSize;
		}

		/// <summary>
		/// Initialise the ImageManager with a collection of photos.
		/// </summary>
		/// <param name="photos"></param>
		public static void Initialise(List<Photo> photos)
		{
			InitialCollection.AddRange(photos);
			PhotosToDownload.AddRange(photos);

			_nextIndex = Rand.Next(0, PhotosToDownload.Count);

            var t = new Thread(InitialiseBackupPhotos);
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
                return _nextBackupPhoto++ % BackupPhotoCount;
            }
        }
		public static Photo NextPhoto
		{
			get 
			{
                if (!IsNetworkConnection)
                {
                    var i = NextBackupPhoto;
                    Debug.WriteLine("Disconnected. Download backup photo " + i);
                    return BackupPhotos[i];
                }
                var p = PhotosToDownload[_nextIndex];
                PopPhoto();
                return p;
			}
		}

        private static void PopPhoto()
		{
			PhotosToDownload.RemoveAt(_nextIndex);

			if( PhotosToDownload.Count == 0 )
				PhotosToDownload.AddRange(InitialCollection);

			_nextIndex = Rand.Next(0, PhotosToDownload.Count);
		}

		public static Uri CalcUrl(Photo p)
		{
            if (BackupPhotos.Contains(p))
            {
                Debug.WriteLine("Calculate Url for backup photo " + p.PhotoId);
                return new Uri(CalculateBackupFilename(p));
            }

            if (SizeRequired == "Small")
            {
                return new Uri(p.SmallUrl);
            }

            if (SizeRequired == "Medium" && p.DoesMediumExist)
            {
                return new Uri(p.MediumUrl);
            }

            if (SizeRequired == "Medium" && !p.DoesMediumExist)
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
            var path = CalculateBackupDirectory();
            if (Directory.Exists(path)) Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            if (BackupPhotos.Count != 0) return;

            for (var i = 0; i < BackupPhotoCount; i++)
            {
                var p = InitialCollection[new Random().Next(0, InitialCollection.Count)];

                Debug.WriteLine("Downloading backup photo " + p.PhotoId);

                var filename = CalculateBackupFilename(p);

                var url = CalcUrl(p);

                using (var client = new WebClient())
                {
                    client.DownloadFile(url, filename);
                }

                Debug.WriteLine("File successfully downloaded to " + filename);

                BackupPhotos.Add(p);
            }
        }

        private static string CalculateBackupFilename(Photo p)
        {
            var filename = CalculateBackupDirectory();
            filename = Path.Combine(filename, p.PhotoId + ".jpg");
            return filename;
        }

        private static string CalculateBackupDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "BackupFiles");
            return path;
        }


	}
}
