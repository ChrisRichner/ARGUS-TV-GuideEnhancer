using Nancy;

namespace EnricherClient
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ =>View["index.html"];
        }
    }
}