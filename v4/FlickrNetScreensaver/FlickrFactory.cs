using System;
using System.Net;
using FlickrNet;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for FlickrFactory.
	/// </summary>
	public static class FlickrFactory
	{
		private const string Key = "0c4db9a5e2b90f6e149c7e617dca77de";
		private const string Secret = "d01c1f947c94f427";

	    public static Flickr GetInstance()
		{
		    return GetInstance(Settings.Default.AuthenticationToken);
		}

	    public static Flickr GetInstance(OAuthAccessToken token)
		{
			Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);

			var f = new Flickr(Key, Secret);
            if (token != null)
            {
                f.OAuthAccessToken = token.Token;
                f.OAuthAccessTokenSecret = token.TokenSecret;
            }

			if( Settings.Default.ProxyUse ) f.Proxy = GetProxy();
			return f;
		}

		public static WebProxy GetProxy()
		{
			var proxy = new WebProxy
			                {
			                    Address =
			                        new Uri("http://" + Settings.Default.ProxyIPAddress + ":" +
			                                Settings.Default.ProxyPort)
			                };
		    if( !String.IsNullOrEmpty(Settings.Default.ProxyUsername) )
			{
				var creds = new NetworkCredential
				                {
				                    UserName = Settings.Default.ProxyUsername,
				                    Password = Settings.Default.ProxyPassword,
				                    Domain = Settings.Default.ProxyDomain
				                };
			    proxy.Credentials = creds;
			}

			return proxy;
		}
	}
}
