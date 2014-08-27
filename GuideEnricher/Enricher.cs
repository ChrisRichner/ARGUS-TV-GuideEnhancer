namespace GuideEnricher
{
    using ArgusTV.DataContracts;
    using ArgusTV.ServiceProxy;
    using GuideEnricher.Config;
    using GuideEnricher.EpisodeMatchMethods;
    using GuideEnricher.Model;
    using GuideEnricher.tvdb;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TvdbLib.Data;

    public class Enricher
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<GuideEnricherProgram> enrichedPrograms;
        //private readonly ISchedulerService tvSchedulerService;
        //private readonly IGuideService tvGuideService;
        private readonly IConfiguration config;
        //private readonly ILogService ftrlogAgent;
        private readonly List<IEpisodeMatchMethod> matchMethods;
        private readonly TvdbLibAccess tvdbLibAccess;
        private readonly Dictionary<string, GuideEnricherSeries> seriesToEnrich;

        private const string MODULE = "GuideEnricher";

        public Enricher(IConfiguration configuration, TvdbLibAccess tvdbLibAccess, List<IEpisodeMatchMethod> matchMethods)
        {
            this.config = configuration;
            this.enrichedPrograms = new List<GuideEnricherProgram>();
            //this.ftrlogAgent = ftrLogService;
            //this.tvGuideService = tvGuideService;
            //this.tvSchedulerService = tvSchedulerService;
            this.tvdbLibAccess = tvdbLibAccess;
            this.matchMethods = matchMethods;
            this.seriesToEnrich = new Dictionary<string, GuideEnricherSeries>();
        }

        public void EnrichUpcomingPrograms()
        {
            this.AddUpcomingPrograms(ScheduleType.Suggestion);
            this.AddUpcomingPrograms(ScheduleType.Recording);

            foreach (var series in this.seriesToEnrich.Values)
            {
                this.EnrichSeries(series);
            }

            if (this.enrichedPrograms.Count > 0)
            {
                this.UpdateFTRGuideData();
            }
            else
            {
                var message = string.Format("No entries were enriched");
                Proxies.LogService.LogMessage(MODULE, LogSeverity.Information, message);
                log.Debug(message);
            }

            foreach (var matchMethod in this.matchMethods)
            {
                log.DebugFormat("Match method {0} matched {1} out of {2} attempts", matchMethod.MethodName, matchMethod.SuccessfulMatches, matchMethod.MatchAttempts);
            }
        }

        private void AddUpcomingPrograms(ScheduleType scheduleType)
        {
            var programs = Proxies.SchedulerService.GetAllUpcomingPrograms(scheduleType, true).Result;//this.tvSchedulerService.GetAllUpcomingPrograms(scheduleType, true);
            foreach (var program in programs.Where(p => p.Channel.ChannelType == ChannelType.Television))
            {
                if (!program.GuideProgramId.HasValue)
                {
                    log.DebugFormat("[{0}] - {1:MM/dd hh:mm tt} does not have a guide program entry", program.Title, program.StartTime);
                    break;
                }

                var guideProgram = new GuideEnricherProgram(Proxies.GuideService.GetProgramById(program.GuideProgramId.Value).Result);//new GuideEnricherProgram(this.tvGuideService.GetProgramById(program.GuideProgramId.Value));
                if (!this.seriesToEnrich.ContainsKey(guideProgram.Title))
                {
                    this.seriesToEnrich.Add(guideProgram.Title, new GuideEnricherSeries(guideProgram.Title, config.UpdateMatchedEpisodes, config.UpdateSubtitlesParameter, config.UpdateDescription));
                }

                this.seriesToEnrich[guideProgram.Title].AddProgram(guideProgram);
            }
        }

        public void EnrichSeries(GuideEnricherSeries series)
        {
            series.TvDbSeriesID = this.tvdbLibAccess.getSeriesId(series.Title);
            if (series.TvDbSeriesID == 0)
            {
                series.isIgnored = true;
            }

            if (series.isIgnored)
            {
                series.IgnoredPrograms.AddRange(series.PendingPrograms);
                series.PendingPrograms.Clear();
            }

            if (series.PendingPrograms.Count > 0)
            {
                log.DebugFormat("Beginning enrichment of episodes for series {0}", series.Title);
                var onlineSeries = this.tvdbLibAccess.GetTvdbSeries(series.TvDbSeriesID, false);
                this.EnrichProgramsInSeries(series, onlineSeries);
                if (series.FailedPrograms.Count > 0)
                {
                    log.DebugFormat("The first run for the series {0} had unmatched episodes.  Checking for online updates.", series.Title);

                    List<string> currentTvDbEpisodes = new List<string>();
                    onlineSeries.Episodes.ForEach(x => currentTvDbEpisodes.Add(x.EpisodeName));

                    TvdbSeries updatedOnlineSeries = this.tvdbLibAccess.GetTvdbSeries(series.TvDbSeriesID, true);
                    if (updatedOnlineSeries.Episodes.FindAll(x => !currentTvDbEpisodes.Contains(x.EpisodeName)).Count > 0)
                    {
                        log.DebugFormat("New episodes were found.  Trying enrichment again.");
                        series.TvDbInformationRefreshed();
                        this.EnrichProgramsInSeries(series, updatedOnlineSeries);
                    }
                }

                this.enrichedPrograms.AddRange(series.SuccessfulPrograms);
            }
        }

        private void EnrichProgramsInSeries(GuideEnricherSeries series, TvdbSeries OnlineSeries)
        {
            this.tvdbLibAccess.DebugEpisodeDump(OnlineSeries);
            do
            {
                GuideEnricherProgram guideProgram = series.PendingPrograms[0];
                this.tvdbLibAccess.EnrichProgram(guideProgram, OnlineSeries);
                if (guideProgram.Matched)
                {
                    series.AddAllToEnrichedPrograms(guideProgram);
                }
                else
                {
                    series.AddAllToFailedPrograms(guideProgram);
                }
            } 
            while (series.PendingPrograms.Count > 0);
            
        }

        private void UpdateFTRGuideData()
        {
            log.DebugFormat("About to commit enriched guide data. {0} entries were enriched", this.enrichedPrograms.Count);
            Proxies.LogService.LogMessage(MODULE, LogSeverity.Information, String.Format("About to commit enriched guide data. {0} entries were enriched.", this.enrichedPrograms.Count));

            int position = 0;
            int windowSize = Int32.Parse(this.config.getProperty("maxShowNumberPerUpdate"));
            List<GuideProgram> guidesToUpdate;

            while (position + windowSize < this.enrichedPrograms.Count)
            {
                log.DebugFormat("Importing shows {0} to {1}", position + 1, position + windowSize + 1);
                guidesToUpdate = new List<GuideProgram>();
                List<GuideProgram> update = guidesToUpdate;
                this.enrichedPrograms.GetRange(position, windowSize).ForEach(x => update.Add(x.GuideProgram));
                this.UpdateForTheRecordPrograms(guidesToUpdate.ToArray());
                position += windowSize;
            }

            log.DebugFormat("Importing shows {0} to {1}", position + 1, this.enrichedPrograms.Count);
            guidesToUpdate = new List<GuideProgram>();
            this.enrichedPrograms.GetRange(position, this.enrichedPrograms.Count - position).ForEach(x => guidesToUpdate.Add(x.GuideProgram));
            this.UpdateForTheRecordPrograms(guidesToUpdate.ToArray());
        }

        private void UpdateForTheRecordPrograms(GuideProgram[] programs)
        {
            ArgusTV.ServiceProxy.Proxies.GuideService.ImportPrograms(programs, GuideSource.Other);
            //this.tvGuideService.ImportPrograms(programs, GuideSource.Other);
        }

        public static string FormatSeasonAndEpisode(int season, int episode)
        {
            return String.Format("S{0:00}E{1:00}", season, episode);
        }
    }
}
