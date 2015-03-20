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
        public static bool ViewedAllPhotos { get; set; }
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
        public static bool NeedToCleanDirectory { get; set; }
		private static readonly Random Rand = new Random();

        private static Thread _downloadThread = null;

		static ImageManager()
		{
            BackupPhotoCount = 5;
            ViewedAllPhotos = false;
            NeedToCleanDirectory = true;
            SizeRequired = Settings.Default.DrawerImageSize;
		}

		/// <summary>
		/// Initialise the ImageManager with a collection of photos.
		/// </summary>
		/// <param name="photos"></param>
		public static void Initialise(List<Photo> photos)
		{
            InitialCollection.Clear();
			InitialCollection.AddRange(photos);

            PhotosToDownload.Clear();
			PhotosToDownload.AddRange(photos);

            ViewedAllPhotos = false;

			_nextIndex = Rand.Next(0, PhotosToDownload.Count);

            BackupPhotoCount = PhotosToDownload.Count;

            _downloadThread = new Thread(InitialiseBackupPhotos);
            _downloadThread.Start();
		}

        public static void StopAllThreads()
        {
            if (_downloadThread != null)
            {
                _downloadThread.Abort();
                _downloadThread = null;
            }
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
                return _nextBackupPhoto++ % BackupPhotos.Count;
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

            if (PhotosToDownload.Count == 0)
            {
                ViewedAllPhotos = true;
                PhotosToDownload.Clear();
                PhotosToDownload.AddRange(InitialCollection);
            }

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

            BackupPhotos.Clear();

            if (NeedToCleanDirectory)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                Directory.CreateDirectory(path);

                NeedToCleanDirectory = false;
            }

            for (var i = 0; i < BackupPhotoCount; i++)
            {
                var p = InitialCollection[i];

                var filename = CalculateBackupFilename(p);

                try
                {
                    if (!File.Exists(filename))
                    {
                        Debug.WriteLine("Downloading backup photo " + p.PhotoId);

                        var url = CalcUrl(p);

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url, filename);
                        }

                        Debug.WriteLine("File successfully downloaded to " + filename);
                    }
                    else
                    {
                        Debug.WriteLine("No need to download backup photo, it already exists in cache: " + p.PhotoId);
                    }

                    BackupPhotos.Add(p);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error downloading a file: " + ex.ToString());
                }
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
            path = Path.Combine(path, "ImageFiles");
            return path;
        }


	}
}
