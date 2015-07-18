set SOURCEDIR=%1
set productversion=%2
cd SOURCEDIR
"%WIX%\bin\candle.exe" -nologo "%SOURCEDIR%\Product.wxs" -out "%SOURCEDIR%\Setup.wixobj"  -ext WixUIExtension -ext WixNetFxExtension
"%WIX%\bin\light.exe" -b %1 -nologo "%SOURCEDIR%\Setup.wixobj" -out "%SOURCEDIR%\bin\GuideEnricherInstall-v%2.msi"  -ext WixUIExtension -ext WixNetFxExtension