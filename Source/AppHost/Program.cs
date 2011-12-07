using System;
using System.IO;
using System.Threading;
using Lokad.Cloud.AppHost;
using Lokad.Cloud.AppHost.Extensions.FileDeployments;
using Lokad.Cloud.AppHost.Framework;

namespace Source
{
    internal class Program
    {
        readonly CancellationTokenSource _cts;
        readonly Host _host;

        static void Main()
        {
            var appHost = new Program();

            appHost.Start();

            Console.CancelKeyPress += (sender, e) => ConsoleCancelKeyPress(appHost);
        }

        static void ConsoleCancelKeyPress(Program appHost)
        {
            appHost.Stop();
        }

        public Program()
        {
            _cts = new CancellationTokenSource();

            var observer = new HostObserverSubject();
            observer.Subscribe(Console.WriteLine);

            var path = Path.Combine(Path.GetDirectoryName(typeof (Program).Assembly.Location), "Deployments");
            var deploymentReader = new FileDeploymentReader(path);

            var context = new HostContext(observer, deploymentReader);

            _host = new Host(context);
        }

        public void Start()
        {
            _host.RunSync(_cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
