namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Config;
    using Model;
    using log4net;
    using TvdbLib.Data;
    using System;

    public abstract class MatchMethodBase : IEpisodeMatchMethod
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int MatchAttempts { get; protected set; }

        public int SuccessfulMatches { get; protected set; }

        public bool IsMatched { get; private set; }

        public abstract string MethodName { get; }

        public abstract bool Match(GuideEnricherProgram enrichedGuideProgram, List<TvdbEpisode> episodes);

        protected bool Matched(GuideEnricherProgram guideProgram, TvdbEpisode episode)
        {
            if (guideProgram == null) throw new ArgumentNullException("guideProgram");
            if (episode == null) throw new ArgumentNullException("episode");
            //
            this.SuccessfulMatches++;
            guideProgram.EpisodeNumber = episode.EpisodeNumber;
            guideProgram.SeriesNumber = episode.SeasonNumber;
            guideProgram.EpisodeNumberDisplay = Enricher.FormatSeasonAndEpisode(episode.SeasonNumber, episode.EpisodeNumber);

            if (bool.Parse(Config.Instance.GetProperty("updateSubtitles")))
            {
                guideProgram.SubTitle = episode.EpisodeName;
            }

            if (bool.Parse(Config.Instance.GetProperty("episodeInDescription")))
            {
                var descriptionWithNoEpisodeNumber = Regex.Replace(guideProgram.Description, "^S[0-9][0-9]E[0-9][0-9] - ", string.Empty);
                guideProgram.Description = string.Format("{0} - {1}", guideProgram.EpisodeNumberDisplay, descriptionWithNoEpisodeNumber);
            }

            this.log.DebugFormat("[{0}] Correctly matched {1} - {2} as {3}", this.MethodName, guideProgram.Title, guideProgram.SubTitle, guideProgram.EpisodeNumberDisplay);
            return true;
        }

        protected bool Unmatched(GuideEnricherProgram guideProgram)
        {
            this.log.DebugFormat("[{0}] Could not match {1} - {2}", this.MethodName, guideProgram.Title, guideProgram.SubTitle);
            return false;
        }

        protected virtual bool IsStringPropertyNull(GuideEnricherProgram guideProgram, string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.log.DebugFormat("[{0}] {1} - {2:MM/dd hh:mm tt} does not have a \"{3}\"", this.MethodName, guideProgram.Title, guideProgram.StartTime, propertyName);
                return true;
            }
            return false;
        }
    }
}