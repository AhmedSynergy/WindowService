using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace TestService_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<TestService>(s =>
                {
                    s.ConstructUsing(heartbeat => new TestService());
                    s.WhenStarted(testService => testService.Start());
                    s.WhenStopped(testService => testService.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Takes input.txt as input and outputs files based on users and their orders.");
                x.SetServiceName("TestService");
                x.SetDisplayName("Test Service");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
