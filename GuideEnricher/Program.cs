namespace GuideEnricher
{
    using System.Reflection;
    using log4net;
    using Topshelf;


    //    http://localhost:49943/ArgusTV/Core/help
    //    http://localhost:49943/ArgusTV/Scheduler/help
    //    http://localhost:49943/ArgusTV/Control/help
    //    http://localhost:49943/ArgusTV/Guide/help
    //    http://localhost:49943/ArgusTV/Configuration/help
    //    http://localhost:49943/ArgusTV/Log/help

    public static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
                {
                    x.Service<Service>(s =>
                                            {                                                
                                                s.ConstructUsing(name => new Service());
                                                s.WhenStarted(service => service.Start());
                                                s.WhenStopped(service => service.Stop());
                                            });
                    x.RunAsLocalSystem();
                    x.SetServiceName("Guide Enricher");
                    x.SetDescription("Fetches season and episode information from TheTVDB for For The Record's guide");
                    x.SetDisplayName("Guide Enricher");
                    x.SetServiceName("GuideEnricher");
                });
        }
    }
}
