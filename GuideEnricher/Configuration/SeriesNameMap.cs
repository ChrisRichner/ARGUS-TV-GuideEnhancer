namespace GuideEnricher.Config
{
    using System.Configuration;

    public class SeriesNameMap : ConfigurationElement
    {
        // Create the element.
        public SeriesNameMap()
        { }

        // dummy constructor to make collection happy
        public SeriesNameMap(string nothing)
        {
            SchedulesDirectName = nothing;
            TvdbComName = "invalid";
            Ignore = false;
        }

        // Create the element.
        public SeriesNameMap(string sdName, string tvdbName)
        {
            SchedulesDirectName = sdName;
            TvdbComName = tvdbName;
            Ignore = false;
        }

        [ConfigurationProperty("schedulesDirectName", DefaultValue = "", IsRequired = true)]
        public string SchedulesDirectName
        {
            get
            {
                return (string)this["schedulesDirectName"];
            }
            
            set
            {
                this["schedulesDirectName"] = value;
            }
        }

        [ConfigurationProperty("tvdbComName", DefaultValue = "", IsRequired = true)]
        public string TvdbComName
        {
            get
            {
                return (string)this["tvdbComName"];
            }
            
            set
            {
                this["tvdbComName"] = value;
            }
        }

        [ConfigurationProperty("ignore", DefaultValue = "false", IsRequired = false)]
        public bool Ignore
        {
            get
            {
                return (bool)this["ignore"];
            }
            
            set
            {
                this["ignore"] = value;
            }
        }

        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            bool ret = base.SerializeElement(writer, serializeCollectionKey);
            // You can enter your custom processing code here.
            return ret;

        }

        protected override bool IsModified()
        {
            bool ret = base.IsModified();
            // You can enter your custom processing code here.
            return ret;
        }
    }
}