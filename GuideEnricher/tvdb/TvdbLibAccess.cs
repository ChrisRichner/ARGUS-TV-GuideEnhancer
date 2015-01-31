/*
 * Created by SharpDevelop.
 * User: geoff
 * Date: 3/25/2010
 * Time: 8:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace GuideEnricher.tvdb
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using GuideEnricher.Config;
    using GuideEnricher.EpisodeMatchMethods;
    using GuideEnricher.Exceptions;
    using GuideEnricher.Model;
    using log4net;
    using TvdbLib;
    using TvdbLib.Cache;
    using TvdbLib.Data;
    using TvdbLib.Exceptions;

    /// <summary>
    /// Description of TvdbLibAccess.
    /// </summary>
    public class TvdbLibAccess
    {
        private const string MODULE = "GuideEnricher";
        private const string REMOVE_PUNCTUATION = @"[^ a-zA-Z]";

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConfiguration config;
        private readonly List<IEpisodeMatchMethod> matchMethods;
        private readonly ITvDbService tvDbService;

        private Dictionary<string, string> seriesNameMapping;
        private Dictionary<string, int> seriesIDMapping = new Dictionary<string, int>();
        private Dictionary<string, string> seriesNameRegex = new Dictionary<string, string>();
        private List<string> seriesIgnore = new List<string>();

        private Dictionary<string, int> seriesCache = new Dictionary<string, int>();

        private TvdbLanguage language;

        public TvdbLibAccess(IConfiguration configuration, List<IEpisodeMatchMethod> matchMethods, ITvDbService tvDbService)
        {
            this.config = configuration;
            this.matchMethods = matchMethods;
            this.tvDbService = tvDbService;
            this.Init();
        }

        private void Init()
        {
            seriesNameMapping = this.config.getSeriesNameMap();
            seriesIgnore = this.config.getIgnoredSeries();
            this.language = SetLanguage();
            this.IntializeRegexMappings();
        }

        public void EnrichProgram(GuideEnricherProgram existingProgram, TvdbSeries tvdbSeries)
        {
            if (existingProgram == null) throw new ArgumentNullException("existingProgram");
            if (tvdbSeries == null) throw new ArgumentNullException("tvdbSeries");
            //
            log.DebugFormat("Starting lookup for {0} - {1}", existingProgram.Title, existingProgram.SubTitle);

            foreach (var matchMethod in this.matchMethods)
            {
                try
                {
                    if (matchMethod.Match(existingProgram, tvdbSeries.Episodes))
                    {
                        existingProgram.Matched = true;
                        break;
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Matchmethod \"{0}\" failed with exception \"{1}\"", matchMethod.MethodName, e.GetBaseException().Message, e);
                }
            }
        }

        public void DebugEpisodeDump(TvdbSeries tvdbSeries)
        {
            if (config.GetProperty("dumpepisodes").ToUpper() == "TRUE")
            {
                this.DumpSeriesEpisodes(tvdbSeries);
            }
        }

        public TvdbSeries GetTvdbSeries(int seriesId, bool forceRefresh)
        {
            TvdbSeries tvdbSeries = null;
            return GetTvdbSeries(seriesId, tvdbSeries, forceRefresh);
        }

        private TvdbSeries GetTvdbSeries(int seriesId, TvdbSeries tvdbSeries, bool forceRefresh)
        {
            bool callSuccessful = false;
            int attemptNumber = 0;
            while (attemptNumber++ < 3 && !callSuccessful)
            {
                try
                {
                    tvdbSeries = this.tvDbService.GetSeries(seriesId, this.language, true, false, false);
                    if (forceRefresh)
                    {
                        tvdbSeries = this.tvDbService.ForceReload(tvdbSeries, true, false, false);
                    }

                    callSuccessful = true;
                }
                catch (TvdbException tvdbException)
                {
                    this.log.Debug("TVDB Error getting series", tvdbException);
                }
            }

            return tvdbSeries;
        }

        private void DumpSeriesEpisodes(TvdbSeries series)
        {
            this.log.InfoFormat("Dumping episode info for {0}", series.SeriesName);
            foreach (var episode in series.Episodes)
            {
                this.log.InfoFormat("S{0:00}E{1:00}-{2}", episode.SeasonNumber, episode.EpisodeNumber, episode.EpisodeName);
            }
        }

        private void IntializeRegexMappings()
        {
            if (this.seriesNameMapping == null)
            {
                return;
            }

            foreach (string regex in this.seriesNameMapping.Keys)
            {
                if (regex.StartsWith("regex="))
                {
                    this.seriesNameRegex.Add(regex.Substring(6), this.seriesNameMapping[regex]);
                }
                else if (this.seriesNameMapping[regex].StartsWith("id="))
                {
                    this.seriesIDMapping.Add(regex, int.Parse(this.seriesNameMapping[regex].Substring(3)));
                }
            }
        }

        private TvdbLanguage SetLanguage()
        {
            TvdbLanguage lang = TvdbLanguage.DefaultLanguage;

            List<TvdbLanguage> availableLanguages = this.tvDbService.Languages;
            string selectedLanguage = this.config.GetProperty("TvDbLanguage");

            // if there is a value for TvDbLanguage in the settings, set the right language
            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                lang = availableLanguages.Find(x => x.Abbriviation == selectedLanguage);
                this.log.DebugFormat("Language: {0}", lang.Abbriviation);
            }

            return lang;
        }

        public int getSeriesId(string seriesName)
        {
            // TODO: A few things here.  We should add more intelligence when there is more then 1 match
            // Things like default to (US) or (UK) or what ever is usally the case.  Also, perhaps a global setting that would always take newest series first...

            if (this.IsSeriesIgnored(seriesName))
            {
                return 0;
            }

            if (this.seriesIDMapping.ContainsKey(seriesName))
            {
                var seriesid = this.seriesIDMapping[seriesName];
                log.DebugFormat("SD-TvDb: Direct mapping: series: {0} id: {1}", seriesName, seriesid);
                return seriesid;
            }

            if (seriesCache.ContainsKey(seriesName))
            {
                log.DebugFormat("SD-TvDb: Series cache hit: {0} has id: {1}", seriesName, seriesCache[seriesName]);
                return seriesCache[seriesName];
            }

            string searchSeries = seriesName;

            if (IsSeriesInMappedSeries(seriesName))
            {
                searchSeries = this.seriesNameMapping[seriesName];
            }
            else if (this.seriesNameRegex.Count > 0)
            {
                foreach (string regexEntry in seriesNameRegex.Keys)
                {
                    var regex = new Regex(regexEntry);
                    if (regex.IsMatch(seriesName))
                    {
                        if (seriesNameRegex[regexEntry].StartsWith("replace="))
                        {
                            searchSeries = regex.Replace(seriesName, seriesNameRegex[regexEntry].Substring(8));
                        }
                        else if (seriesNameRegex[regexEntry].StartsWith("id="))
                        {
                            return int.Parse(seriesNameRegex[regexEntry].Substring(3));
                        }
                        else
                        {
                            searchSeries = seriesNameRegex[regexEntry];
                        }

                        log.DebugFormat("SD-TvDb: Regex mapping: series: {0} regex: {1} seriesMatch: {2}", seriesName, regexEntry, searchSeries);
                        break;
                    }
                }
            }

            List<TvdbSearchResult> searchResults = this.tvDbService.SearchSeries(searchSeries, this.language);

            log.DebugFormat("SD-TvDb: Search for {0} return {1} results", searchSeries, searchResults.Count);

            var seriesWithoutPunctuation = Regex.Replace(searchSeries, REMOVE_PUNCTUATION, string.Empty);
            if (searchResults.Count >= 1)
            {
                for (int i = 0; i < searchResults.Count; i++)
                {
                    if (seriesWithoutPunctuation.Equals(Regex.Replace(searchResults[i].SeriesName, REMOVE_PUNCTUATION, string.Empty), StringComparison.InvariantCultureIgnoreCase))
                    {
                        var seriesId = searchResults[i].Id;
                        log.DebugFormat("SD-TvDb: series: {0} id: {1}", searchSeries, seriesId);
                        seriesCache.Add(seriesName, seriesId);
                        return seriesId;
                    }
                }

                log.DebugFormat("SD-TvDb: Could not find series match: {0} renamed {1}", seriesName, searchSeries);
            }

            log.DebugFormat("Cannot find series ID for {0}", seriesName);
            return 0;
        }

        private bool IsSeriesIgnored(string seriesName)
        {
            if (this.seriesIgnore == null)
            {
                return false;
            }

            if (this.seriesIgnore.Contains(seriesName))
            {
                this.log.DebugFormat("{0}: Series {1} is ignored", MODULE, seriesName);
                return true;
            }
            return false;
        }

        private bool IsSeriesInMappedSeries(string seriesName)
        {
            if (this.seriesNameMapping == null)
            {
                return false;
            }

            return this.seriesNameMapping.ContainsKey(seriesName);
        }
    }
}
