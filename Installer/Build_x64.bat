@ECHO OFF
REM This will build the wix project for default Wix install path on x64 Windows
msbuild Installer.wixproj /p:WixTargetsPath="C:\Program Files (x86)\MSBuild\Microsoft\WiX\v3.x\wix.targets";Configuration=Release;WixToolPath="C:\Program Files (x86)\WiX Toolset v3.8\bin";WixTasksPath="C:\Program Files (x86)\WiX Toolset v3.8\bin\WixTasks.dll"
