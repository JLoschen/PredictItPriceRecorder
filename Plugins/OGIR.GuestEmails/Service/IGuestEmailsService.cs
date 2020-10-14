using System.Collections.Generic;
using System.Threading.Tasks;
using Menards.Merch.Shared.GuestEmail;

namespace OGIR.GuestEmails.Service
{
    public interface IGuestEmailsService
    {
        //Task<bool> InsertGuestEmails(List<GuestContactModel> models);
        Task InsertGuestEmails(List<GuestContactModel> models);
    }
}