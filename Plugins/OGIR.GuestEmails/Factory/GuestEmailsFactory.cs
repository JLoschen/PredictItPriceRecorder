using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Common.Logging;
using Menards.Merch.Shared.GuestEmail;

namespace OGIR.GuestEmails.Factory
{
    public class GuestEmailsFactory : IGuestEmailsFactory
    {
        private readonly XmlSerializer _xmlSerializer;
        private readonly ILog _log;

        public GuestEmailsFactory(ILog log, XmlSerializer xmlSerializer)
        {
            _xmlSerializer = xmlSerializer;
            _log = log;
        }

        public List<GuestContactModel> GetGuestEmailModels()
        {
            var guestEmailModels = new List<GuestContactModel>();
            return guestEmailModels;
        }

        private GuestContactModel GetGuestEmailModel()
            => new GuestContactModel
            {
            };
    }
}