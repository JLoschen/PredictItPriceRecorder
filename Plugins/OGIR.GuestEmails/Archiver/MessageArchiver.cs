using System;
using System.IO;
using System.Linq;
using System.Threading;
using Apache.NMS;
using Common.Logging;

namespace OGIR.GuestEmails.Archiver
{
    public class FileArchiverConfig
    {
        public string ArchivePath { get; set; }
        public int ArchiveDays { get; set; }
    }

    public class MessageArchiver : IMessageArchiver, IDisposable
    {
        private const string DayDirFormat = "yyyy.MM.dd";
        private const string FileTimePrefixFormat = "HH.mm.ss";
        private const string DeserializationDir = "DeserializationErrors";
        private const string ErrorsDir = "Errors";

        private readonly string _archivePath;
        private readonly ILog _log;

        private readonly Timer _archivePurgeTimer;

        public MessageArchiver(FileArchiverConfig config, ILog log)
        {
            _log = log;
            _archivePath = config.ArchivePath;

            _archivePurgeTimer = new Timer(_ => PurgeArchivePath(TimeSpan.FromDays(config.ArchiveDays), config.ArchivePath, log),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(6));
        }

        public void ArchiveFailedMessage(ITextMessage message) => ArchiveMessage(message, ErrorsDir);

        public void ArchiveFailedDeserializationMessage(ITextMessage message) =>
            ArchiveMessage(message, DeserializationDir);

        private void ArchiveMessage(ITextMessage message, string directory)
        {
            var msgIdHash = message.NMSMessageId.GetHashCode().ToString("X");
            var now = DateTime.Now;
            var dir = Path.Combine(directory,$"{now.ToString(DayDirFormat)}");

            var filename = $"{now.ToString(FileTimePrefixFormat)}_{msgIdHash}.xml";

            Directory.CreateDirectory(Path.Combine(_archivePath, dir));
            var fullpath = Path.Combine(_archivePath, dir, filename);

            _log.Info($"Archiving message ({message.NMSMessageId}) to '{fullpath}'");
            File.AppendAllText(fullpath, message.Text);
        }

        private static void PurgeArchivePath(TimeSpan archiveLifespan, string archiveDir, ILog log)
        {
            log.Info("Running message archive purge process...");
            var cuttoffDate = DateTime.Today - archiveLifespan;

            if (!Directory.Exists(archiveDir))
                return;

            foreach (var dayDir in Directory.GetDirectories(archiveDir).Select(d => new DirectoryInfo(d)))
            {
                if (dayDir.Name.Length != DayDirFormat.Length)
                    continue;

                try
                {
                    var date = DateTime.ParseExact(dayDir.Name, DayDirFormat, null);
                    if (date < cuttoffDate)
                    {
                        dayDir.Delete(true);
                    }
                }
                catch (Exception ex)
                {
                    log.Warn($"Error while purging '{dayDir.FullName}' from message archive.", ex);
                }
            }

            log.Info("Purge process complete.");
        }

        public void Dispose()
        {
            _archivePurgeTimer.Dispose();
        }
    }
}