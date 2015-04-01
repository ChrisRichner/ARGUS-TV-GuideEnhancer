namespace GuideEnricher
{
    using ArgusTV.DataContracts;
    using ArgusTV.ServiceProxy;
    using Config;
    using EpisodeMatchMethods;
    using Model;
    using tvdb;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TvdbLib.Data;
    using System.Globalization;
    using System.Threading.Tasks;

    public class Enricher
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<GuideEnricherProgram> enrichedPrograms;
        private readonly IConfiguration config;
        private readonly List<IEpisodeMatchMethod> matchMethods;
        private readonly TvdbLibAccess tvdbLibAccess;
        private readonly Dictionary<string, GuideEnricherSeries> seriesToEnrich;

        private const string MODULE = "GuideEnricher";

        public Enricher(IConfiguration configuration, TvdbLibAccess tvdbLibAccess, List<IEpisodeMatchMethod> matchMethods)
        {
            this.config = configuration;
            this.enrichedPrograms = new List<GuideEnricherProgram>();
            this.tvdbLibAccess = tvdbLibAccess;
            this.matchMethods = matchMethods;
            this.seriesToEnrich = new Dictionary<string, GuideEnricherSeries>();
        }

        public async Task EnrichUpcomingProgramsAsync()
        {
            await this.AddUpcomingProgramsAsync(ScheduleType.Suggestion);
            await this.AddUpcomingProgramsAsync(ScheduleType.Recording);

            foreach (var series in this.seriesToEnrich.Values)
            {
                try
                {
                    this.EnrichSeries(series);
                }
                catch (Exception e)
                {
                    log.Error(string.Format(CultureInfo.CurrentCulture, "Error enriching series {0}", series.Title), e.GetBaseException());
                }
            }

            if (this.enrichedPrograms.Count > 0)
            {
                this.UpdateFTRGuideData();
            }
            else
            {
                var message = string.Format("No entries were enriched");
                await Proxies.LogService.LogMessage(MODULE, LogSeverity.Information, message);
                log.Debug(message);
            }

            foreach (var matchMethod in this.matchMethods)
            {
                log.DebugFormat("Match method {0} matched {1} out of {2} attempts", matchMethod.MethodName, matchMethod.SuccessfulMatches, matchMethod.MatchAttempts);
            }
        }
        private async Task AddUpcomingProgramsAsync(ScheduleType scheduleType)
        {
            var schedules = await Proxies.SchedulerService.GetAllSchedules(ChannelType.Television, scheduleType);
            foreach (var scheduleSummary in schedules)
            {
                if (!scheduleSummary.IsActive) continue;
                //
                var schedule = await Proxies.SchedulerService.GetScheduleById(scheduleSummary.ScheduleId);
                // remove schedule filters
                var filtersToRemove = new List<ScheduleRuleType>
                        {
                            { ScheduleRuleType.EpisodeNumberEquals },
                            { ScheduleRuleType.EpisodeNumberContains },
                            { ScheduleRuleType.EpisodeNumberDoesNotContain },
                            { ScheduleRuleType.EpisodeNumberStartsWith },
                            { ScheduleRuleType.NewEpisodesOnly },
                            { ScheduleRuleType.NewTitlesOnly },
                            { ScheduleRuleType.SkipRepeats }
                         };
                schedule.Rules.RemoveAll(x => filtersToRemove.Contains(x.Type));
                //
                // EpisodeDisplayname must have S01E01 format
                var episodeDataValidRegEx = new System.Text.RegularExpressions.Regex(@"S\d\dE\d\d");
                foreach (var program in await Proxies.SchedulerService.GetUpcomingPrograms(schedule, true))
                {
                    var guideProgram = new GuideEnricherProgram(await Proxies.GuideService.GetProgramById(program.GuideProgramId.Value));
                    // skip already enriched entries
                    if (!string.IsNullOrWhiteSpace(guideProgram.EpisodeNumberDisplay) || episodeDataValidRegEx.IsMatch(guideProgram.EpisodeNumberDisplay)) continue;
                    //
                    if (!this.seriesToEnrich.ContainsKey(guideProgram.Title))
                    {
                        this.seriesToEnrich.Add(guideProgram.Title, new GuideEnricherSeries(guideProgram.Title, config.UpdateMatchedEpisodes, config.UpdateSubtitlesParameter, config.UpdateDescription));
                    }
                    this.seriesToEnrich[guideProgram.Title].AddProgram(guideProgram);
                }
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
            int windowSize = Int32.Parse(this.config.GetProperty("maxShowNumberPerUpdate"));
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
            Proxies.GuideService.ImportPrograms(programs, GuideSource.Other);
        }

        public static string FormatSeasonAndEpisode(int season, int episode)
        {
            return String.Format("S{0:00}E{1:00}", season, episode);
        }
    }
}
