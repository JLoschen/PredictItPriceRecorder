using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Menards.Merch.BOA.Core;
using Menards.Merch.Extensions.Http;
using Menards.Merch.Shared.GuestEmail;
using Ninject;

namespace OGIR.GuestEmails.Service
{
    public class GuestEmailsService : IGuestEmailsService
    {
        private readonly HttpClient _guestEmailClient;
        public GuestEmailsService([Named("GuestEmailClient")] HttpClient guestEmailClient, HostServerType hostServerType)
        {
            //To avoid issue with load balancer erroring out after connection has been live for a couple days, hardcode pointing at ec-boaws2
            if(hostServerType == HostServerType.Live)
                guestEmailClient.BaseAddress = new Uri("https://ec-boaws2.menards.net/GuestEmailService/api/v1.0/");

            _guestEmailClient = guestEmailClient;
        }

        public Task InsertGuestEmails(List<GuestContactModel> models)
            => _guestEmailClient.PostLoggedAsync("GuestEmail", models);
    }
}