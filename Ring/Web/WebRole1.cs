using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using OTP.Ring.Web.Utility;
using OTP.Ring.Common;

namespace OTP.Ring.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            var diagnosticsConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();

            //diagnosticsConfig.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            diagnosticsConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromSeconds(60);
            if (ExtensionMethods.GetConfigurationSetting("loggingLevel") == Common.Logger.LogType.ERROR.ToString())
            {
                diagnosticsConfig.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;
            }
            else if (ExtensionMethods.GetConfigurationSetting("loggingLevel") == Common.Logger.LogType.DIAGNOSTIC.ToString())
            {
                diagnosticsConfig.Logs.ScheduledTransferLogLevelFilter = LogLevel.Warning;
            }

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticsConfig);

            Logger.LogDiagnostic("Starting WebRole");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
