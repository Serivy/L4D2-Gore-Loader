using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L4D2GL
{
    public static class Constants
    {
        public const string Version = "1.3.0.0";
        public const string resourcePath = @"pack://application:,,,/L4D2GL;component/Resources/";
        public const Int32 TimerTime = 5000;
        public const string GameExe = "left4dead2.exe";
        public const string CommonL4DPath = "steamapps\\common\\left 4 dead 2";
        public const string regPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam\\";
        public const string l4d2glPath = "HKEY_CURRENT_USER\\SOFTWARE\\L4D2GL\\";
        public const string regL4D2GLLocalUser = "SOFTWARE\\L4D2GL\\";
        public const string RegKeyL4D2GLFolder = "L4D2Folder";
        public const string RegKeyAcceptedDisclaimer = "AcceptedDisclaimer";
        public const string RegKeySteam = "InstallPath";
        public const string steamAppFile = "steam_appid.txt";
        public const string steamInfFile = "left4dead2\\steam.inf";
        public const int normalAppId = 550;
        public const int dedicatedAppId = 560;
        public const string ConfigBadPathCaption = "Invalid Path";
        public const string ConfigBadPathMessage = "Unable to find the L4D2 executable in this directory. Please navigate to the main left 4 dead 2 folder.";
        
        public const string WarningConflictMessage = "The files are mixed for both Gore and Normal L4D2. An update may have overritten one of the files. Switching versions will make them both the same.";
        public const string ErrorUnknownMessage = "Can't determine what version is being used. Please press the 'Verify integrity of game cache...' button under steams L4D2 properties AND make sure you are pointing to a valid L4D2 Directory in configuration.";
        public const string ErrorBadPathMessage = "L4D2 Gore loader could not detect a valid L4D2 folder. Please press configure and browse to your L4D2.exe";
        public const string ErrorUnknownVersion = "Unknown";
        public const string ConfigUIBadPath = "Unable to load game path";
        public const string Disclaimer = "You agree that the author and hosting provider of the Left 4 Dead 2 Gore Loader are not responsible for any damage that may occur using this program. You will not use this program to break the law in your country. You accept responsibly if you are banned from the steam service through use of this program.\n\nDo you accept these conditions and revoke all liability from the author?";

        public const string Left4Dead2VersionString = "Left 4 Dead 2 Version: {0}";

        public const string ProjectURL = "http://l4d2gl.codeplex.com/";
    }
}
