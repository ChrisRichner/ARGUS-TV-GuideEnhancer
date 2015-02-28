namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Model;
    using log4net;
    using TvdbLib.Data;
    using System;

    public class RemoveCommonWordsMatchMethod : MatchMethodBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string COMMON_WORDS_REG_EX = @"1|2|3|4|5|6|7|8|9|0|\(|\)|&|part|in|,|is|and|a|the|I|X|V|-|%|percent|!|#| ";

        public override string MethodName
        {
            get { return "Remove common words"; }
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
                    if (Regex.Replace(episode.EpisodeName, COMMON_WORDS_REG_EX, string.Empty, RegexOptions.IgnoreCase).ToLower() == Regex.Replace(guideProgram.SubTitle, COMMON_WORDS_REG_EX, string.Empty, RegexOptions.IgnoreCase).ToLower())
                    {
                        return this.Matched(guideProgram, episode);
                    }
                }
            }

            return this.Unmatched(guideProgram);
        }
    }
}