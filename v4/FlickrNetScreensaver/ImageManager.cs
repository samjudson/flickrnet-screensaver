using System;
using System.Collections;
using System.Collections.Specialized;

using FlickrNet;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for ImageManager.
	/// </summary>
	public sealed class ImageManager
	{
		// Store the photo in this collection
		private static readonly PhotoCollection initialCollection = new PhotoCollection();

		// contains list of photos to download
		private static readonly PhotoCollection photosToDownload = new PhotoCollection();

		// The flickr instance
		private static readonly Flickr flickr = FlickrFactory.GetInstance();
		private static readonly Hashtable sizeCache = new Hashtable();
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
		public static void Initialise(PhotoCollection photos)
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

		public static string NextPhotoUrl
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

		private static string CalcUrl(Photo p)
		{
			string url = null;
			try
			{
				FlickrNet.Sizes s = null;
				if( sizeCache.ContainsKey(p.PhotoId) )
				{
					s = (Sizes)sizeCache[p.PhotoId];
				}
				else
				{
					s = flickr.PhotosGetSizes(p.PhotoId);
					sizeCache.Add(p.PhotoId, s);
				}
				url = s.SizeCollection[s.SizeCollection.Length - 2].Source;

				foreach(FlickrNet.Size size in s.SizeCollection)
				{
					if( sizeRequired == size.Label ) 
						url = size.Source;
				}
			}
			catch(Exception)
			{
			}

			return url;
		}
	}
}
