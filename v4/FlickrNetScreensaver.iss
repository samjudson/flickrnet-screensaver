
[Setup]
AppName=Flickr.Net Screensaver
AppVerName=Flickr.Net Screensaver 4
InfoAfterFile=
ShowLanguageDialog=yes
AppPublisherURL=http://www.codeplex.com/FlickrNetScreensaver
CreateAppDir=true
DisableFinishedPage=false
DisableReadyPage=true
OutputBaseFilename=FlickerScreensaverSetup4
PrivilegesRequired=poweruser
DefaultGroupName=Flickr.Net Screensaver
AppCopyright=Copyright Sam Judson
InfoBeforeFile=readme.txt
DefaultDirName={pf}\Wackylabs\Flickr.Net Screensaver
AllowUNCPath=false
AppPublisher=Wackylabs
AppVersion=4.2

[Dirs]

[Files]
Source: FlickrNetScreensaver\bin\Release\FlickrNetScreensaver.scr; DestDir: {app}
Source: FlickrNetScreensaver\bin\Release\FlickrNet.dll; DestDir: {app}
Source: FlickrNetScreensaver\bin\Release\log4net.dll; DestDir: {app}

[Run]
Filename: {sys}\rundll32.exe; Parameters: desk.cpl,InstallScreenSaver {app}\FlickrNetScreensaver.scr; WorkingDir: {sys}; Description: Install Screensaver Now.; Flags: runasoriginaluser

[UninstallDelete]
Name: {app}; Type: filesandordirs
Name: {group}; Type: filesandordirs

[_ISToolPreCompile]

[Icons]
Name: {group}\Uninstall Flickr.Net Screensaver; Filename: {uninstallexe}
Name: {group}\Visit Wackylabs.Net; Filename: http://www.wackylabs.net/
Name: {group}\Visit Codeplex.com; Filename: http://www.codeplex.com/FlickrNetScreensaver
