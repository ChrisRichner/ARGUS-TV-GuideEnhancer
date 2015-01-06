namespace GuideEnricher
{
    using ArgusTV.DataContracts;
    using ArgusTV.ServiceProxy;
    using GuideEnricher.Config;
    using GuideEnricher.tvdb;
    using log4net;
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using System.Timers;
    using Timer = System.Timers.Timer;

    public class Service
    {
        private const string MODULE = "GuideEnricher";

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IConfiguration config = Config.Config.Instance;
        private static Timer ftrConnectionTimer;
        private static Timer enrichTimer;
        public static bool BusyEnriching;
        private static object lockThis = new object();
        private static int waitTime;

        private ForTheRecordListener eventListener;

        public Service()
        {
            eventListener = new ForTheRecordListener(new Action(() => Enrich(null, null)), config, log);
        }

        public void Start()
        {
            try
            {
                log.Info("Starting");
                //ForTheRecordListener.CreateServiceHost(config.getProperty("serviceUrl")).Open();
                eventListener.StartEventListenerTask();

                ftrConnectionTimer = new Timer(500) { AutoReset = false };
                ftrConnectionTimer.Elapsed += this.SetupFTRConnection;
                ftrConnectionTimer.Start();

                if (!int.TryParse(config.GetProperty("sleepTimeInHours"), out waitTime))
                {
                    waitTime = 12;
                }
                enrichTimer = new Timer(TimeSpan.FromSeconds(15).TotalMilliseconds) { AutoReset = false };
                enrichTimer.Elapsed += Enrich;
                enrichTimer.Start();
            }
            catch (Exception ex)
            {
                log.Fatal("Error on starting service", ex);
                throw;
            }
        }

        public void Stop()
        {
            eventListener.CancelEventListenerTask();
            log.Info("Service stopping");
        }

        public void SetupFTRConnection(Object state, ElapsedEventArgs eventArgs)
        {
            try
            {
                if (Proxies.IsInitialized)
                {
                    if (Proxies.CoreService.Ping(Constants.RestApiVersion).Result > 0)
                    {
                        log.Debug("Ping");
                    }
                    return;
                }

                log.Debug("Trying to connect to Argus TV");

                ArgusTV.ServiceProxy.ServerSettings serverSettings = GetServerSettings();
                if (ArgusTV.ServiceProxy.Proxies.Initialize(serverSettings))
                {
                    Proxies.LogService.LogMessage(MODULE, LogSeverity.Information, "GuideEnricher successfully connected");
                    log.Info("Successfully connected to Argus TV");
                }
                else
                {
                    log.Fatal("Unable to connect to Argus TV, check your settings.  Will try again later");
                }
            }
            catch (ArgusTV.ServiceProxy.ArgusTVNotFoundException notFoundException)
            {
                log.Error(notFoundException.Message);
            }
            catch (EndpointNotFoundException)
            {
                log.Error("Connection to Argus TV lost, make sure the Argus TV service is running");
            }
            catch (ArgusTVException ftrException)
            {
                log.Fatal(ftrException.Message);
            }
            finally
            {
                ftrConnectionTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                ftrConnectionTimer.Start();
            }
        }

        public static void Enrich(Object state, ElapsedEventArgs eventArgs)
        {
            try
            {
                lock (lockThis)
                {
                    BusyEnriching = true;
                }
                int ping = Proxies.CoreService.Ping(Constants.RestApiVersion).Result;
                if (ping > 0)
                {
                    log.DebugFormat("Ping {0}", ping);
                }

                var matchMethods = EpisodeMatchMethodLoader.GetMatchMethods();
                var tvDbApi = new TvDbService(config.CacheFolder, config.ApiKey);
                var tvdbLibAccess = new TvdbLibAccess(config, matchMethods, tvDbApi);
                //var enricher = new Enricher(config, ftrlogAgent, tvGuideServiceAgent, tvSchedulerServiceAgent, tvdbLibAccess, matchMethods);
                var enricher = new Enricher(config, tvdbLibAccess, matchMethods);
                enricher.EnrichUpcomingPrograms();
            }
            catch (Exception exception)
            {
                log.Error("Error enriching", exception);
            }
            finally
            {
                lock (lockThis)
                {
                    BusyEnriching = false;
                }

                if (!enrichTimer.Enabled)
                {
                    enrichTimer.Interval = TimeSpan.FromHours(waitTime).TotalMilliseconds;
                    enrichTimer.Start();
                }
            }
        }

        public static ArgusTV.ServiceProxy.ServerSettings GetServerSettings()
        {
            var serverSettings = new ArgusTV.ServiceProxy.ServerSettings();
            serverSettings.ServerName = config.GetProperty("ftrUrlHost");
            serverSettings.Transport = ArgusTV.ServiceProxy.ServiceTransport.Http;
            serverSettings.Port = Convert.ToInt32(config.GetProperty("ftrUrlPort"));
            var password = config.GetProperty("ftrUrlPassword");
            var userName = config.GetProperty("ftrUserName");

            if (!string.IsNullOrEmpty(userName))
            {
                serverSettings.UserName = userName;
            }
            if (!string.IsNullOrEmpty(password))
            {
                serverSettings.Password = password;
            }

            return serverSettings;
        }

        internal static bool InitializeConnectionToArgusTV()
        {
            ArgusTV.ServiceProxy.ServerSettings serverSettings = Service.GetServerSettings();
            return ArgusTV.ServiceProxy.Proxies.Initialize(serverSettings, false);
        }
    }
}