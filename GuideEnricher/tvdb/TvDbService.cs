namespace GuideEnricher.tvdb
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TvdbLib;
    using TvdbLib.Cache;
    using TvdbLib.Data;

    public class TvDbService : ITvDbService, IDisposable
    {
        private readonly TvdbHandler tvdbHandler;

        public List<TvdbSearchResult> SearchSeries(string name, TvdbLanguage language)
        {
            return Retry.Do(() => this.tvdbHandler.SearchSeries(name, language), TimeSpan.FromSeconds(2));
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
            return Retry.Do(() => this.tvdbHandler.GetSeries(seriesId, language, loadEpisodes, loadActors, loadBanners), TimeSpan.FromSeconds(2));
        }

        public TvdbSeries ForceReload(TvdbSeries series, bool loadEpisodes, bool loadActors, bool loadBanners)
        {
            return Retry.Do(() => this.tvdbHandler.ForceReload(series, loadEpisodes, loadActors, loadBanners), TimeSpan.FromSeconds(2));
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

    public static class Retry
    {
        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Task.Delay(retryInterval).Wait();
                }
            }

            throw new AggregateException(exceptions);
        }        
    }
}