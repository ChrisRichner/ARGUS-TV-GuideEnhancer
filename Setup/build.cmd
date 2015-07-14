set SOURCEDIR=%1
cd SOURCEDIR
"C:\Program Files (x86)\WiX Toolset v3.9\bin\candle.exe" -nologo "%SOURCEDIR%\Product.wxs" -out "%SOURCEDIR%\Setup.wixobj"  -ext WixUIExtension -ext WixNetFxExtension
"C:\Program Files (x86)\WiX Toolset v3.9\bin\light.exe" -b %1 -nologo "%SOURCEDIR%\Setup.wixobj" -out "%SOURCEDIR%\GuideEnricherInstall-v%2.msi"  -ext WixUIExtension -ext WixNetFxExtension