using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuideEnricher
{
    class ForTheRecordListener : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _eventsClientId;
        private SynchronizationContext _uiSyncContext;

        private Task _eventListenerTask;
        private CancellationTokenSource _listenerCancellationTokenSource;

        public ForTheRecordListener()
        {
            _uiSyncContext = SynchronizationContext.Current == null ? new SynchronizationContext() : SynchronizationContext.Current;

            _eventsClientId = String.Format("{0}-{1}-99b8cd44d1ab459cb16f199a48086588", // Unique for the Notifier!
                Dns.GetHostName(), System.Environment.GetEnvironmentVariable("SESSIONNAME"));
            //StartEventListenerTask();
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

        #region ARGUS TV Events

        public void OnNewGuideData()
        {
            Service.Enrich(null, null);
        }

        public void OnUpcomingSuggestionsChanged()
        {
            Service.Enrich(null, null);
        }

        public void OnUpcomingRecordingsChanged()
        {
            Service.Enrich(null, null);
        }

        public void OnScheduleChanged()
        {
            Service.Enrich(null, null);
        }

        #endregion

        #region Connection

        private bool _eventListenerSubscribed;
        private int _eventsErrorCount;

        public bool IsConnected { get; set; }

        private void ConnectAndHandleEvents(CancellationToken cancellationToken)
        {
            log.Info("Connection and event listener task started...");

            _eventListenerSubscribed = false;
            _eventsErrorCount = 0;

            for (; ; )
            {
                if (Proxies.IsInitialized)
                {
                    IList<ServiceEvent> events = null;
                    if (!_eventListenerSubscribed)
                    {
                        try
                        {
                            Proxies.CoreService.SubscribeServiceEvents(_eventsClientId, EventGroup.GuideEvents | EventGroup.ScheduleEvents).Wait();
                            _eventListenerSubscribed = true;
                            _eventsErrorCount = 0;

                            log.Info("SubscribeServiceEvents() succeeded");

                            this.IsConnected = true;
                            _uiSyncContext.Post(s => RefreshStatus(), null);
                        }
                        catch (Exception ex)
                        {
                            log.Warn("SubscribeServiceEvents() failed: " + ex.ToString());
                        }
                    }
                    if (_eventListenerSubscribed)
                    {
                        try
                        {
                            this.IsConnected = true;
                            _uiSyncContext.Post(s => RefreshStatus(), null);

                            events = Proxies.CoreService.GetServiceEvents(_eventsClientId, cancellationToken).Result;
                            if (events == null)
                            {
                                _eventListenerSubscribed = false;
                                _uiSyncContext.Post(s => RefreshStatus(), null);
                            }
                            else
                            {
                                if (events.Count == 0)
                                {
                                    // In case of a timeout, let's refresh the general status -- to make sure we don't miss any events.
                                    _uiSyncContext.Post(s => RefreshStatus(), null);
                                }
                                ProcessEvents(events);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is ArgusTVNotFoundException
                                || ++_eventsErrorCount > 5)
                            {
                                _eventListenerSubscribed = false;
                                _uiSyncContext.Post(s => log.Warn("Connection to Argus TV lost, make sure the Argus TV service is running"), null);
                            }
                        }
                    }
                }
                else
                {
                    _uiSyncContext.Send(s => InitializeConnectionToArgusTV(), null);
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
                    Proxies.CoreService.UnsubscribeServiceEvents(_eventsClientId).Wait();
                }
                catch
                {
                }
                _eventListenerSubscribed = false;
            }
        }

        private void ProcessEvents(IList<ServiceEvent> events)
        {
            foreach (var @event in events)
            {
                if (@event.Name == ServiceEventNames.UpcomingRecordingsChanged)
                {
                    _uiSyncContext.Post(s => OnUpcomingRecordingsChanged(), null);
                }
                else if (@event.Name == ServiceEventNames.ScheduleChanged)
                {
                    _uiSyncContext.Post(s => OnScheduleChanged(), null);
                }
                else if (@event.Name == ServiceEventNames.NewGuideData)
                {
                    _uiSyncContext.Post(s => OnNewGuideData(), null);
                }
                else if (@event.Name == ServiceEventNames.UpcomingSuggestionsChanged)
                {
                    _uiSyncContext.Post(s => OnUpcomingSuggestionsChanged(), null);
                }
            }
        }

        #endregion

        private void InitializeConnectionToArgusTV()
        {
            this.IsConnected = Service.InitializeConnectionToArgusTV();
            if (this.IsConnected)
            {

            }
        }

        public void RefreshStatus()
        {
            Service.Enrich(null, null);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
