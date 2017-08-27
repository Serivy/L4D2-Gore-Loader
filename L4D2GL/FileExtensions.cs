namespace L4D2GL
{
    public static class FileExtensions
    {

        public static bool AppIdContains(this string str, int appId)
        {
            return str.Contains(appId.ToString());
        }

        public static bool InfContains(this string str, int appId)
        {
            return str.Contains(string.Format("appID={0}", appId.ToString()));
        }

        public static string GetL4D2Version(this string str)
        {
            string version = string.Empty;

            if (str != null)
            {
                // Browse through each line in the content
                foreach (var line in str.Split('\n'))
                {   // Find the line with the PatchVersion tag
                    if (line != null && line.StartsWith("PatchVersion=") && line.EndsWith("\r"))
                    {   // Replace this whole line with the new appid tag.
                        version = line.Replace("PatchVersion=", "").Replace("\r", "");
                    }
                }
            }

            // Return whatever version was found.
            return version;
        }

        public static string UpdateInfAppId(this string str, int appId)
        {
            string returnedInfContents = str;

            // Browse through each line in the content
            foreach (var line in str.Split('\n'))
            {   // Find the line with the app id tag
                if (line.StartsWith("appID="))
                {   // Replace this whole line with the new appid tag.
                    returnedInfContents = str.Replace(line, string.Format("appID={0}", appId.ToString()));
                }
            }

            // return unmodified contents or updated depending on if appid was found.
            return returnedInfContents;
        }

    }
}