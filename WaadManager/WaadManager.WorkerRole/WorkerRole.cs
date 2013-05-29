using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using WaadManager.Common;

namespace WaadManager.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("WaadManager.WorkerRole entry point called", "Information");

            var confMgr = new ConferenceScheduleProcessor();
            var manager = new UserGroupProcessor();
            while (true)
            {
                Thread.Sleep(3000);
                Trace.WriteLine("Working", "Information");
                manager.RunUpdate();
                confMgr.RunUpdate();
            }
        }

        public override bool OnStart()
        {
			DiagnosticMonitorConfiguration diagnosticConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration();
			DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticConfiguration);

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            if (!RoleEnvironment.IsAvailable)
                throw new InvalidOperationException("RoleEnvironment not available");

            return base.OnStart();
        }
    }
}
