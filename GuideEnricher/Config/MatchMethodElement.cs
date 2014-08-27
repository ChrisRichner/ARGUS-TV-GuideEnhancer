namespace GuideEnricher.Config
{
    using System.Configuration;

    public class MatchMethodElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string MethodName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
    }
}