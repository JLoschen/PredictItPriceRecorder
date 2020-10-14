using Common.Logging;
using Menards.Merch.Configuration.Ninject;
using Menards.Merch.Configuration.ConnectionFactory;
using Ninject.Modules;
using OGIR.GuestEmails.Archiver;
using OGIR.GuestEmails.Factory;
using OGIR.GuestEmails.Service;
using System.Xml.Serialization;
using Menards.Merch.BOA.Core;

namespace OGIR.GuestEmails.Ninject
{
    public class GuestEmailsNinjectModule : NinjectModule
    {
        private readonly ILog _log;
        private readonly GuestEmailsConfig _config;

        public GuestEmailsNinjectModule(ILog log, GuestEmailsConfig config)
        {
            _log = log;
            _config = config;
        }

        public override void Load()
        {
            if (Kernel == null)
                return;

            Kernel.Bind<ILog>().ToConstant(_log);
            Kernel.Bind<HostServerType>().ToConstant(_config.HostType);
            Kernel.Bind<IGuestEmailsService>().To<GuestEmailsService>();
            Kernel.Bind<IGuestEmailsFactory>().To<GuestEmailsFactory>();
            Kernel.Bind<IMessageArchiver>().To<MessageArchiver>();
            Kernel.BindHttpClient("GuestEmailClient", _config.GuestEmailServiceVersion);
            Kernel.Bind<FileArchiverConfig>().ToConstant(_config.FileArchiverConfig);
        }
    }
}