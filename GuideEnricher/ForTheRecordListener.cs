using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using GuideEnricher.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GuideEnricher
{
    class ForTheRecordListener : IDisposable
    {
        #region fields
        private readonly ILog _logger;
        private string _eventsClientId;

        private Task _eventListenerTask;
        private CancellationTokenSource _listenerCancellationTokenSource;
        readonly Action _enrich;
        readonly IConfiguration _configuration;

        protected const string MODULE = "GuideEnricher";
        #endregion

        #region ctor
        public ForTheRecordListener(Action enrich, IConfiguration configuration, ILog logger)
        {
            if (enrich == null) throw new ArgumentNullException("enrich");
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (logger == null) throw new ArgumentNullException("logger");
            //
            _enrich = enrich;
            _logger = logger;
            _configuration = configuration;
            //
            _eventsClientId = Guid.NewGuid().ToString();// string.Format("{0}-{1}-99b8cd44d1ab459cb16f199a48086588", Dns.GetHostName(), Environment.GetEnvironmentVariable("SESSIONNAME"));
        }
        #endregion

        #region Listener
        public void StartEventListenerTask()
        {
            _listenerCancellationTokenSource = new CancellationTokenSource();
            _eventListenerTask = new Task(() => ConnectAndHandleEvents(_listenerCancellationTokenSource.Token),
                _listenerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _eventListenerTask.Start();
        }

        public void CancelEventListenerTask()
        {
            try
            {
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Cancel();
                    _eventListenerTask.Wait();
                }
            }
            catch
            {
                // Swallow
            }
            finally
            {
                if (_eventListenerTask != null)
                {
                    _eventListenerTask.Dispose();
                    _eventListenerTask = null;
                }
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Dispose();
                    _listenerCancellationTokenSource = null;
                }
            }
        }

        public void RestartEventListenerTask()
        {
            CancelEventListenerTask();
            StartEventListenerTask();
        }

        #endregion

        #region Connection

        private bool _eventListenerSubscribed;
        private int _eventsErrorCount;

        private async void ConnectAndHandleEvents(CancellationToken cancellationToken)
        {
            _logger.Info("Connection and event listener task started...");

            _eventListenerSubscribed = false;
            _eventsErrorCount = 0;

            for (; ;)
            {
                if (Proxies.IsInitialized)
                {
                    IList<ServiceEvent> events = null;
                    if (!_eventListenerSubscribed)
                    {
                        try
                        {
                            await Proxies.CoreService.SubscribeServiceEvents(_eventsClientId, EventGroup.GuideEvents | EventGroup.ScheduleEvents);
                            _eventListenerSubscribed = true;
                            _eventsErrorCount = 0;

                            _logger.Info("SubscribeServiceEvents() succeeded");
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn("SubscribeServiceEvents() failed: " + ex.ToString());
                        }
                    }
                    if (_eventListenerSubscribed)
                    {
                        try
                        {
                            events = await Proxies.CoreService.GetServiceEvents(_eventsClientId, cancellationToken);
                            if (events == null)
                            {
                                _eventListenerSubscribed = false;
                            }
                            else
                            {
                                await Task.Run(_enrich);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is ArgusTVNotFoundException
                                || ++_eventsErrorCount > 5)
                            {
                                _eventListenerSubscribed = false;
                                _logger.Warn("Connection to Argus TV lost, make sure the Argus TV service is running");
                            }
                        }
                    }
                }
                else
                {
                    await InitializeConnectionToArgusTV();
                }
                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_eventListenerSubscribed ? 0 : 10)))
                {
                    break;
                }
            }

            if (Proxies.IsInitialized
                && _eventListenerSubscribed)
            {
                try
                {
                    await Proxies.CoreService.UnsubscribeServiceEvents(_eventsClientId);
                }
                catch
                {
                }
                _eventListenerSubscribed = false;
            }
        }

        ServerSettings GetServerSettings()
        {
            var serverSettings = new ServerSettings();
            serverSettings.ServerName = _configuration.GetProperty("ftrUrlHost");
            serverSettings.Port = Convert.ToInt32(_configuration.GetProperty("ftrUrlPort"));
            var password = _configuration.GetProperty("ftrUrlPassword");
            var userName = _configuration.GetProperty("ftrUserName");

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

        protected async Task<bool> InitializeConnectionToArgusTV()
        {
            ServerSettings serverSettings = GetServerSettings();
            return await Proxies.InitializeAsync(serverSettings, false);
        }
        #endregion

        #region IDisposeable
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelEventListenerTask();
            }
        }
        #endregion
    }
}
