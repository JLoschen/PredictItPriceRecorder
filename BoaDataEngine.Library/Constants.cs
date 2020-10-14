using System;
using System.IO;
using System.Reflection;


namespace BoaDataEngine.Library
{
    public static class Constants
    {
        public static string XmlDirectoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "\\";

        public const string SmtpHost = "omdc-mail.menards.net";
        public const string OperatorEmailAddress = "cdpoper@menards.net";
        public const string SkuTeamPublicFolder = "skuman_nooper@menards.net";
        public const string SkuTeamEmailAddress = "isboappsteam@menards.net";
        public const string TestingEmailAddress = "isboappsteam@menards.net";
    }
}
