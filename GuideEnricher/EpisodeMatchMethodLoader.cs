namespace GuideEnricher
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Config;
    using EpisodeMatchMethods;

    public class EpisodeMatchMethodLoader
    {
        /// <summary>
        /// Use reflection to add comparison methods for episode names
        /// </summary>
        public static List<IEpisodeMatchMethod> GetMatchMethods()
        {
            var matchMethodsSection = ConfigurationManager.GetSection("MatchMethodsSection") as MatchMethodsSection;
            var matchMethods = new List<IEpisodeMatchMethod>(matchMethodsSection.MatchMethods.Count);

            for (int i = 0; i < matchMethodsSection.MatchMethods.Count; i++)
            {
                var type = Type.GetType(matchMethodsSection.MatchMethods[i].MethodName);
                matchMethods.Add(Activator.CreateInstance(type) as IEpisodeMatchMethod);
            }
            
            return matchMethods;
        }
    }
}