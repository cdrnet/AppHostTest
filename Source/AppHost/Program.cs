using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using Lokad.Cloud.AppHost;
using Lokad.Cloud.AppHost.Extensions.FileDeployments;
using Lokad.Cloud.AppHost.Framework;
using Lokad.Cloud.AppHost.Framework.Events;

namespace AppHost
{
    internal class Program
    {
        readonly CancellationTokenSource _cts;
        readonly Host _host;

        static void Main()
        {
            var appHost = new Program();

            appHost.Start();

            Console.ReadKey();
            appHost.Stop();

            Console.ReadKey();
        }

        public Program()
        {
            _cts = new CancellationTokenSource();

            var observer = new HostObserverSubject();
            observer.OfType<HostStartedEvent>().Subscribe(e => Console.WriteLine("AppHost started."));
            observer.OfType<HostStoppedEvent>().Subscribe(e => Console.WriteLine("AppHost stopped."));
            observer.OfType<CellStartedEvent>().Subscribe(e => Console.WriteLine("Cell {0} started.", e.CellName));
            observer.OfType<CellStoppedEvent>().Subscribe(e => Console.WriteLine("Cell {0} stopped.", e.CellName));
            observer.OfType<CellExceptionRestartedEvent>().Subscribe(e => Console.WriteLine("Cell {0} exception: {1}", e.CellName, e.Exception));
            observer.OfType<CellFatalErrorRestartedEvent>().Subscribe(e => Console.WriteLine("Cell {0} fatal error: {1}", e.CellName, e.Exception));

            var path = Path.Combine(Path.GetDirectoryName(typeof (Program).Assembly.Location), "Deployments");
            var deploymentReader = new FileDeploymentReader(path);

            var context = new HostContext(observer, deploymentReader);

            _host = new Host(context);
        }

        public void Start()
        {
            _host.Run(_cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
