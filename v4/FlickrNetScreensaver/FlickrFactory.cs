using System;
using System.Net;
using FlickrNet;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for FlickrFactory.
	/// </summary>
	public sealed class FlickrFactory
	{
		private const string key = "0c4db9a5e2b90f6e149c7e617dca77de";
		private const string secret = "d01c1f947c94f427";
		private FlickrFactory()
		{
		}

		public static Flickr GetInstance()
		{
			if( Settings.Contains("AuthToken") )
				return GetInstance(Settings.Get("AuthToken"));
			else
				return GetInstance(null);
		}

		public static Flickr GetInstance(string token)
		{
			Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);

			Flickr f = new Flickr(key, secret, token);

			f.Proxy = GetProxy();
			return f;
		}

		public static WebProxy GetProxy()
		{
			bool useProxy = false;
			if( Settings.Contains("UseProxy") )
			{
				useProxy = bool.Parse(Settings.Get("UseProxy"));
			}

			if( !useProxy )
			{
				return WebProxy.GetDefaultProxy();
			}

			WebProxy proxy = new WebProxy();
			proxy.Address = new Uri("http://" + Settings.Get("ProxyIPAddress") + ":" + Settings.Get("ProxyPort"));
			if( Settings.Contains("ProxyUsername") && Settings.Get("ProxyUsername").Length > 0 )
			{
				NetworkCredential creds = new NetworkCredential();
				creds.UserName = Settings.Get("ProxyUsername");
				creds.Password = Settings.Get("ProxyPassword");
				creds.Domain = Settings.Get("ProxyDomain");
				proxy.Credentials = creds;
			}

			return proxy;
		}
	}
}
