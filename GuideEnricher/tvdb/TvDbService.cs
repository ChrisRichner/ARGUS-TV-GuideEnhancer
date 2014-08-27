namespace GuideEnricher.tvdb
{
    using System;
    using System.Collections.Generic;

    using TvdbLib;
    using TvdbLib.Cache;
    using TvdbLib.Data;

    public class TvDbService : ITvDbService, IDisposable
    {
        private readonly TvdbHandler tvdbHandler;

        public List<TvdbSearchResult> SearchSeries(string name, TvdbLanguage language)
        {
            return this.tvdbHandler.SearchSeries(name, language);
        }

        public List<TvdbLanguage> Languages
        {
            get
            {
                return this.tvdbHandler.Languages;
            }
        }

        public TvdbSeries GetSeries(int seriesId, TvdbLanguage language, bool loadEpisodes, bool loadActors, bool loadBanners)
        {
            return this.tvdbHandler.GetSeries(seriesId, language, loadEpisodes, loadActors, loadBanners);
        }

        public TvdbSeries ForceReload(TvdbSeries series, bool loadEpisodes, bool loadActors, bool loadBanners)
        {
            return this.tvdbHandler.ForceReload(series, loadEpisodes, loadActors, loadBanners);
        }

        public TvDbService(string cacheFolder, string apiKey)
        {
            this.tvdbHandler = new TvdbHandler(new XmlCacheProvider(cacheFolder), apiKey);
            this.tvdbHandler.InitCache();
        }

        public void Dispose()
        {
            if (this.tvdbHandler != null)
            {
                this.tvdbHandler.CloseCache();
            }
        }
    }
}