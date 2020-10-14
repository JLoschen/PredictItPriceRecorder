using Apache.NMS;

namespace OGIR.GuestEmails.Archiver
{
    public interface IMessageArchiver
    {
        void ArchiveFailedMessage(ITextMessage message);
        void ArchiveFailedDeserializationMessage(ITextMessage message);
    }
}