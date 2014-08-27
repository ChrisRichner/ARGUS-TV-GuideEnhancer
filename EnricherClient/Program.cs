using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Self;

namespace EnricherClient
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(@"log4net.config"));
            var host = new NancyHost(new Uri("http://localhost:8080"));
            
            host.Start(); // start hosting

            Console.ReadKey();
            host.Stop(); // stop hosting
        }
    }
}
