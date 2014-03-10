using System;
using System.Collections.Generic;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace FlickrNetScreensaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if !DEBUG
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
#endif
			XmlConfigurator.Configure();

			if (args.Length > 0)
			{
				string command = args[0].ToLower().Trim().Substring(0, 2);
				// load the config stuff
				switch(command)
				{
					case "/c":
						Application.EnableVisualStyles();
						Application.DoEvents();
						Application.Run(new FConfigure());
						break;
					case "/s":
						Application.Run(new FScreensaver(0));
						break;
					case "/p":
						break;
				}
			}
			else // there are no arguments...nevertheless, do something!
			{

#if DEBUG
				DialogResult result = MessageBox.Show("Configure or not?", "Configure?", MessageBoxButtons.YesNoCancel);
				if( result == DialogResult.Cancel ) return;
				if( result == DialogResult.Yes )
				{
					Application.EnableVisualStyles();
					Application.DoEvents();
					Application.Run(new FConfigure());
				}
				else
				{
					Application.Run(new FScreensaver(0));
				}
#endif
#if !DEBUG
				Application.Run(new FScreensaver(0));
#endif
			}
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show("The application has performed an exception. Please report to Sam Judson (sam@wackylabs.net)\r\n" + e.Exception.Message);
			Application.Exit();
		}

    }
}