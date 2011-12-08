using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Lokad.Cloud.AppHost.Framework;

namespace App2
{
    class EntryPoint : IApplicationEntryPoint
    {
        readonly ActionLogger _logger;

        public EntryPoint()
        {
            var writer = File.AppendText("app2_log.txt");
            _logger = new ActionLogger((format, args) =>
                {
                    writer.WriteLine(string.Format(format, args));
                    writer.Flush();
                });
        }

        public void Run(XElement settings, IDeploymentReader deploymentReader, IApplicationEnvironment environment, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.Write("[App2] {0} {1}: Timestamp {2}", environment.CurrentDeploymentName, environment.CellName, DateTime.Now);

                cancellationToken.WaitHandle.WaitOne(5000);
            }
        }

        public void ApplyChangedSettings(XElement settings)
        {
            _logger.Write("[App2] SettingsChanged: {0}", settings.ToString());
        }
    }
}
