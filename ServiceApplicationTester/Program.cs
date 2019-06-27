using System;
using Topshelf;

namespace ServiceApplicationTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = HostFactory.New(x => { 
                x.SetServiceName("TestService");
                x.SetDisplayName("Test Service");
                x.StartAutomaticallyDelayed();
                x.Service<MyTestService>(sc =>
            {
                sc.ConstructUsing(() => new MyTestService());
                sc.WhenStarted(s => s.Start());
                sc.WhenStopped(s => s.Stop());
            }); });
            result.Run();
        }
    }
}