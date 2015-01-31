namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using GuideEnricher.Model;
    using log4net;
    using TvdbLib.Data;
    using System;

    public class NoPunctuationMatchMethod : MatchMethodBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string REMOVE_PUNCTUATION_REG_EX = @"[^ a-zA-Z]";

        public override string MethodName
        {
            get { return "No Punctuation"; }
        }

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (guideProgram == null) throw new ArgumentNullException("enrichedGuideProgram");
            if (IsStringPropertyNull(guideProgram, guideProgram.SubTitle, "SubTitle")) return false;
            //
            this.MatchAttempts++;
            foreach (var episode in episodes)
            {
                if (!string.IsNullOrEmpty(episode.EpisodeName))
                {
                    if (Regex.Replace(episode.EpisodeName, REMOVE_PUNCTUATION_REG_EX, string.Empty) == Regex.Replace(guideProgram.SubTitle, REMOVE_PUNCTUATION_REG_EX, string.Empty))
                    {
                        return this.Matched(guideProgram, episode);
                    }
                }
            }

            return this.Unmatched(guideProgram);
        }
    }
}