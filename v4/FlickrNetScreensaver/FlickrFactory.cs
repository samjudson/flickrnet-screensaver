using System;
using System.Net;
using FlickrNet;
using FlickrNetScreensaver.Properties;

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
			if( !String.IsNullOrEmpty(Settings.Default.AuthenticationToken) )
				return GetInstance(Settings.Default.AuthenticationToken);
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
			bool useProxy = Settings.Default.ProxyUse;

            if( !useProxy )
			{
				return WebProxy.GetDefaultProxy();
			}

			WebProxy proxy = new WebProxy();
			proxy.Address = new Uri("http://" + Settings.Default.ProxyIPAddress + ":" + Settings.Default.ProxyPort);
			if( !String.IsNullOrEmpty(Settings.Default.ProxyUsername) )
			{
				NetworkCredential creds = new NetworkCredential();
				creds.UserName = Settings.Default.ProxyUsername;
				creds.Password = Settings.Default.ProxyPassword;
				creds.Domain = Settings.Default.ProxyDomain;
				proxy.Credentials = creds;
			}

			return proxy;
		}
	}
}
