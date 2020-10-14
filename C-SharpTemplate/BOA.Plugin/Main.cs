//-- Copyright © 2017 Menard, Inc.  Eau Claire, WI.  All rights reserved.

using System.AddIn;
using Menards.Merch.DataEngine.Plugin;

namespace BOA.Plugin
{
    [AddIn("BOA.Plugin", Description = "<Add Description for BOA.Plugin>")]
    public class Main : DataEngineAddInView
    {
        protected override void Run()
        {
            while (!Stopping)
            {
                // Add your code in a loop that checks for Stopping
            }
        }
    }
}