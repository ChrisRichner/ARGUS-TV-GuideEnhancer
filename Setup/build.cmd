set SOURCEDIR=%CD%
"%WIX%bin\candle.exe" -nologo "%SOURCEDIR%\Product.wxs" -out "%SOURCEDIR%\Setup.wixobj"  -ext WixUIExtension -ext WixNetFxExtension
"%WIX%bin\light.exe" -nologo "%SOURCEDIR%\Setup.wixobj" -out "%SOURCEDIR%\GuideEnricher.msi"  -ext WixUIExtension -ext WixNetFxExtension