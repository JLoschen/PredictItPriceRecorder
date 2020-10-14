using System;
using System.AddIn;
using System.Net;
using System.Threading;
using Common.Logging;
using Menards.Merch.BOA.Core;
using Menards.Merch.DataEngine.Plugin;
using Ninject;
using OGIR.GuestEmails.Ninject;

namespace OGIR.GuestEmails
{
    [AddIn("PredictIt.PriceArchiver",
        Description = "Inserts known guest email addresses from vendors into a Teradata table for OGIR to read")]
    public class Main : DataEngineAddInView
    {
        private Timer _predictItQueryTimer;
        protected override void Run()
        {
            Log.Info($"Creating Ninject Kernel for '{HostServerType.ToString()}'.");
            _predictItQueryTimer = new Timer(_ => DoThing(Log),
               null,
               TimeSpan.Zero,
               TimeSpan.FromSeconds(5));

            while (!Stopping)
            {
                Thread.Sleep(1000);
            }
        }

        private void DoThing(ILog log)
        {
            log.Info($"Stuff happened AGAIN3 at {DateTime.Now:hhmmss}");
        }

        protected override void Terminate()
        {
            Log.Info("Terminating Active MQ connection.");
            _predictItQueryTimer.Dispose();
        }
    }
}