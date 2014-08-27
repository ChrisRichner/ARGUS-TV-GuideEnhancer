@ECHO OFF
REM This will build the wix project for default Wix install path on x64 Windows
msbuild Installer.wixproj /p:WixTargetsPath="C:\Program Files (x86)\MSBuild\Microsoft\WiX\v3.x\wix.targets";Configuration=Release;WixToolPath="c:\Program Files (x86)\Windows Installer XML v3.6\bin";WixTasksPath="c:\Program Files (x86)\MSBuild\Microsoft\WiX\v3.x\WixTasks.dll"
