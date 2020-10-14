using System;
using Menards.Merch.BOA.Core;
using OGIR.GuestEmails.Archiver;

namespace OGIR.GuestEmails.Ninject
{
    public class GuestEmailsConfig
    {
        private GuestEmailsConfig() { }
        private const int ArchiveDays = 30;
        private const string ArchivePath = @"C:\GuestEmailArchive";
        public string QueueName { get; private set; }
        public HostServerType HostType { get; private set; }
        public string GuestEmailServiceVersion { get; private set; }
        public TimeSpan WaitForMessageTimeout { get; private set; }
        public FileArchiverConfig FileArchiverConfig { get; private set; }
        public static GuestEmailsConfig Create(HostServerType hostType) => new GuestEmailsConfig
        {
            QueueName = "Guest.Attributes.Merch",
            HostType = hostType,
            GuestEmailServiceVersion = "v1.0",
            WaitForMessageTimeout = TimeSpan.FromSeconds(4),
            FileArchiverConfig = new FileArchiverConfig
            {
                ArchiveDays = ArchiveDays,
                ArchivePath = ArchivePath
            }
        };
    }
}