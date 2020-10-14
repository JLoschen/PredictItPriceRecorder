using System;
using System.Threading.Tasks;
using Apache.NMS;
using Common.Logging;
using OGIR.GuestEmails.Archiver;
using OGIR.GuestEmails.Factory;
using OGIR.GuestEmails.Service;

namespace OGIR.GuestEmails
{
    public class GuestEmailConsumer
    {
        private readonly ILog _log;
        private readonly IGuestEmailsService _guestEmailService;
        private readonly IGuestEmailsFactory _guestEmailFactory;
        private readonly IMessageArchiver _messageArchiver;

        public GuestEmailConsumer(IGuestEmailsService guestEmailService,
                                  IGuestEmailsFactory guestEmailFactory,
                                  IMessageArchiver messageArchiver,
                                  ILog log)
        {
            _guestEmailService = guestEmailService;
            _guestEmailFactory = guestEmailFactory;
            _messageArchiver = messageArchiver;
            _log = log;
        }

        public async Task Consume(ITextMessage message)
        {
            //var guestAttributes = _guestEmailFactory.DeserializeXml(message.Text);
            //if (guestAttributes == null)
            //{
            //    _log.Info("Failed to deserialize XML. Archiving failed message");
            //    _messageArchiver.ArchiveFailedDeserializationMessage(message);
            //}
            //else
            //{
            //    try
            //    {
            //        var guestEmails = _guestEmailFactory.GetGuestEmailModels(guestAttributes);
            //        await _guestEmailService.InsertGuestEmails(guestEmails);
            //    }
            //    catch (Exception e)
            //    {
            //        _log.Error("GuestEmailService call to insert guest emails failed. Archiving failed message", e);
            //        _messageArchiver.ArchiveFailedMessage(message);
            //    }
            //}
            //message.Acknowledge();
        }
    }
}