using System;
using Serilog;
using Topshelf;

namespace PerfWcf.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();

            var rc = HostFactory.Run(x =>
            {
                x.Service<GlobalStateService>(s =>
                {
                    s.ConstructUsing(name => new GlobalStateService());
                    s.WhenStarted(tc => tc.Start());                         
                    s.WhenStopped(tc => tc.Stop());                          
                });
                x.RunAsLocalSystem();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
