#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("GuideEnricher")]
[assembly: AssemblyDescription("Fetches season and episode information from TheTVDB for ARGUS-TV's guide")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("https://github.com/ChrisRichner/ARGUS-TV-GuideEnhancer")]
[assembly: AssemblyProduct("GuideEnricher")]
[assembly: AssemblyCopyright("Copyright 2015 GitHub Contributors")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("2.3.3.*")]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
