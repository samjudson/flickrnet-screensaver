using System;
using System.Collections;
using System.Collections.Specialized;

using FlickrNet;
using FlickrNetScreensaver.Properties;
using System.Collections.Generic;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for ImageManager.
	/// </summary>
	public sealed class ImageManager
	{
		// Store the photo in this collection
        private static readonly List<Photo> initialCollection = new List<Photo>();

		// contains list of photos to download
        private static readonly List<Photo> photosToDownload = new List<Photo>();

		// The flickr instance
		private static readonly Flickr flickr = FlickrFactory.GetInstance();
		private static string sizeRequired;

		private static int nextIndex;
		private static Random rand = new Random();

		static ImageManager()
		{
            sizeRequired = Settings.Default.DrawerImageSize;
		}

		private ImageManager()
		{
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
		}

		public static Photo NextPhoto
		{
			get 
			{
				return photosToDownload[nextIndex];
			}
		}

		public static Uri NextPhotoUrl
		{
			get
			{
				return CalcUrl(photosToDownload[nextIndex]);
			}
		}

		public static void PopPhoto()
		{
			photosToDownload.RemoveAt(nextIndex);

			if( photosToDownload.Count == 0 )
				photosToDownload.AddRange(initialCollection);

			nextIndex = rand.Next(0, photosToDownload.Count);
		}

		private static Uri CalcUrl(Photo p)
		{
            if (sizeRequired == "Small")
            {
                return p.SmallUrl;
            }

            if (sizeRequired == "Medium" && p.DoesMediumExist)
            {
                return p.MediumUrl;
            }

            if (sizeRequired == "Medium" && !p.DoesMediumExist)
            {
                return p.SmallUrl;
            }

            // sizeRequired == "Large"
            if (p.DoesLargeExist) return p.LargeUrl;
            if (p.DoesMediumExist) return p.MediumUrl;

            return p.SmallUrl;
		}
	}
}
