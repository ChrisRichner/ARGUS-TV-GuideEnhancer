ARGUS-TV-GuideEnhancer
======================

Electronic Program Guide Enhancer for ARGUS TV 2.3RC+

Pre Requisite
======================

You need to have .Net 4.0 installed on the machine that will host the GuideEnricher. You can get the web install here: http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

And the offline installer: http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7

Installation
======================
If you are going to use the Guide Enricher, follow these steps.

Download the installer
Make sure you have ARGUS-TV 2.3 RC+ installed and running
Run the Guide Enricher installer
Only option is path where to install it...
After a successful install (hopefully), you will have a windows service called GuideEnricher available (on versions higher than 0.5, the install does not start the service).
Logs are put into the Windows logging system. You can view them by doing the following:
right-click on my computer
Manage
System Tools->Event Viewer->Application Logs
Anything with GuideEnricher is for this service.
Try adding a new schedule
View the logs as it cycles through all your schedules and updates the episode information.
If you see errors for series that cannot be found, take a look at the GuideEnricherService.exe.config file in the program directory. You will be able to add series name mappings in there. Restart the service for changes to take effect.
