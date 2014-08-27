using System;
using Nancy;

namespace EnricherClient
{
    public class ProgramsModule : NancyModule
    {
        public ProgramsModule()
        {
            Get["/programs"] = _ =>
                {
                    dynamic model = new Uri("http://localhost:49943/ArgusTV/Scheduler/Schedules/0/82").GetDynamicJsonObject();
                    return View["list", new { title = "title"}];
                };
        }
    }
}