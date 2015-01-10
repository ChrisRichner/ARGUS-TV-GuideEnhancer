#ARGUS-TV-GuideEnhancer
**Electronic Program Guide Enhancer for ARGUS TV 2.3.0+**

[ARGUS TV](http://http://www.argus-tv.com/ "ARGUS TV") is a sophisticated scheduling/recording engine. It has it's own Electronic Program Guide (EPG). You can populate the guide from many sources (i.e., xmltv, SchedulesDirect, etc.).

Regardless of the method with which you populate the EPG, you may want to "enrich" your guide data to add Season and Episode numbers (SxxExx). This "enrich"-ing is useful for [XBMC/Kodi](http://www.kodi.tv "Kodi") or [MediaPortal](http://www.team-mediaportal.com/ "") plugin TVSeries that automatically download episode information, backdrops, ratings, etc. By enriching your guide data, you can automatically have the SxxExx information in your file names and thus have TVSeries register your recordings with no manual intervention.

##Getting Started
If you are going to use the Guide Enricher, follow these steps.

###Pre Requisite
- You need to have .Net 4.0 installed on the machine that will host the GuideEnricher. You can get the web install here: http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992
- And the offline installer: http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7

###Installation
- Download the installer
- Make sure you have ARGUS-TV 2.3.0+ installed and running
- Run the Guide Enricher installer
- Only option is path where to install it...
- After a successful install (hopefully), you will have a windows service called GuideEnricher available

###Configuration
If you see errors for series that cannot be found, take a look at the GuideEnricherService.exe.config file in the program directory. You will be able to add series name mappings in there. Restart the service for changes to take effect.

###Troubleshoot
======================
