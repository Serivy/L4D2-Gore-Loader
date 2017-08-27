using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows;

namespace L4D2GL
{
    public class LoaderEngine
    {
        public string l4d2Path { get; set; }
        public bool validPath { get; set; }
        public GoreType goreType = GoreType.Unknown;
        public StatusType status = StatusType.None;
        public string statusMessage = "";
        public string VersionNumber = "";


        public LoaderEngine()
        {
        }

        public void LoadSettings()
        {
            LoadPaths();
        }

        /// <summary>
        /// Try and get the path automaticly and return it.
        /// </summary>
        /// <returns>The path to L4D2 or null if not found.</returns>
        public string GetAutoPath()
        {
            string regSteamPath;
            string loaderPath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;

            // Check the same folder as the loader app.
            if (ValidGamePath(loaderPath))
                return loaderPath;

            // Look in registry for steam entry
            regSteamPath = (string)Registry.GetValue(Constants.regPath, Constants.RegKeySteam, null);
            regSteamPath = string.Format("{0}\\{1}", regSteamPath, Constants.CommonL4DPath);
            if (ValidGamePath(regSteamPath))
                return regSteamPath;

            return null;
        }

        /// <summary>
        /// Returns true if user has accepted disclaimer.
        /// </summary>
        /// <returns></returns>
        public bool IsDisclaimerAccepted()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.regL4D2GLLocalUser);
            return (key != null && key.GetValue(Constants.RegKeyAcceptedDisclaimer) != null);
        }

        /// <summary>
        /// Deletes disclaimer acceptance.
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDisclaimer()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.regL4D2GLLocalUser);
            if (key != null && key.GetValue(Constants.RegKeyAcceptedDisclaimer) != null)
                    key.DeleteValue(Constants.RegKeyAcceptedDisclaimer);
        }

        /// <summary>
        /// Accepts the disclaimer.
        /// </summary>
        /// <returns></returns>
        public void AcceptDisclaimer()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.regL4D2GLLocalUser);
            key.SetValue(Constants.RegKeyAcceptedDisclaimer, true.ToString());
        }

        /// <summary>
        /// Returns manualy set path if it has been set AND is valid, else null.
        /// </summary>
        /// <returns></returns>
        public string GetManualPath()
        {
            string regLoaderPath = (string)Registry.GetValue(Constants.l4d2glPath, Constants.RegKeyL4D2GLFolder, null);
            if (ValidGamePath(regLoaderPath))
                return regLoaderPath;

            return null;
        }

        /// <summary>
        /// Sets the manual path.
        /// </summary>
        /// <param name="path"></param>
        public void SetManualPath(string path)
        {
            if (path == null)
            {   // Delete the key.
                RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.regL4D2GLLocalUser);

                if (key != null && key.GetValue(Constants.RegKeyL4D2GLFolder) != null)
                    key.DeleteValue(Constants.RegKeyL4D2GLFolder);
            }
            else
            {
                Registry.SetValue(Constants.l4d2glPath, Constants.RegKeyL4D2GLFolder, path);
            }
        }

        /// <summary>
        /// Load the paths into the engine.
        /// </summary>
        public void LoadPaths()
        {
            // Try get the manual path, otherwise try get the auto path.
            l4d2Path = GetManualPath();
            if (l4d2Path == null)
                l4d2Path = GetAutoPath();

            // Valid path if it has been set.
            validPath = (l4d2Path != null);
        }

        /// <summary>
        /// Needs cleaning up
        /// </summary>
        /// <param name="type"></param>
        public void SwitchVersion(GoreType type)
        {
            int desiredAppId;
            string infContent;

            desiredAppId = type == GoreType.Gore ? Constants.dedicatedAppId : Constants.normalAppId;

            if (ValidGamePath(l4d2Path))
            {
                // Write the app id to the steam app file.
                WriteFile(string.Format("{0}\\{1}", l4d2Path, Constants.steamAppFile), desiredAppId.ToString()); 

                // Read in inf contents and pull out the appId line.
                infContent = ReadFile(string.Format("{0}\\{1}", l4d2Path, Constants.steamInfFile));
                infContent = infContent.UpdateInfAppId(desiredAppId);
                WriteFile(string.Format("{0}\\{1}", l4d2Path, Constants.steamInfFile), infContent); 
            }
        }

        public void DetectVersionType()
        {
            string appidContents;
            string infContents;
            string version;

            goreType = GoreType.Unknown;
            status = StatusType.Error;
            statusMessage = Constants.ErrorUnknownMessage;
            VersionNumber = Constants.ErrorUnknownVersion;

            if (!ValidGamePath(l4d2Path))
            {
                status = StatusType.Error;
                statusMessage = Constants.ErrorBadPathMessage;
            }
            else
            {
                appidContents = ReadFile(string.Format("{0}\\{1}", l4d2Path, Constants.steamAppFile));
                infContents = ReadFile(string.Format("{0}\\{1}", l4d2Path, Constants.steamInfFile));

                version = infContents.GetL4D2Version();
                if (version != string.Empty)
                {
                    VersionNumber = version;

                    /// Check to see if it is a normal version.
                    if (appidContents.AppIdContains(Constants.normalAppId) && infContents.InfContains(Constants.normalAppId))
                    {
                        goreType = GoreType.Normal;
                        status = StatusType.None;
                    }


                    /// Check to see if gore has been turned on version.
                    if (appidContents.AppIdContains(Constants.dedicatedAppId) && infContents.InfContains(Constants.dedicatedAppId))
                    {
                        goreType = GoreType.Gore;
                        status = StatusType.None;
                    }

                    /// Check to see if there is a missmatch.
                    if ((appidContents.AppIdContains(Constants.dedicatedAppId) && infContents.InfContains(Constants.normalAppId))
                        || (appidContents.AppIdContains(Constants.normalAppId) && infContents.InfContains(Constants.dedicatedAppId)))
                    {
                        status = StatusType.Warning;
                        statusMessage = Constants.WarningConflictMessage;
                    }
                }

            }
        }

        private string ReadFile(string path)
        {
            string fileContents = null;
            StreamReader fileStream;

            if (File.Exists(path))
            {
                fileStream = File.OpenText(path);
                fileContents = fileStream.ReadToEnd();
                fileStream.Close();
            }
            return fileContents;
        }

        private bool WriteFile(string path, string text)
        {
            File.SetAttributes(path, FileAttributes.Normal);
            File.WriteAllText(path, text);
            return true;
        }

        public bool ValidGamePath(string path)
        {
            return File.Exists(string.Format("{0}\\{1}", path, Constants.GameExe));
        }

        public string GetExePath()
        {
            return string.Format("{0}\\{1}", l4d2Path, Constants.GameExe);
        }

    }
}
